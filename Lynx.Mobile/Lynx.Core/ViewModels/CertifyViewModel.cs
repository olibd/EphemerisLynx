using System;
using System.Linq;
using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;
using MvvmCross.Core.ViewModels;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;
using MvvmCross.Core.Navigation;
using System.Threading.Tasks;
using Lynx.Core.Communications.Packets;
using Lynx.Core.PeerVerification.Interfaces;

namespace Lynx.Core.ViewModels
{
    public class CertifyViewModel : MvxViewModel<IVerifier>
    {
        public ID ID { get; set; }
        public List<Attribute> Attributes { get; set; }
        private List<string> _attributesToCertify;
        private IMvxNavigationService _navigationService;
        private IVerifier _verifier;

        public CertifyViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
            _attributesToCertify = new List<string>();
        }

        public override Task Initialize(IVerifier verifier)
        {
            _verifier = verifier;
            ID = _verifier.SynAck.Id;
            Attributes = ID.Attributes.Values.ToList();
            return Task.FromResult(true);
        }
        public IMvxCommand UpdateCertifiedAttributesCommand => new MvxCommand<string>(UpdateCertifiedAttributes);

        public void UpdateCertifiedAttributes(string attributeDescription)
        {
            if (_attributesToCertify.Contains(attributeDescription))
            {
                _attributesToCertify.Remove(attributeDescription);
            }
            else
            {
                _attributesToCertify.Add(attributeDescription);
            }
        }

        public IMvxCommand CertifyIDCommand => new MvxCommand(CertifyID);

        private void CertifyID()
        {
            _verifier.Certify(attributesToCertify.ToArray());
            _verifier.CertificatesSent += (sender, e) => { Close((this)); };
        }
    }
}
