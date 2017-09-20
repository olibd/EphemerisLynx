using System;
namespace Lynx.Core.Models.Interactions
{
    public class BooleanInteraction
    {
        public Action<bool> Callback { get; set; }
        public string Query { get; set; }
    }
}
