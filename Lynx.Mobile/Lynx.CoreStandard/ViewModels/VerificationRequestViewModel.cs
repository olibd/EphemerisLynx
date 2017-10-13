using System;
using Lynx.Core.Models.Interactions;
using Lynx.Core.PeerVerification.Interfaces;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;

namespace Lynx.Core.ViewModels
{
    public class VerificationRequestViewModel : MvxViewModel
    {
        private IRequester _requester;
        public string Syn
        {
            get
            {
                return _requester.CreateEncodedSyn();
            }
        }
        public MvxInteraction<BooleanInteraction> ConfirmationInteraction { get; set; }

        public VerificationRequestViewModel(IRequester requester)
        {
            _requester = requester;
        }

        //TODO: For more information see: https://www.mvvmcross.com/documentation/fundamentals/navigation
        public void Init()
        {
            ConfirmationInteraction = new MvxInteraction<BooleanInteraction>();
            _requester.HandshakeComplete += (sender, e) =>
            {
                DeployConfirm();
            };
        }

        /// <summary>
        /// Alert user that their ID has been verified
        /// </summary>
        private void DeployConfirm()
        {
            BooleanInteraction confirmationRequest = new BooleanInteraction
            {
                Callback = (bool ok) =>
                {
                    Close((this));
                },

                Query = "Your ID was certified by " + _requester.Ack.Id.Attributes["firstname"].Content.ToString()
            };

            ConfirmationInteraction.Raise(confirmationRequest);
        }
    }
}
