using System;
using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.Communications.Packets
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
                SetOnPayload("idAddr", value.Address);
                _id = value;
            }
        }

        public string PublicKey
        {
            get
            {
                return GetFromHeader("pubkey");
            }

            set
            {
                SetOnHeader("pubkey", value);
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

        protected HandshakeToken() : base()
        {

        }

        protected HandshakeToken(string token) : base(token)
        {
            //TODO: Handle encrypted tokens
        }
    }
}
