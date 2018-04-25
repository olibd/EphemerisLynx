using System;
namespace Lynx.Core.Models.IDSubsystem
{
    public class StringContent : Model, IContent
    {
        public StringContent(String content)
        {
            Content = content;
        }

        public object Content { get; set; }

        public override string ToString()
        {
            return (String)Content;
        }
    }
}
