using System;
using System.Collections.Generic;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Navigation;
using Lynx.Core.Communications.Packets;
using Lynx.Core.PeerVerification;
using Lynx.Core.PeerVerification.Interfaces;

namespace Lynx.Core.ViewModels
{
    public class InfoRequestViewModel : MvxViewModel<IReceiver>
    {

        private IMvxNavigationService _navigationService;
        private IReceiver _receiver;
        private InfoRequestSynAck _infoRequestSynAck;

        //a list of requested information by the API
        public List<string> _requestedAttributes { get; set; }

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
            _receiver.InfoRequestAuthorized += (sender, e) => { Close((this)); };
            _receiver.AuthorizeReadRequest(_infoRequestSynAck.RequestedAttributes);
        }

        public override void Prepare(IReceiver parameter)
        {
            _receiver = parameter;
            _infoRequestSynAck = parameter.InfoRequestSynAck;
            _requestedAttributes = new List<string>(_infoRequestSynAck.RequestedAttributes);
        }
    }
}
