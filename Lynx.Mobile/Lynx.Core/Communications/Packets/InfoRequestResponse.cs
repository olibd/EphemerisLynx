using System;
using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.Communications.Packets.Interfaces;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace Lynx.Core.Communications.Packets
{
    public class InfoRequestResponse : InfoRequestToken, IInfoRequestResponse
    {
        public InfoRequestResponse() : base()
        {
		}

		public InfoRequestResponse(Dictionary<string, string> header, Dictionary<string, string> payload, ID id) : base(header, payload, id)
        {
		}

		public InfoRequestResponse(Dictionary<string, string> header, Dictionary<string, string> payload) : base(header, payload)
        {
		}

		public Attribute[] RequestedAttributes { get; set; }

    }
}
