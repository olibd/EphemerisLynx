using System;
using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.Services.Interfaces;
using Newtonsoft.Json;
using System.Diagnostics.Contracts;

namespace Lynx.Core.Services
{
    public abstract class HandshakeToken : Token, IHandshakeToken
    {
        private ID _id;
        public ID Id
        {
            get
            {
                return _id;
            }

            set
            {
                Payload["idAddr"] = value.Address;
                _id = value;
            }
        }

        public string PublicKey
        {
            get
            {
                return Header["pubkey"];
            }

            set
            {
                Header["pubkey"] = value;
            }
        }

        public bool Encrypted
        {
            get
            {
                return Boolean.Parse(Header["pubkey"]);
            }

            set
            {
                Header["pubkey"] = value.ToString();
            }
        }
    }
}
