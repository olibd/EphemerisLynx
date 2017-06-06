using System;
namespace Lynx.Core.Models.IDSubsystem
{
    public class Certificate<T> : ExternalElement<T>
    {
        public bool Revoked { get; set; }
    }
}
