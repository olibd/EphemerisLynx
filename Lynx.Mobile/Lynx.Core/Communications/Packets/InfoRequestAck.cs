using System;
using Lynx.Core.Communications.Packets.Interfaces;
using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.Communications.Packets
{
    public class InfoRequestAck : InfoRequestToken, IInfoRequestAck 
    {
        public InfoRequestAck()
        {
        }

		public InfoRequestAck(Dictionary<string, string> header, Dictionary<string, string> payload, ID id) : base(header, payload, id)
        {
		}

		public InfoRequestAck(Dictionary<string, string> header, Dictionary<string, string> payload) : base(header, payload)
        {
		}
    }
}
