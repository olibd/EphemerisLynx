using System;
using System.Collections.Generic;
using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.Communications.Packets
{
    public class InfoRequestSynAck : InfoRequestToken, IInfoRequestSynAck
    {
		public InfoRequestSynAck() : base()
        {
		}

		public InfoRequestSynAck(Dictionary<string, string> header, Dictionary<string, string> payload, ID id) : base(header, payload, id)
        {
		}

		public InfoRequestSynAck(Dictionary<string, string> header, Dictionary<string, string> payload) : base(header, payload)
        {
		}
    }
}
