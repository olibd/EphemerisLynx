using Lynx.Core.Services.Communications.Packets.Interfaces;

namespace Lynx.Core.Services.Communications.Packets
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
