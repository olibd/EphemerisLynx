using System;
using Lynx.Core.Communications.Packets;

namespace Lynx.Core.PeerVerification
{
    public class IdentityProfileReceivedEvent : EventArgs
    {
        public SynAck SynAck { get; set; }
    }
}
