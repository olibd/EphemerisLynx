using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lynx.Core.Interactions;
using MvvmCross.Core.ViewModels;
using NBitcoin;
using Org.BouncyCastle.Asn1.X509;

namespace Lynx.Core
{
    class MnemonicConfirmation
    {
        private readonly MvxInteraction<MnemonicCheckInteraction> _createButtons;
        private readonly Action _verificationSuccess;
        private readonly Action<string> _updateText;

        private string[] _mnemonic;
        private Stack<string> _mnemonicBackupWords;
        private Stack<int> _wordsToVerify;
        private List<string> _buttons;

        private int failedAttempts = 0;

        public MnemonicConfirmation(string mnemonic, MvxInteraction<MnemonicCheckInteraction> createButtonsInteraction, Action successAction, Action<string> updateTextAction)
        {
            _mnemonic = mnemonic.Split(' ');
            _createButtons = createButtonsInteraction;
            _verificationSuccess = successAction;
            _updateText = updateTextAction;

            _mnemonicBackupWords = new Stack<string>();
            _wordsToVerify = new Stack<int>();
        }

        public void StartMnemonicVerification()
        {
            //Used to add words that are not part of the mnemonic
            _mnemonicBackupWords = new Stack<string>(new Mnemonic(Wordlist.English, WordCount.Twelve).Words);
            _wordsToVerify = new Stack<int>(GenerateRandomValues(4, WordCount.Twelve));

            _buttons = new List<string>();
            foreach(int i in _wordsToVerify)
                _buttons.Add(_mnemonic[i]);

            for(int i = 0; i < _wordsToVerify.Count; i++)
            {
                _buttons.Add(_mnemonicBackupWords.Pop());
            }

            VerifyNext();

        }

        private void VerifyButtonInput(string text)
        {
            string[] mnemonicWords = _mnemonic;
            if (text == mnemonicWords[_wordsToVerify.Peek()])
            {
                _wordsToVerify.Pop();
                if (_wordsToVerify.Count == 0)
                    _verificationSuccess();
                else
                {
                    VerifyNext();
                }
            }
            else
            {
                failedAttempts++;
                StartMnemonicVerification();
            }
        }

        private void VerifyNext()
        {
            _updateText("Please enter the word from your seed phrase in position " +
                                     (_wordsToVerify.Peek() + 1).ToString() + " (" + _wordsToVerify.Count + " left)");

            Random rng = new Random();

            MnemonicCheckInteraction interaction = new MnemonicCheckInteraction()
            {
                buttons = _buttons.OrderBy(x => rng.Next()),
                onButtonClick = VerifyButtonInput
            };

            _createButtons.Raise(interaction);
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

    }
}
