using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using Newtonsoft.Json;

namespace Lynx.Core.Communications.Packets
{
    public class Ack : HandshakeToken, IAck
    {
        private IContent _name;
        public IContent Name
        {
            get
            {
                return _name;
            }

            set
            {
                SetOnPayload("name", JsonConvert.SerializeObject(value));
                _name = value;
            }
        }
    }
}
