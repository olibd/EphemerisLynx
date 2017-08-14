using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using Newtonsoft.Json;

namespace Lynx.Core.Communications.Packets
{
    public class Ack : HandshakeToken, IAck
    {
        private Attribute[] _accessibleAttributes;
        public Attribute[] AccessibleAttributes
        {
            get
            {
                return _accessibleAttributes;
            }

            set
            {
                string[] attrDescriptionArr = new string[value.Length];

                for (int i = 0; i < value.Length; i++)
                {
                    attrDescriptionArr[i] = value[i].Description;
                }

                SetOnPayload("accAttr", JsonConvert.SerializeObject(attrDescriptionArr));
                _accessibleAttributes = value;
            }
        }

        public Ack() : base()
        {

        }

        public Ack(string token) : base(token)
        {

        }
    }
}
