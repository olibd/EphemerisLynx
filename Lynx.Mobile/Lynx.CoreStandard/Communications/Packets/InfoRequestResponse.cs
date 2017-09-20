using System;
using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.Communications.Packets
{
    public class InfoRequestResponse : HandshakeToken
    {
		protected override string Type
		{
			get
			{
				return "inforeqresp";
			}
		}

		public InfoRequestResponse() : base()
        {
		}

		public InfoRequestResponse(Dictionary<string, string> header, Dictionary<string, string> payload, ID id) : base(header, payload, id)
        {
		}

		public InfoRequestResponse(Dictionary<string, string> header, Dictionary<string, string> payload) : base(header, payload)
        {
		}
    }
}
