using System;
namespace Lynx.Core.Models.IDSubsystem
{
    public interface IContent : IDBSerializable
    {
        Object Content { get; set; }
    }
}
