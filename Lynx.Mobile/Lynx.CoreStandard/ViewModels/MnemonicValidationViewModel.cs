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
    public class MnemonicValidationViewModel : MvxViewModel
    {
        public IMvxCommand MainButtonClick => _mainButtonClick;

        //The IMvxInteraction and the MvxInteraction must be separate because the Raise() method is not part if the IMvxInteraction interface.
        public IMvxInteraction<MnemonicValidationInteraction> CreateButtons => _createButtons;

        public string DisplayText => _displayText;
        public string MainButtonText => _mainButtonText;

        private readonly IMvxNavigationService _navigationService;

        private string _displayText;
        private IMnemonicValidation _validation;
        private readonly MvxInteraction<MnemonicValidationInteraction> _createButtons = new MvxInteraction<MnemonicValidationInteraction>();
        private IAccountService _accountService;
        private string _mainButtonText;
        private IMvxCommand _mainButtonClick;

        public MnemonicValidationViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override void Start()
        {
            base.Start();

            GenerateAccount();
        }

        private void GenerateAccount()
        {
            _accountService = new AccountService();
            UpdateInfoText("Your mnemonic phrase is: " + _accountService.MnemonicPhrase);
            CreateMnemonicValidation(_accountService.MnemonicPhrase);
        }

        /// <summary>
        /// Resets the mnemonic confirmation process, generating a new key.
        /// </summary>
        private void ResetValidation()
        {
            // Passing an empty list of buttons simply deletes all existing buttons
            _createButtons.Raise(new MnemonicValidationInteraction() { buttons = { }, onButtonClick = { } });
            GenerateAccount();
        }

        /// <summary>
        /// Creates the instance of MnemonicValidation that will perform the validation, but does not start the process yet
        /// </summary>
        /// <param name="mnemonic"></param>
        private void CreateMnemonicValidation(string mnemonic)
        {
            _validation = new MnemonicValidation(mnemonic);

            _validation.OnValidationSuccess += (s, a) => ValidationSuccess();
            _validation.InfoTextUpdate += (s, str) => UpdateInfoText(str);
            _validation.ValidationWordsChanged += (s, interaction) => _createButtons.Raise(interaction);

            EditMainButton("I have backed up my seed phrase", StartMnemonicValidation);
        }

        private void StartMnemonicValidation()
        {
            EditMainButton("Generate new seed phrase", ResetValidation);
            _validation.StartMnemonicValidation();
        }

        private void EditMainButton(string text, Action action)
        {
            _mainButtonClick = new MvxCommand(action, () => true);
            _mainButtonText = text;

            RaisePropertyChanged(() => MainButtonClick);
            RaisePropertyChanged(() => MainButtonText);
        }

        private void UpdateInfoText(string text)
        {
            _displayText = text;
            RaisePropertyChanged(() => DisplayText);
        }

        private async void NavigateRegistration()
        {
            Mvx.RegisterType(() => _accountService);
            await _navigationService.Navigate<RegistrationViewModel>();
        }

        private void ValidationSuccess()
        {
            //TODO: Encrypt file, store key in keystore, fingerprint locked
            Mvx.Resolve<IPlatformSpecificDataService>().SaveAccount(_accountService);
            Mvx.RegisterType<IAccountService>(() => _accountService);

            _displayText = "Seed phrase verified, new keys registered";
            RaisePropertyChanged(() => DisplayText);

            _createButtons.Raise(new MnemonicValidationInteraction());

            EditMainButton("Register new ID", NavigateRegistration);
        }

    }
}
