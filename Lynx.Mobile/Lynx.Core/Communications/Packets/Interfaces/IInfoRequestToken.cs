using System;
using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.Communications.Packets.Interfaces
{
    public interface IInfoRequestToken : IToken
    {
		/// <summary>
		/// The ID of the user sending the packet
		/// </summary>
		ID Id { get; set; }

		/// <summary>
		/// Specifies if the Token was/will be encrypted or not
		/// </summary>
		bool Encrypted { get; set; }
    }
}
