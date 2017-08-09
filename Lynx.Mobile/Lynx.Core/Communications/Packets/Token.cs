using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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

        protected Token(string encodedToken)
        {
            string[] splittedEncodedToken = encodedToken.Split('.');
            string jsonDecodedHeader = Base64Decode(splittedEncodedToken[0]);
            string jsonDecodedPayload = Base64Decode(splittedEncodedToken[1]);
            //if the token is signed, restore the signature
            if (splittedEncodedToken.Length == 3)
            {
                string sig = splittedEncodedToken[2];
                _signature = sig;
            }

            _header = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedHeader);
            _payload = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedPayload);
        }

        public void SignAndLock(string signature)
        {
            Contract.Ensures(!_signedAndlocked);
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
            Contract.Ensures(!_signedAndlocked);
            _header.Add(key, val);
        }

        public void SetOnPayload(string key, string val)
        {
            Contract.Ensures(!_signedAndlocked);
            _payload.Add(key, val);
        }

        public string GetFromHeader(string key)
        {
            return _header[key];
        }

        public string GetFromPayload(string key)
        {
            return _payload[key];
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
    }
}
