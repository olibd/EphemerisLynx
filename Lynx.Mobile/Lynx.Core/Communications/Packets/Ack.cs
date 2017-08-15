using System.Collections.Generic;
using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using Newtonsoft.Json;

namespace Lynx.Core.Communications.Packets
{
    public class Ack : HandshakeToken, IAck
    {

        public Ack() : base()
        {

        }

        protected Ack(Dictionary<string, string> header, Dictionary<string, string> payload, ID id) : base(header, payload, id)
        {
        }

        protected Ack(Dictionary<string, string> header, Dictionary<string, string> payload) : base(header, payload)
        {
        }
    }
}
