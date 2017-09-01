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
        public SynAck _synAck;
        public List<string> attributesToCertify;
        private IMvxNavigationService _navigationService;
        private IVerifier _verifier;

        public CertifyViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override Task Initialize(IVerifier verifier)
        {
            _verifier = verifier;
            ID = _verifier.SynAck.Id;
            Attributes = ID.Attributes.Values.ToList();
            return base.Initialize();
        }

        public override void Start()
        {
            //TODO: Add starting logic here
        }

        public void UpdateCertifiedAttributes()
        {

        }

        public IMvxCommand UpdateCertifiedAttributesCommand => new MvxCommand<string>(UpdateCertifiedAttributes);

        public void UpdateCertifiedAttributes(string attributeDescription)
        {
            if (attributesToCertify.Contains(attributeDescription))
            {
                attributesToCertify.Remove(attributeDescription);
            }
            else
            {
                attributesToCertify.Add(attributeDescription);
            }
        }

        public IMvxCommand CertifyIDCommand => new MvxCommand(CertifyID);

        private void CertifyID()
        {
            _verifier.Certify(attributesToCertify.ToArray());
        }
    }
}
