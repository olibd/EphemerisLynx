using System;
namespace Lynx.Core.Models.IDSubsystem
{
    public class IntContent : Model, IContent
    {
        public IntContent(int content)
        {
            Content = content;
        }

        public object Content { get; set; }
    }
}
