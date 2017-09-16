using System;
using System.Collections.Generic;
using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using Newtonsoft.Json;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace Lynx.Core.Communications.Packets
{
    public abstract class HandshakeToken : Token, IHandshakeToken
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

        private Attribute[] _accessibleAttributes;
        public Attribute[] AccessibleAttributes
        {
            get
            {
                return _accessibleAttributes;
            }

            set
            {
                if (value != null)
                {
                    string[] attrDescriptionArr = new string[value.Length];

                    for (int i = 0; i < value.Length; i++)
                    {
                        attrDescriptionArr[i] = value[i].Description;
                    }

                    SetOnPayload("accAttr", JsonConvert.SerializeObject(attrDescriptionArr));
                    _accessibleAttributes = value;
                }
                else
                {
                    RemoveFromPayload("accAttr");
                    _accessibleAttributes = null;
                }
            }
        }

        protected HandshakeToken() : base()
        {

        }

        protected HandshakeToken(Dictionary<string, string> header, Dictionary<string, string> payload, ID id) : base(header, payload)
        {
            Id = id;
            id.Attributes.Values.CopyTo(_accessibleAttributes, 0);
        }

        protected HandshakeToken(Dictionary<string, string> header, Dictionary<string, string> payload) : base(ClearIdAddrFromHeader(header), payload)
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
