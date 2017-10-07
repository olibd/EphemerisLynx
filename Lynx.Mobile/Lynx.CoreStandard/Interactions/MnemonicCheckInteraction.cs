using System;
using System.Collections.Generic;

namespace Lynx.Core.Interactions
{
    public class MnemonicCheckInteraction
    {
        public IEnumerable<string> buttons;
        public Action<string> onButtonClick;
    }
}