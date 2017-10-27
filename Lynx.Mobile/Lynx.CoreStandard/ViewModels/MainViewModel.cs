using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynx.Core.Crypto;
using Lynx.Core.Facade;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Interfaces;
using Lynx.Core.Mappers.IDSubsystem.Strategies;
using Lynx.Core.Models.IDSubsystem;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Core.Navigation;
using Plugin.Fingerprint;
using NBitcoin;

namespace Lynx.Core.ViewModels
{
    public class MainViewModel : MvxViewModel
    {
        private IMvxNavigationService _navigationService;
        private IMvxCommand _checkAndLoadID => new MvxCommand(CheckAndLoadID);
        public IMvxCommand RegisterClickCommand => new MvxCommand(NavigateRegistration);
        public IMvxCommand FingerprintLoginCommand => new MvxCommand(FingerprintLoginAsync);

        private string _mnemonicPhrase;

        public string MnemonicPhrase
        {
            get => _mnemonicPhrase;
        }

        private string _fingerprintAuthenticationMessage;

        public string FingerprintAuthenticationMessage
        {
            get => _fingerprintAuthenticationMessage;
        }

        public MainViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override void Start()
        {
            base.Start();
            LoadKeys();
            _checkAndLoadID.Execute();
        }
        private async void CheckAndLoadID()
        {
            IPlatformSpecificDataService dataService = Mvx.Resolve<IPlatformSpecificDataService>();
            if (dataService.IDUID != -1)
            {
                //TODO: Load from database, and fallback to loading from blockchain in case of exception
                IIDFacade idFacade = Mvx.Resolve<IIDFacade>();

                //ID id = await idFacade.GetIDAsync(dataService.IDAddress);
                ID id = await Mvx.Resolve<IMapper<ID>>().GetAsync(dataService.IDUID);
                Mvx.RegisterSingleton(() => id);
                FingerprintLoginCommand.Execute();
            }
        }

        private void LoadKeys()
        {
            IPlatformSpecificDataService dataService = Mvx.Resolve<IPlatformSpecificDataService>();
            IAccountService accountService = dataService.LoadAccount();

            if (accountService == null)
            {
                //We do not have any keys yet

                accountService = new AccountService();
                dataService.SaveAccount(accountService);

                _mnemonicPhrase = "Mnemonic phrase: " + accountService.MnemonicPhrase;
            }
            else
            {
                _mnemonicPhrase = "Loading existing keys, no seed phrase generated";
            }

            RaisePropertyChanged(() => MnemonicPhrase);

            Mvx.RegisterType(() => accountService);
        }

        private async void NavigateRegistration()
        {
            //TODO: Generate private key and save into file
            //TODO: Encrypt file, store key in keystore, fingerprint locked
            await _navigationService.Navigate<RegistrationViewModel>();
        }

        private async void FingerprintLoginAsync()
        {
            var result = await CrossFingerprint.Current.IsAvailableAsync(true);
            if (result)
            {
                var auth = await CrossFingerprint.Current.AuthenticateAsync("Scan fingerprint to access your ID.");
                if (auth.Authenticated)
                {
                    await _navigationService.Navigate<IDViewModel>();
                }
                else
                {
                    _fingerprintAuthenticationMessage = "Fingerprint Authentication Failed!";
                    RaisePropertyChanged(() => FingerprintAuthenticationMessage);
                }
            }
            else
            {
                _fingerprintAuthenticationMessage = "A fingerprint needs to be registered on the phone.";
                RaisePropertyChanged(() => FingerprintAuthenticationMessage);
            }
        }

    }
}
