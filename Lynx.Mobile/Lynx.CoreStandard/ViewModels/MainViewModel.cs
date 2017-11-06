using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using Plugin.Fingerprint.Abstractions;
using NBitcoin;
using Lynx.Core.Interactions;

namespace Lynx.Core.ViewModels
{
    public class MainViewModel : MvxViewModel
    {
        public IMvxCommand FingerprintLoginCommand => new MvxCommand(FingerprintLoginAsync);
        public IMvxCommand RegistrationButtonClick => new MvxCommand(NavigateRegistration);

        private readonly IMvxNavigationService _navigationService;
        private IAccountService _accountService;

        private string _fingerprintAuthenticationMessage;

        public string FingerprintAuthenticationMessage
        {
            get => _fingerprintAuthenticationMessage;
        }

        public MainViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override async void Start()
        {
            base.Start();

            try
            {
                SetupAccount();
                await CheckAndLoadID();
            }
            catch(NoAccountExistsException e)
            {
                await StartMnemonicValidation();
            }
        }

        private async Task CheckAndLoadID()
        {
            IPlatformSpecificDataService dataService = Mvx.Resolve<IPlatformSpecificDataService>();
            if (dataService.IDUID != -1)
            {
                //TODO: Load from database, and fallback to loading from blockchain in case of exception

                ID id = await Mvx.Resolve<IMapper<ID>>().GetAsync(dataService.IDUID);
                Mvx.RegisterSingleton(() => id);
                FingerprintLoginCommand.Execute();
            }
        }

        /// <summary>
        /// Sets up the AccountService, loading it if a key is saved.
        /// </summary>
        private void SetupAccount()
        {
            IPlatformSpecificDataService dataService = Mvx.Resolve<IPlatformSpecificDataService>();
            _accountService = dataService.LoadAccount();

            if (_accountService == null)
            {
                throw new NoAccountExistsException();
            }
        }

        private async Task StartMnemonicValidation()
        {
            await _navigationService.Navigate<MnemonicValidationViewModel>();
        }

        private async void NavigateRegistration()
        {
            Mvx.RegisterType(() => _accountService);
            await _navigationService.Navigate<RegistrationViewModel>();
        }

        private async void FingerprintLoginAsync()
        {
            var result = await CrossFingerprint.Current.IsAvailableAsync(true);
            if (result)
            {
                var auth = await CrossFingerprint.Current.AuthenticateAsync("Scan fingerprint to access your ID.");
                if (auth.Status == FingerprintAuthenticationResultStatus.TooManyAttempts)
                {
                    _fingerprintAuthenticationMessage = "Too many attempts. Try again later.";
                    RaisePropertyChanged(() => FingerprintAuthenticationMessage);
                }
                else if (auth.Authenticated)
                {
                    _fingerprintAuthenticationMessage = "Fingerprint Authentication Successful!";
                    RaisePropertyChanged(() => FingerprintAuthenticationMessage);
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
