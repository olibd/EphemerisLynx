using System;
using System.Collections.Generic;
using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using Newtonsoft.Json;

namespace Lynx.Core.Communications.Packets
{
    public class InfoRequestSynAck : SynAck
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

        public string[] RequestedAttributes
        {
            get
            {
                if (PayloadContains("reqAttr"))
                    return JsonConvert.DeserializeObject<string[]>(GetFromPayload("reqAttr"));
                else
                    return null;
            }

            set
            {
                if (value != null)
                {
                    SetOnPayload("reqAttr", JsonConvert.SerializeObject(value));
                }
                else
                {
                    RemoveFromPayload("reqAttr");
                }
            }
        }
    }
}
