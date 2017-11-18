using System;
using Lynx.Core.Mappers.IDSubsystem.Strategies;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.Models.Interactions;
using Lynx.Core.PeerVerification;
using Lynx.Core.PeerVerification.Interfaces;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;

namespace Lynx.Core.ViewModels
{
    public class VerificationRequestViewModel : MvxViewModel
    {
        private Requester _requester;
        private IMapper<ID> _idMapper;
        private ID _id;
        public string Syn
        {
            get
            {
                return _requester.CreateEncodedSyn();
            }
        }
        public MvxInteraction<BooleanInteraction> ConfirmationInteraction { get; set; }

        public VerificationRequestViewModel(Requester requester, IMapper<ID> idMapper, ID id)
        {
            _requester = requester;
            _idMapper = idMapper;
            _id = id;
        }

        //TODO: For more information see: https://www.mvvmcross.com/documentation/fundamentals/navigation
        public void Init()
        {
            ConfirmationInteraction = new MvxInteraction<BooleanInteraction>();
            _requester.HandshakeComplete += async (sender, e) =>
            {
                await _idMapper.SaveAsync(_id);
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

                Query = "Your ID was sucessfully certified!"
            };

            ConfirmationInteraction.Raise(confirmationRequest);
        }
    }
}
