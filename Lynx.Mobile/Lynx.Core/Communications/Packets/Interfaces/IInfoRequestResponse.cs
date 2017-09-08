using System;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace Lynx.Core.Communications.Packets.Interfaces
{
    public interface IInfoRequestResponse : IInfoRequestToken
    {
        Attribute[] RequestedAttributes { get; set; }
    }
}
