using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lynx.Core.Interactions;
using Lynx.Core.Interfaces;
using NBitcoin;

namespace Lynx.Core
{
    /// <summary>
    /// This class contains the logic necessary to perform key generation while verifying that the user does have a backup.
    /// </summary>
    class MnemonicValidation : IMnemonicValidation
    {
        /// <summary>
        /// This event fires whenever the info text needs to be updated, when prompting the user for another word.
        /// </summary>
        public event EventHandler<string> InfoTextUpdate;

        /// <summary>
        /// This event fires when the user successfully verified that they have backed up their mnemonic.
        /// </summary>
        public event EventHandler OnValidationSuccess;  

        /// <summary>
        /// This event fires when the words presented to the user change.
        /// </summary>
        public event EventHandler<MnemonicValidationInteraction> ValidationWordsChanged;  

        private string[] _mnemonic;
        private Stack<string> _promptsNotInMnemonic; //Random words not in the mnemonic string
        private Stack<int> _promptsInMnemonic; //Indices of the words in the mnemonic string
        private List<string> _buttons; 

        public MnemonicValidation(string mnemonic)
        {
            _mnemonic = mnemonic.Split(' ');

            _promptsNotInMnemonic = new Stack<string>();
            _promptsInMnemonic = new Stack<int>();
        }

        /// <summary>
        /// Start the validation process, prompting the user for input
        /// </summary>
        public void StartMnemonicValidation()
        {
            _promptsNotInMnemonic = new Stack<string>(new Mnemonic(Wordlist.English, WordCount.TwentyFour).Words);
            _promptsInMnemonic = new Stack<int>(GenerateRandomValues(4, WordCount.Twelve));

            // Add the 4 words that we will want to verify, and 4 other random words, to the mnemonic

            _buttons = new List<string>();
            foreach(int i in _promptsInMnemonic)
                _buttons.Add(_mnemonic[i]);

            for(int i = 0; i < _promptsInMnemonic.Count; i++)
            {
                _buttons.Add(_promptsNotInMnemonic.Pop());
            }

            VerifyNext();

        }

        private void VerifyButtonInput(string text)
        {
            string[] mnemonicWords = _mnemonic;
            if (text == mnemonicWords[_promptsInMnemonic.Peek()])
            {
                _promptsInMnemonic.Pop();
                if (_promptsInMnemonic.Count == 0)
                    OnValidationSuccess(this, new EventArgs());
                else
                {
                    VerifyNext();
                }
            }
            else
            {
                // If the user gets a word wrong, they restart the validation
                StartMnemonicValidation();
            }
        }

        private void VerifyNext()
        {
            InfoTextUpdate(this, "Please enter the word from your seed phrase in position " +
                                     (_promptsInMnemonic.Peek() + 1) + " (" + _promptsInMnemonic.Count + " left)");

            // RNG is only used to shuffle the buttons
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
