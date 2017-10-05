using System;
using System.Collections.Generic;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Navigation;
using Lynx.Core.Communications.Packets;

namespace Lynx.Core.ViewModels
{
    public class InfoRequestViewModel : MvxViewModel<InfoRequestSynAck>
    {

		private IMvxNavigationService _navigationService;
        public List<string> requestedAttributes { get; set; }

		public InfoRequestViewModel(IMvxNavigationService navigationService)
		{
			_navigationService = navigationService;
		}

		public override void Start()
		{
			//TODO: Add starting logic here
		}

        public override void Prepare(InfoRequestSynAck infoRequestSynAck)
        {
            requestedAttributes = new List<string>(infoRequestSynAck.RequestedAttributes);
        }
    }
}
