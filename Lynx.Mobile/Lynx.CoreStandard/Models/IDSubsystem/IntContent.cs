using System;
namespace Lynx.Core.Models.IDSubsystem
{
    public class IntContent : Model, IContent
    {
        public IntContent(int content)
        {
            Content = content;
        }

        public IntContent(string content)
        {
            Content = int.Parse(content);
        }

        public object Content { get; set; }

        public override string ToString()
        {
            return ((int)Content).ToString();
        }
    }
}
