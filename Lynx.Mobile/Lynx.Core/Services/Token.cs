using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Lynx.Core.Services.Interfaces;
using Newtonsoft.Json;

namespace Lynx.Core.Services
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
            set
            {
                Contract.Ensures(!_signedAndlocked);
                _signedAndlocked = true;
                _signature = value;
            }
        }
        private Dictionary<string, string> _header;
        private Dictionary<string, string> _payload;

        public string GetEncodedToken()
        {
            return GetUnsignedEncodedToken() + Signature != null || Signature.Equals("") ? "." + Signature : "";
        }

        private string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
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
