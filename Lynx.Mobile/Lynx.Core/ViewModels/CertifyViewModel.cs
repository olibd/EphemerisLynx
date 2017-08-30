using System;
using System.Linq;
using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;
using MvvmCross.Core.ViewModels;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;
using MvvmCross.Core.Navigation;
using System.Threading.Tasks;
using Lynx.Core.Communications.Packets;

namespace Lynx.Core.ViewModels
{
    public class CertifyViewModel : MvxViewModel<SynAck>
    {
		public ID ID { get; set; }
		public List<Attribute> Attributes { get; set; }
        public SynAck _synAck;
		private IMvxNavigationService _navigationService;

		public CertifyViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

		public override Task Initialize(SynAck synAck)
		{
            _synAck = synAck;
            ID = synAck.Id;
            Attributes = ID.Attributes.Values.ToList();
		
            return base.Initialize();
		}

		public override void Start()
		{
			//TODO: Add starting logic here
		}

        public IMvxCommand CertifyIDCommand => new MvxCommand(CertifyID);

		private void CertifyID()
		{
			throw new NotImplementedException();
		}
    }
}
