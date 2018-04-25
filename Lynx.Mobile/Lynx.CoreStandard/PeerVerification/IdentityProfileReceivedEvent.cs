using System;
using Lynx.Core.Communications.Packets;
using Lynx.Core.Communications.Packets.Interfaces;

namespace Lynx.Core.PeerVerification
{
    public class IdentityProfileReceivedEvent : EventArgs
    {
        public ISynAck SynAck { get; set; }
    }
}
