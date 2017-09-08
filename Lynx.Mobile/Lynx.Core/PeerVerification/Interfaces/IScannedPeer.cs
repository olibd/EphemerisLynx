using System;
using Lynx.Core.Communications.Packets;

namespace Lynx.Core.PeerVerification.Interfaces
{
    public interface IScannedPeer
    {
        void GenerateAndSendSynAck(Ack ack);
    }
}
