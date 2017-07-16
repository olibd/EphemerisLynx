using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.Services.Interfaces
{
    public interface IHandshakePayload
    {
        /// <summary>
        /// The ID of the user sending the packet
        /// </summary>
        ID Id { get; set; }

        /// <summary>
        /// The Ethereum public key of the user sending the handshake
        /// </summary>
        string PublicKey { get; set; }
    }
}
