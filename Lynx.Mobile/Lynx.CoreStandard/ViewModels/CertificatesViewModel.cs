using System;
using System.Collections.Generic;
using System.Linq;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.PeerVerification.Interfaces;
using MvvmCross.Core.ViewModels;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace Lynx.Core.ViewModels
{
    public class CertificatesViewModel : MvxViewModel<Attribute>
    {
        public List<Certificate> Certificates { get; set; }
        private Attribute _attribute;

        public override void Prepare(Attribute parameter)
        {
            _attribute = parameter;
            Certificates = _attribute.Certificates.Values.ToList();
        }
    }
}
