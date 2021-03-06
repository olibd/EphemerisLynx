using System.Linq;
using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;
using MvvmCross.Core.Navigation;
using Lynx.Core.PeerVerification.Interfaces;

namespace Lynx.Core.ViewModels
{
    public class IDViewModel : MvxViewModel
    {
        public ID ID { get; set; }
        public List<Attribute> Attributes { get; set; }
        public string Fullname { get; set; }
        private IMvxNavigationService _navigationService;

        public IMvxCommand RequestVerificationCommand => new MvxCommand(RequestVerification);
        public IMvxCommand QrCodeScanCommand => new MvxCommand<string>(QrCodeScan);
        public IMvxCommand UpdateID => new MvxCommand(FetchID);
        public IMvxCommand AttributeSelectedCommand => new MvxCommand<Attribute>(ViewCertificates);

        private IReceiver _verifier;
        private bool _scanned = false;

        public IDViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        //TODO: For more information see: https://www.mvvmcross.com/documentation/fundamentals/navigation
        public void Init()
        {
            ID = Mvx.Resolve<ID>();
            Attributes = ID.Attributes.Values.ToList();
            Attributes.Remove(ID.Attributes["firstname"]);
            Attributes.Remove(ID.Attributes["lastname"]);

            Fullname = ID.Attributes["firstname"].Content.ToString() + " " + ID.Attributes["lastname"].Content.ToString();
            Fullname = Fullname.ToUpperInvariant();
            UpdateID.Execute();
        }

        public override void Start()
        {
            //TODO: Add starting logic here
        }

        private async void QrCodeScan(string content)
        {
            if (_scanned)
                return;

            _scanned = true;
            _verifier = Mvx.Resolve<IReceiver>();

            //TODO: Setup the verifier callback
            await _verifier.ProcessSyn(content);

            _verifier.IdentityProfileReceived += async (sender, e) =>
            {
                _scanned = false;
                await _navigationService.Navigate<CertifyViewModel, IReceiver>((IReceiver)sender);
            };

            _verifier.InfoRequestReceived += async (sender, e) =>
            {
                _scanned = false;
                await _navigationService.Navigate<InfoRequestViewModel, IReceiver>((IReceiver)sender);
            };
        }

        private async void FetchID()
        {
            //ID = await Mvx.Resolve<IIDFacade>().GetIDAsync(ID.Address);

            //TODO: Fix crash when saving with the mappers - requires a bit of time to make this work properly, and non-critical
            //This will need to be fixed by the time we have certificate revocation

            //int UID = await Mvx.Resolve<IMapper<ID>>().SaveAsync(ID);
            //Mvx.Resolve<IPlatformSpecificDataService>().IDUID = UID;

            //Attributes = ID.Attributes.Values.ToList();
            //RaisePropertyChanged(() => Attributes);
            //TODO: Tell the user when this is done
        }

        public async void ViewCertificates(Attribute attrDTO)
        {
            await _navigationService.Navigate<CertificatesViewModel, Attribute>(ID.Attributes[attrDTO.Description]);
        }

        private async void RequestVerification()
        {
            await _navigationService.Navigate<VerificationRequestViewModel>();
        }
    }
}
