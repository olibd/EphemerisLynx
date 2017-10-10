using System;
using Lynx.Core.Communications.Packets;
using Lynx.Core.Communications.Packets.Interfaces;

namespace Lynx.Core.PeerVerification
{
    public class InfoRequestReceivedEvent : EventArgs
    {
        public InfoRequestSynAck InfoRequestSynAck { get; set; }
    }
}
