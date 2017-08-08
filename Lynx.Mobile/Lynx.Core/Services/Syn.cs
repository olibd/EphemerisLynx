using Lynx.Core.Services.Interfaces;

namespace Lynx.Core.Services
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
