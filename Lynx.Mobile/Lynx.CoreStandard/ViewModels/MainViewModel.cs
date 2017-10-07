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
        public IMvxCommand MainButtonClick => _mainButtonClick;
        public IMvxInteraction<MnemonicCheckInteraction> CreateButtons => _createButtons;

        public string DisplayText => _displayText;
        public string MainButtonText => _mainButtonText;

        private readonly IMvxNavigationService _navigationService;
        private IMvxCommand CheckAndLoadId => new MvxCommand(CheckAndLoadID);

        private string _displayText;
        private MnemonicConfirmation _confirmation;
        private readonly MvxInteraction<MnemonicCheckInteraction> _createButtons = new MvxInteraction<MnemonicCheckInteraction>();
        private IAccountService _accountService;
        private string _mainButtonText;
        private IMvxCommand _mainButtonClick;

        public MainViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override void Start()
        {
            base.Start();
            bool didAccountExist = SetupAccount();
            if(didAccountExist)
                CheckAndLoadId.Execute();
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
                await _navigationService.Navigate<IDViewModel>();
            }
        }

        /// <summary>
        /// Sets up the AccountService, loading it if a key is saved and gneerating it otherwise.
        /// </summary>
        /// <returns>True if it was loaded, False if it was generated</returns>
        private bool SetupAccount()
        {
            IPlatformSpecificDataService dataService = Mvx.Resolve<IPlatformSpecificDataService>();
            _accountService = dataService.LoadAccount();

            if (_accountService == null)
            {
                GenerateAccount();
                return false;
                //We do not have any keys yet
            }
            else
            {
                _displayText = "Loaded existing keys, no seed phrase generated";
                EditMainButton("Register new ID", NavigateRegistration);
                return true;
            }
        }

        private void GenerateAccount()
        {
            _accountService = new AccountService();
            _displayText = "Mnemonic phrase: " + _accountService.MnemonicPhrase;
            CreateMnemonicConfirmation(_accountService.MnemonicPhrase);
            RaisePropertyChanged(() => DisplayText);
        }

        /// <summary>
        /// Resets the mnemonic confirmation process, generating a new key.
        /// </summary>
        private void ResetConfirmation()
        {
            _createButtons.Raise(new MnemonicCheckInteraction(){buttons = {}, onButtonClick = {}});
            GenerateAccount();
        }

        private void CreateMnemonicConfirmation(string mnemonic)
        {
            _confirmation = new MnemonicConfirmation(mnemonic, _createButtons, VerificationSuccess,
                text =>
                {
                    _displayText = text;
                    RaisePropertyChanged(() => DisplayText);
                }
            );
            EditMainButton("I have backed up my seed phrase", StartMnemonicVerification);
        }

        private void StartMnemonicVerification()
        {
            EditMainButton("Generate new seed phrase", ResetConfirmation);
            _confirmation.StartMnemonicVerification();
        }

        private void EditMainButton(string text, Action action)
        {
            _mainButtonClick = new MvxCommand(action, () => true);
            _mainButtonText = text;

            RaisePropertyChanged(() => MainButtonClick);
            RaisePropertyChanged(() => MainButtonText);
        }

        private async void NavigateRegistration()
        {
            Mvx.RegisterType(() => _accountService);
            await _navigationService.Navigate<RegistrationViewModel>();
        }

        private void VerificationSuccess()
        {
            //TODO: Encrypt file, store key in keystore, fingerprint locked
            Mvx.Resolve<IPlatformSpecificDataService>().SaveAccount(_accountService);

            _displayText = "Seed phrase verified, new keys registered";
            RaisePropertyChanged(() => DisplayText);

            _createButtons.Raise(new MnemonicCheckInteraction());

            EditMainButton("Register new ID", NavigateRegistration);
        }

    }
}
