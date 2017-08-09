using System;
using Lynx.Core.PeerVerification.Interfaces;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;

namespace Lynx.Core.ViewModels
{
    public class VerificationRequestViewModel : MvxViewModel
    {
        private IRequesterService _requester;
        public string Syn
        {
            get
            {
                return _requester.CreateEncodedSyn();
            }
        }
        public VerificationRequestViewModel(IRequesterService requester)
        {
            _requester = requester;
        }
    }
}
