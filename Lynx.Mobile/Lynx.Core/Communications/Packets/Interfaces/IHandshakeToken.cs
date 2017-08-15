using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.Communications.Packets.Interfaces
{
    public interface IHandshakeToken : IToken
    {
        /// <summary>
        /// The ID of the user sending the packet
        /// </summary>
        ID Id { get; set; }

        /// <summary>
        /// The Ethereum public key of the user sending the handshake
        /// </summary>
        string PublicKey { get; set; }

        /// <summary>
        /// The Ethereum public key of the user sending the handshake
        /// </summary>
        bool Encrypted { get; set; }

        /// <summary>
        /// The verifier's name (for identification purposes)
        /// </summary>
        Attribute[] AccessibleAttributes { get; set; }
    }
}
