using System;
using System.Collections.Generic;

namespace Lynx.Core.Interactions
{
    public class MnemonicCheckInteraction
    {
        public List<string> buttons;
        public Action<string> onButtonClick;
    }
}