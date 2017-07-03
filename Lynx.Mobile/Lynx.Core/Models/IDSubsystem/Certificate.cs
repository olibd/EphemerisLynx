using System;
using SQLite;

namespace Lynx.Core.Models.IDSubsystem
{
    public class Certificate : ExternalElement
    {
        public bool Revoked { get; set; }
        [Ignore]
        public Attribute OwningAttribute { get; set; }
    }
}
