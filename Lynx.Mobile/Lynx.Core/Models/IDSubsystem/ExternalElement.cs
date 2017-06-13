using System;
namespace Lynx.Core.Models.IDSubsystem
{
    public class ExternalElement : SmartContract
    {
        public string Location { get; set; }
        public string Hash { get; set; }
        public IContent Content { get; set; }

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

        public ExternalElement(string location, string hash, IContent content)
        {
            Location = location;
            Hash = hash;
            Content = content;
        }
    }
}
