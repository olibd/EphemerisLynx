using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lynx.Core.Interactions;
using Lynx.Core.Interfaces;
using NBitcoin;

namespace Lynx.Core
{
    class MnemonicValidation : IMnemonicValidation
    {
        public event EventHandler<string> InfoTextUpdate;
        public event EventHandler OnValidationSuccess;  
        public event EventHandler<MnemonicValidationInteraction> ValidationWordsChanged;  

        private string[] _mnemonic;
        private Stack<string> _mnemonicBackupWords;
        private Stack<int> _wordsToVerify;
        private List<string> _buttons;

        public MnemonicValidation(string mnemonic)
        {
            _mnemonic = mnemonic.Split(' ');

            _mnemonicBackupWords = new Stack<string>();
            _wordsToVerify = new Stack<int>();
        }

        public void StartMnemonicValidation()
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
                    OnValidationSuccess(this, new EventArgs());
                else
                {
                    VerifyNext();
                }
            }
            else
            {
                StartMnemonicValidation();
            }
        }

        private void VerifyNext()
        {
            InfoTextUpdate(this, "Please enter the word from your seed phrase in position " +
                                     (_wordsToVerify.Peek() + 1) + " (" + _wordsToVerify.Count + " left)");

            Random rng = new Random();

            MnemonicValidationInteraction interaction = new MnemonicValidationInteraction()
            {
                buttons = _buttons.OrderBy(x => rng.Next()),
                onButtonClick = VerifyButtonInput
            };

            ValidationWordsChanged(this, interaction);
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
