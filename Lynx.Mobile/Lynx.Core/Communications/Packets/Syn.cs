using System.Collections.Generic;
using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.Communications.Packets
{
    public class Syn : HandshakeToken, ISyn
    {
        protected override string Type
        {
            get
            {
                return "syn";
            }
        }

        public Syn() : base()
        {
        }

        public Syn(Dictionary<string, string> header, Dictionary<string, string> payload, ID id) : base(header, payload, id)
        {
        }

        public Syn(Dictionary<string, string> header, Dictionary<string, string> payload) : base(header, payload)
        {
        }

        public string NetworkAddress
        {
            get
            {
                return GetFromPayload("netAddr");
            }

            set
            {
                if (value != null)
                {
                    SetOnPayload("netAddr", value);
                }
                else
                {
                    RemoveFromPayload("netAddr");
                }
            }
        }
    }
}
