using System;
using System.Collections.Generic;
using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using Newtonsoft.Json;

namespace Lynx.Core.Communications.Packets
{
	public class InfoRequestSynAck : HandshakeToken
	{
		protected override string Type
		{
			get
			{
				return "inforeqsynack";
			}
		}

		public InfoRequestSynAck() : base()
		{
		}

		public InfoRequestSynAck(Dictionary<string, string> header, Dictionary<string, string> payload, ID id) : base(header, payload, id)
		{
		}

		public InfoRequestSynAck(Dictionary<string, string> header, Dictionary<string, string> payload) : base(header, payload)
		{
		}

        private string[] _requestedAttributes;
		public string[] RequestedAttributes
		{
			get
			{
				return _requestedAttributes;
			}

			set
			{
				if (value != null)
				{
					SetOnPayload("reqAttr", JsonConvert.SerializeObject(_requestedAttributes));
					_requestedAttributes = value;
				}
				else
				{
					RemoveFromPayload("reqAttr");
					_requestedAttributes = null;
				}
			}
		}
	}
}
