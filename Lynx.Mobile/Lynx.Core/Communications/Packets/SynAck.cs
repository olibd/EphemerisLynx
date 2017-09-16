using System;
using System.Collections.Generic;
using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.Communications.Packets
{
    public class SynAck : HandshakeToken, ISynAck
    {
        protected override string Type
        {
            get
            {
                return "synack";
            }
        }

        public SynAck() : base()
        {
        }

        public SynAck(Dictionary<string, string> header, Dictionary<string, string> payload, ID id) : base(header, payload, id)
        {
        }

        public SynAck(Dictionary<string, string> header, Dictionary<string, string> payload) : base(header, payload)
        {
        }
    }
}
