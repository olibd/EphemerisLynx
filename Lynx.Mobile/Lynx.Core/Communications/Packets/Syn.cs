using System.Collections.Generic;
using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.Communications.Packets
{
    public class Syn : HandshakeToken, ISyn
    {
        public Syn() : base()
        {
        }

        protected Syn(Dictionary<string, string> header, Dictionary<string, string> payload, ID id) : base(header, payload, id)
        {
        }

        protected Syn(Dictionary<string, string> header, Dictionary<string, string> payload) : base(header, payload)
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
