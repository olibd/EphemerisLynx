using System;
namespace Lynx.Core.Models.IDSubsystem
{
    public class ExternalElement<T> : SmartContract
    {
        public string Location { get; set; }
        public string Hash { get; set; }
        public T Content { get; set; }

        public ExternalElement()
        {
        }

        public ExternalElement(String location)
        {
            Location = location;
        }

        public ExternalElement(string location, string hash)
        {
            Location = location;
            Hash = hash;
        }

        public ExternalElement(string location, string hash, T content)
        {
            Location = location;
            Hash = hash;
            Content = content;
        }
    }
}
