using System;
using System.Collections.Generic;
using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using Newtonsoft.Json;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace Lynx.Core.Communications.Packets
{
	public abstract class InfoRequestToken : Token, IInfoRequestToken
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
				if (value != null)
				{
					SetOnPayload("idAddr", value.Address);
					_id = value;
				}
				else
				{
					RemoveFromPayload("idAddr");
					_id = null;
				}
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

		protected InfoRequestToken() : base()
		{

		}

		protected InfoRequestToken(Dictionary<string, string> header, Dictionary<string, string> payload, ID id) : base(header, payload)
		{
			Id = id;
		}

		protected InfoRequestToken(Dictionary<string, string> header, Dictionary<string, string> payload) : base(ClearIdAddrFromHeader(header), payload)
		{
		}

		/// <summary>
		/// Clears the idAddr from header. This method should only be called
		/// by constructors that are not receiving an ID as a parameter.
		/// Since no id object is supplied, we need to make sure that 
		/// if the header contains the idAddr key that it is not 
		/// present in the dictionary passed to the parent
		/// </summary>
		private static Dictionary<string, string> ClearIdAddrFromHeader(Dictionary<string, string> header)
		{
			if (header.ContainsKey("idAddr"))
				header.Remove("idAddr");

			return header;
		}
	}
}
