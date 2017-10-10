using System;
using System.Collections.Generic;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Navigation;
using Lynx.Core.Communications.Packets;
using Lynx.Core.PeerVerification;

namespace Lynx.Core.ViewModels
{
    public class InfoRequestViewModel : MvxViewModel<Receiver>
    {

		private IMvxNavigationService _navigationService;
        private Receiver _receiver;
        private InfoRequestSynAck _infoRequestSynAck;
        public List<string> requestedAttributes { get; set; }

		public InfoRequestViewModel(IMvxNavigationService navigationService)
		{
			_navigationService = navigationService;
		}

		public override void Start()
		{
			//TODO: Add starting logic here
		}

		public IMvxCommand ProvideInfoCommand => new MvxCommand(ProvideInfo);

		private void ProvideInfo()
		{
			_receiver.GenerateAndSendResponse(_infoRequestSynAck);
		}

        public override void Prepare(Receiver receiver)
        {
            _receiver = receiver;
            _infoRequestSynAck = receiver.InfoRequestSynAck;
            requestedAttributes = new List<string>(_infoRequestSynAck.RequestedAttributes);
        }
    }
}
