using System;
using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;
using MvvmCross.Core.ViewModels;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;
using MvvmCross.Core.Navigation;
using Lynx.Core.PeerVerification.Interfaces;
using Lynx.Core.Interactions;
using System.Threading.Tasks;

namespace Lynx.Core.ViewModels
{
    public class CertifyViewModel : MvxViewModel<IReceiver>
    {
        public ID ID { get; set; }
        public List<CheckedAttribute> Attributes { get; set; }
        private List<string> _attributesToCertify;
        private IMvxNavigationService _navigationService;
        private IReceiver _verifier;

        public IMvxInteraction<UserFacingErrorInteraction> DisplayErrorInteraction => _displayErrorInteraction;
        //We specify the instance separately because IMvxInteraction does not
        //offer the Raise() method, only MvxInteraction does
        private readonly MvxInteraction<UserFacingErrorInteraction> _displayErrorInteraction = new MvxInteraction<UserFacingErrorInteraction>();

        public class CheckedAttribute
        {
            private CertifyViewModel _certifyViewModel;
            public string Description { get { return attr.Description; } }
            public IContent Content { get { return attr.Content; } }
            private bool _isChecked;
            private Attribute attr;

            public bool IsChecked
            {
                get { return _isChecked; }
                set
                {
                    _isChecked = value;
                    _certifyViewModel.UpdateCertifiedAttributes(Description);
                }
            }

            public CheckedAttribute(CertifyViewModel certifyViewModel, bool check, Attribute attribute)
            {
                _certifyViewModel = certifyViewModel;
                _isChecked = check;
                attr = attribute;
            }
        }

        public CertifyViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override void Start()
        {
            //TODO: Add starting logic here
        }

        private void UpdateCertifiedAttributes(string attributeDescription)
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

        public IMvxCommand CertifyIDCommand => new MvxAsyncCommand(CertifyID);

        private async Task CertifyID()
        {
            try
            {
                await _verifier.Certify(_attributesToCertify.ToArray());
                Close((this));
            }
            catch (UserFacingException e)
            {
                _displayErrorInteraction.Raise(new UserFacingErrorInteraction(e));

            }
        }

        public override void Prepare(IReceiver verifier)
        {
            _verifier = verifier;
            ID = _verifier.SynAck.Id;
            Attributes = new List<CheckedAttribute>();
            foreach (Attribute attr in ID.Attributes.Values)
            {
                Attributes.Add(new CheckedAttribute(this, false, attr));
            }
            _attributesToCertify = new List<string>();
        }
    }
}
