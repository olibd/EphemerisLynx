using System;
using SQLite;

namespace Lynx.Core.Models.IDSubsystem
{
    public class ExternalElement : SmartContract
    {
        [Unique]
        public string Location { get; set; }

        [Indexed, Unique]
        public string Hash { get; set; }

        [Ignore]
        public IContent Content { get; set; }
    }
}
