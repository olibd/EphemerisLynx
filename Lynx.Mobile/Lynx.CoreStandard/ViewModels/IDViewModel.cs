using System.Linq;
using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;
using MvvmCross.Core.Navigation;
using System.Threading.Tasks;
using Lynx.Core.PeerVerification;
using Lynx.Core.Communications.Packets;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Interactions;
using Lynx.Core.Interfaces;
using Lynx.Core.Mappers.IDSubsystem.Strategies;
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
        public IMvxCommand AttributeSelectedCommand => new MvxCommand<Attribute>(ViewCertificates);

        //The IMvxInteraction and the MvxInteraction must be separate because the Raise() method is not part if the IMvxInteraction interface.
        public IMvxInteraction<UserFacingErrorInteraction> DisplayErrorInteraction => _displayErrorInteraction;

        private IReceiver _receiver;
        private bool _scanned = false;
        private readonly MvxInteraction<UserFacingErrorInteraction> _displayErrorInteraction = new MvxInteraction<UserFacingErrorInteraction>();

        public IDViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        //TODO: For more information see: https://www.mvvmcross.com/documentation/fundamentals/navigation
        public async void Init()
        {
            ID = Mvx.Resolve<ID>();
            Attributes = ID.Attributes.Values.ToList();
            Attributes.Remove(ID.Attributes["firstname"]);
            Attributes.Remove(ID.Attributes["lastname"]);

            Fullname = ID.Attributes["firstname"].Content.ToString() + " " + ID.Attributes["lastname"].Content.ToString();
            Fullname = Fullname.ToUpperInvariant();
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
            _receiver = Mvx.Resolve<IReceiver>();

            //Error callback - necessary to handle exceptions occuring in code that is called by the Session
            _receiver.OnError += (sender, e) =>
            {
                _displayErrorInteraction.Raise(new UserFacingErrorInteraction()
                {
                    Exception = e.Exception,
                    Callback = () => _scanned = false
                });
            };

            //TODO: Setup the verifier callback
            _receiver.IdentityProfileReceived += async (sender, e) =>
            {
                _scanned = false;
                await _navigationService.Navigate<CertifyViewModel, IReceiver>((IReceiver)sender);
            };

            _receiver.InfoRequestReceived += async (sender, e) =>
            {
                _scanned = false;
                await _navigationService.Navigate<InfoRequestViewModel, IReceiver>((IReceiver)sender);
            };

            try
            {
                await _receiver.ProcessSyn(content);
            }
            catch (UserFacingException e)
            {
                _displayErrorInteraction.Raise(new UserFacingErrorInteraction()
                {
                    Exception = e,
                    Callback = () => _scanned = false
                });
            }

        }

        private async Task FetchID()
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
