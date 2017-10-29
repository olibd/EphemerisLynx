using System;
using System.Threading.Tasks;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Interfaces;
using Lynx.Core.Mappers.IDSubsystem.Strategies;
using Lynx.Core.Models;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.Models.Interactions;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using NBitcoin;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace Lynx.Core.ViewModels
{
    public class RecoveryViewModel : MvxViewModel
    {
        private IMvxNavigationService _navigationService;

        private string _buttonText;
        private string _mnemonicInput;
        private IMvxCommand _onSubmitClick;

        public IMvxCommand OnSubmitClick
        {
            get => _onSubmitClick;
        }

        public string MnemonicInput
        {
            get => _mnemonicInput;
            set
            {
                _mnemonicInput = value;
                RaisePropertyChanged(() => MnemonicInput);
            }
        }

        public string ButtonText
        {
            get => _buttonText;
            set
            {
                _buttonText = value;
                RaisePropertyChanged(() => ButtonText);
            }
        }

        private bool _canSubmit;

        private bool CanSubmit
        {
            get => _canSubmit;
            set
            {
                if (value != _canSubmit)
                {
                    _canSubmit = value;
                    RaisePropertyChanged(() => OnSubmitClick);
                }
            }
        }

        public RecoveryViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
            _onSubmitClick = new MvxCommand(DoRecovery, () => CanSubmit);
            PropertyChanged += (sender, args) =>
            {
                CanSubmit = CheckMnemonicValidity();
            };
        }

        private bool CheckMnemonicValidity()
        {
            try
            {
                Mnemonic mnemonic = new Mnemonic(MnemonicInput, Wordlist.English);
                return mnemonic.IsValidChecksum;
            }
            catch (FormatException e)
            {
                return false;
            }
        }

        private async void DoRecovery()
        {
            //Disable the submit button
            _onSubmitClick = new MvxCommand(() => { }, () => false);
            RaisePropertyChanged(() => OnSubmitClick);

            Mnemonic mnemonic = new Mnemonic(MnemonicInput, Wordlist.English);
            AccountService newAccount = new AccountService(mnemonic);
            Mvx.Resolve<IPlatformSpecificDataService>().SaveAccount(newAccount);

            IIDFacade _idFacade = Mvx.Resolve<IIDFacade>();
            ID id = await _idFacade.RecoverIDAsync();

            await Mvx.Resolve<IMapper<ID>>().SaveAsync(id);
            Mvx.RegisterType(() => id);

            await _navigationService.Navigate<IDViewModel>();
        }

        public override void Start()
        {
        }
    }
}