using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Lynx.Core.Models.IDSubsystem
{
    public class ExternalElement : SmartContract
    {
        [Unique]
        public string Location { get; set; }

        [Indexed, Unique]
        public string Hash { get; set; }

        [ForeignKey(typeof(IContent))]
        private string ContentId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public IContent Content { get; set; }
    }
}
