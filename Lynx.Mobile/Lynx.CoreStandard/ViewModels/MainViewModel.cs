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
using NBitcoin;
using Lynx.Core.Interactions;

namespace Lynx.Core.ViewModels
{
    public class MainViewModel : MvxViewModel
    {
        private const int _numWordsToCheck = 4;

        public IMvxCommand RegisterClickCommand => new MvxCommand(NavigateRegistration);
        public IMvxCommand BeginMnemonicCheck => new MvxCommand(StartMnemonicVerification);
        public IMvxInteraction<MnemonicCheckInteraction> CreateButtons => _createButtons;

        private IMvxNavigationService _navigationService;
        private IMvxCommand _checkAndLoadID => new MvxCommand(CheckAndLoadID);

        private string _mnemonicPhrase;
        private Mnemonic _mnemonic;
        private Stack<string> _mnemonicBackupWords;
        private Stack<int> _wordsToVerify;
        private MvxInteraction<MnemonicCheckInteraction> _createButtons = new MvxInteraction<MnemonicCheckInteraction>();

        public string MnemonicText
        {
            get => _mnemonicPhrase;
        }

        public MainViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
            _mnemonicBackupWords = new Stack<string>();
            _wordsToVerify = new Stack<int>();
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
                await _navigationService.Navigate<IDViewModel>();
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


            RaisePropertyChanged(() => MnemonicText);

            Mvx.RegisterType(() => accountService);
        }

        private async void NavigateRegistration()
        {
            //TODO: Generate private key and save into file
            //TODO: Encrypt file, store key in keystore, fingerprint locked
            await _navigationService.Navigate<RegistrationViewModel>();
        }

        #region Mnemonic phrase confirmation

        private void StartMnemonicVerification()
        {
            //Used to add words that are not part of the mnemonic
            _mnemonicBackupWords = new Stack<string>(new Mnemonic(Wordlist.English, WordCount.Twelve).Words);
            _wordsToVerify = new Stack<int>(GenerateRandomValues(4, WordCount.Twelve));

            List<string> buttons = new List<string>();
            foreach(int i in _wordsToVerify)
                buttons.Add(_mnemonic.Words[i]);

            for(int i = 0; i < _numWordsToCheck - _wordsToVerify.Count; i++)
            {
                buttons.Add(_mnemonicBackupWords.Pop());
            }

            Random random = new Random();
            buttons.OrderBy(x => random.Next());

            MnemonicCheckInteraction interaction = new MnemonicCheckInteraction()
            {
                buttons = buttons,
                onButtonClick = VerifyButtonInput
            };

            _createButtons.Raise(interaction);

        }

        private void VerifyButtonInput(string text)
        {
            string[] mnemonicWords = _mnemonicPhrase.Split(' ');
            if (text == mnemonicWords[_wordsToVerify.Peek()])
            {
                _wordsToVerify.Pop();
                if (_wordsToVerify.Count == 0)
                    VerificationSuccess();
                else
                {
                    VerifyNext();
                }
            }
        }

        private void VerifyNext()
        {
            throw new NotImplementedException();
        }

        private void VerificationSuccess()
        {
            throw new NotImplementedException();
        }

        private List<int> GenerateRandomValues(int amount, WordCount wordCount)
        {
            Random random = new Random();
            List<int> values = Enumerable.Range(0, (int)wordCount)
                .OrderBy(x => random.Next())
                .Take(amount)
                .ToList();

            return values;
        }

        #endregion
    }
}
