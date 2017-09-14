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
		public List<CheckedAttribute> Attributes { get; set; }
		public List<string> _attributesToCertify;
		private IMvxNavigationService _navigationService;
		private IVerifier _verifier;

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

		public override Task Initialize(IVerifier verifier)
		{
			_verifier = verifier;
			ID = _verifier.SynAck.Id;
			Attributes = new List<CheckedAttribute>();
			foreach (Attribute attr in ID.Attributes.Values)
			{
				Attributes.Add(new CheckedAttribute(this, false, attr.Description));
			}
			_attributesToCertify = new List<string>();
			return base.Initialize();
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

		public IMvxCommand CertifyIDCommand => new MvxCommand(CertifyID);

		private void CertifyID()
		{
			_verifier.Certify(_attributesToCertify.ToArray());
		}
	}
}