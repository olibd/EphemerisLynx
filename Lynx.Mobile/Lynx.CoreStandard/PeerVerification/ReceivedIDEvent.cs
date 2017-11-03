using System;
using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.PeerVerification
{
    public class ReceivedIDEvent : EventArgs
    {
        public ID ReceivedID { get; set; }
    }
}
