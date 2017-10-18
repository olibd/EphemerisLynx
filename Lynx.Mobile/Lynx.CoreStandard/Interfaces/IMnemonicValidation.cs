using System;
using System.Collections.Generic;
using System.Text;
using Lynx.Core.Interactions;

namespace Lynx.Core.Interfaces
{
    public interface IMnemonicValidation
    {
        event EventHandler<string> InfoTextUpdate;
        event EventHandler OnValidationSuccess;
        event EventHandler<MnemonicValidationInteraction> ValidationWordsChanged;

        void StartMnemonicValidation();
    }

}
