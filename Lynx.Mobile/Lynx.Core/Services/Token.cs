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

        public string GetEncodedToken
        {
            get
            {
                string header = JsonConvert.SerializeObject(Header);
                header = Base64Encode(header);
                string payload = JsonConvert.SerializeObject(Payload);
                payload = Base64Encode(payload);

                return header + "." + payload + Signature != null || Signature.Equals("") ? "." + Signature : "";
            }
        }

        private Dictionary<string, string> _header;
        public Dictionary<string, string> Header
        {
            get
            {
                return _header;
            }
            set
            {
                Contract.Ensures(!_signedAndlocked);
                _header = value;
            }
        }

        private Dictionary<string, string> _payload;
        public Dictionary<string, string> Payload
        {
            get
            {
                return _payload;
            }
            set
            {
                Contract.Ensures(!_signedAndlocked);
                _payload = value;
            }
        }

        private string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
