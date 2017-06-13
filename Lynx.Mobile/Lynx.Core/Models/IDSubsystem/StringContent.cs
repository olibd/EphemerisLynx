using System;
namespace Lynx.Core.Models.IDSubsystem
{
    public class StringContent : IContent
    {
        public StringContent(String content)
        {
            Content = content;
        }

        public object Content { get; set; }
    }
}
