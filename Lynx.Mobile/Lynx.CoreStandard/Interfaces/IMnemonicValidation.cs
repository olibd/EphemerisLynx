using System;
using System.Collections.Generic;
using System.Text;
using Lynx.Core.Interactions;

namespace Lynx.Core.Interfaces
{
    public interface IMnemonicValidation
    {
        /// <summary>
        /// This event fires whenever the info text needs to be updated, when prompting the user for another word.
        /// </summary>
        event EventHandler<string> InfoTextUpdate;

        /// <summary>
        /// This event fires when the user successfully verified that they have backed up their mnemonic
        /// </summary>
        event EventHandler OnValidationSuccess;

        /// <summary>
        /// This event fires when the user successfully verified that they have backed up their mnemonic
        /// </summary>
        event EventHandler<MnemonicValidationInteraction> ValidationWordsChanged;

        /// <summary>
        /// Start the validation process, prompting the user for input
        /// </summary>
        void StartMnemonicValidation();
    }

}
