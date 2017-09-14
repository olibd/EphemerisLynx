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
    public class CertifyViewModel : MvxViewModel<ID>
    {
		public ID ID { get; set; }
		public List<CheckedAttribute> Attributes { get; set; }
        public SynAck _synAck;
        public List<string> certifiedAttributes;
		private IMvxNavigationService _navigationService;

		public class CheckedAttribute
		{
			private CertifyViewModel _certifyViewModel;
			public string Description { get; set; }
			private bool _isChecked;

			public bool IsChecked
			{
				get { return _isChecked; }
				set
				{
					_isChecked = value;
					_certifyViewModel.UpdateCertifiedAttributes(Description);
				}
			}

			public CheckedAttribute(CertifyViewModel certifyViewModel, bool check, string attrDescription)
			{
				_certifyViewModel = certifyViewModel;
				_isChecked = check;
				Description = attrDescription;
			}
		}

		public CertifyViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

		public override Task Initialize(ID Id)
		{
            ID = Id;
            Attributes = new List<CheckedAttribute>();
            foreach (Attribute attr in ID.Attributes.Values)
            {
                Attributes.Add(new CheckedAttribute(this, false, attr.Description));
            }
            certifiedAttributes = new List<string>(); 
            return base.Initialize();
		}

		public override void Start()
		{
			//TODO: Add starting logic here
		}

		private void UpdateCertifiedAttributes(string Description)
		{
			if (certifiedAttributes.Contains(Description))
			{
				certifiedAttributes.Remove(Description);
			}
			else
			{
				certifiedAttributes.Add(Description);
			}
		}

        public IMvxCommand CertifyIDCommand => new MvxCommand(CertifyID);

		private void CertifyID()
		{
			throw new NotImplementedException();
		}
    }
}
