using System;
using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.PeerVerification
{
    public class IssuedCertificatesAddedToIDEvent : EventArgs
    {
        public List<Certificate> CertificatesAdded { get; set; }
    }
}