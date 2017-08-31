using System;
using System.Collections.Generic;
using Lynx.Core.Communications.Packets.Interfaces;
using Newtonsoft.Json;

namespace Lynx.Core.Communications.Packets
{
    public abstract class Token : IToken
    {
        private bool _signedAndlocked;
        private string _signature;
        public string Signature
        {
            get
            {
                return _signature;
            }
        }
        public bool Locked { get { return _signedAndlocked; } }
        private Dictionary<string, string> _header;
        private Dictionary<string, string> _payload;

        protected Token()
        {
            _header = new Dictionary<string, string>();
            _payload = new Dictionary<string, string>();
        }

        protected Token(Dictionary<string, string> header, Dictionary<string, string> payload)
        {
            _header = header;
            _payload = payload;
        }

        public string PublicKey
        {
            get
            {
                return GetFromHeader("pubkey");
            }

            set
            {
                if (value != null)
                {
                    SetOnHeader("pubkey", value);
                }
                else
                {
                    RemoveFromPayload("idAddr");
                }
            }
        }

        public bool Encrypted
        {
            get
            {
                return Boolean.Parse(GetFromHeader("encrypted"));
            }

            set
            {
                SetOnHeader("encrypted", value.ToString());
            }
        }

        public void SignAndLock(string signature)
        {
            EnsureTokenIsNotSignedAndLocked();
            _signedAndlocked = true;
            _signature = signature;
        }

        public string GetEncodedToken()
        {
            return GetUnsignedEncodedToken() + (Signature == null || Signature.Equals("") ? "" : "." + Signature);
        }

        private string Base64Encode(string plainText)
        {
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        private string Base64Decode(string encodedText)
        {
            byte[] plainTextBytes = Convert.FromBase64String(encodedText);
            return System.Text.Encoding.UTF8.GetString(plainTextBytes, 0, plainTextBytes.Length);
        }

        public void SetOnHeader(string key, string val)
        {
            EnsureTokenIsNotSignedAndLocked();

            //Existing key value pairs are overwritten
            if (_header.ContainsKey(key))
                RemoveFromHeader(key);

            _header.Add(key, val);
        }

        public void SetOnPayload(string key, string val)
        {
            EnsureTokenIsNotSignedAndLocked();

            //Existing key value pairs are overwritten
            if (_payload.ContainsKey(key))
                RemoveFromPayload(key);

            _payload.Add(key, val);
        }

        public void RemoveFromHeader(string key)
        {
            EnsureTokenIsNotSignedAndLocked();
            _header.Remove(key);
        }

        public void RemoveFromPayload(string key)
        {
            EnsureTokenIsNotSignedAndLocked();
            _payload.Remove(key);
        }

        public string GetFromHeader(string key)
        {
            return _header[key];
        }

        public string GetFromPayload(string key)
        {
            return _payload[key];
        }

        public bool HeaderContains(string key)
        {
            return _header.ContainsKey(key);
        }

        public bool PayloadContains(string key)
        {
            return _payload.ContainsKey(key);
        }

        public string GetEncodedHeader()
        {
            string header = JsonConvert.SerializeObject(_header);
            return Base64Encode(header);
        }

        public string GetEncodedPayload()
        {
            string payload = JsonConvert.SerializeObject(_payload);
            return Base64Encode(payload);
        }

        public string GetUnsignedEncodedToken()
        {
            return GetEncodedHeader() + "." + GetEncodedPayload();
        }

        private void EnsureTokenIsNotSignedAndLocked()
        {
            if (_signedAndlocked)
                throw new TokenIsSignedAndLockedException();
        }
    }
}
