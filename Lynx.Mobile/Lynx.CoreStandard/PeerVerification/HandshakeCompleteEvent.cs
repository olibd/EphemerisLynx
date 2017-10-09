using System;
using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.PeerVerification
{
    public class HandshakeCompleteEvent : EventArgs
    {
        public ID Id { get; set; }
    }
}
