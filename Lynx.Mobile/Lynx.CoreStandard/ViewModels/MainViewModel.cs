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
using NBitcoin;
using Lynx.Core.Interactions;

namespace Lynx.Core.ViewModels
{
    public class MainViewModel : MvxViewModel
    {
        public IMvxCommand RegistrationButtonClick => new MvxCommand(Register);
        public IMvxCommand RecoveryButtonClick => new MvxCommand(NavigateRecovery);

        private readonly IMvxNavigationService _navigationService;
        private IMvxCommand CheckAndLoadId => new MvxCommand(CheckAndLoadID);

        private bool _mustGenerateKeys = false;

        private IAccountService _accountService;

        public MainViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override void Start()
        {
            base.Start();
            
            try
            {
                SetupAccount();
                CheckAndLoadId.Execute();
            }
            catch (NoAccountExistsException e)
            {
                _mustGenerateKeys = true;
            }
        }

        private async void CheckAndLoadID()
        {
            IPlatformSpecificDataService dataService = Mvx.Resolve<IPlatformSpecificDataService>();
            if (dataService.IDUID != -1)
            {
                //TODO: Load from database, and fallback to loading from blockchain in case of exception

                ID id = await Mvx.Resolve<IMapper<ID>>().GetAsync(dataService.IDUID);
                Mvx.RegisterSingleton(() => id);
                await _navigationService.Navigate<IDViewModel>();
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

        private async void Register()
        {
            if (_mustGenerateKeys)
                new MvxCommand(NavigateMnemonicValidation).Execute();
            else
                new MvxCommand(NavigateRegistration);
        }

        private async void NavigateMnemonicValidation()
        {
            await _navigationService.Navigate<MnemonicValidationViewModel>();
        }

        private async void NavigateRegistration()
        {
            Mvx.RegisterType(() => _accountService);
            await _navigationService.Navigate<RegistrationViewModel>();
        }

        private async void NavigateRecovery()
        {
            await _navigationService.Navigate<RecoveryViewModel>();
        }
    }
}
