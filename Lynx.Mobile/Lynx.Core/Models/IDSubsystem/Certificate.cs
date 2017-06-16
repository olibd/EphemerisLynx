using System;

namespace Lynx.Core.Models.IDSubsystem
{
    public class Certificate : ExternalElement
    {
        public bool Revoked { get; set; }
        public Attribute OwningAttribute { get; set; }
    }
}
