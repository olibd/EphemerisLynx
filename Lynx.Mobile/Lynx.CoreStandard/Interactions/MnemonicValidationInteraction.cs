using System;
using System.Collections.Generic;

namespace Lynx.Core.Interactions
{
    public class MnemonicValidationInteraction
    {
        public IEnumerable<string> buttons;
        public Action<string> onButtonClick;
    }
}