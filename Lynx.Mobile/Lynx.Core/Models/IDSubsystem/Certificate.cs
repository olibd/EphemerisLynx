using System;
using SQLiteNetExtensions.Attributes;

namespace Lynx.Core.Models.IDSubsystem
{
    public class Certificate : ExternalElement
    {
        [ForeignKey(typeof(Attribute))]
        private int AttributeMUID { get; set; }
        public bool Revoked { get; set; }
    }
}
