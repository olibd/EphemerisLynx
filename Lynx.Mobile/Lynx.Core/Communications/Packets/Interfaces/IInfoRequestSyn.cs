using System;

namespace Lynx.Core.Communications.Packets.Interfaces
{
    public interface IInfoRequestSyn : IInfoRequestToken
    {
		string NetworkAddress { get; set; }
    }
}
