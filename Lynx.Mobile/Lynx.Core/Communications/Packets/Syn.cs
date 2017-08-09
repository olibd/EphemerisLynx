using Lynx.Core.Communications.Packets.Interfaces;

namespace Lynx.Core.Communications.Packets
{
    public class Syn : HandshakeToken, ISyn
    {
        public Syn() : base()
        {

        }

        public Syn(string token) : base(token)
        {

        }

        public string NetworkAddress
        {
            get
            {
                return GetFromPayload("netAddr");
            }

            set
            {
                SetOnPayload("netAddr", value);
            }
        }
    }
}
