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
        /// Specifies if the Token was/will be encrypted or not
        /// </summary>
        bool Encrypted { get; set; }

        /// <summary>
        /// The Accessible ID attributes that the peer can read
        /// </summary>
        Attribute[] AccessibleAttributes { get; set; }
    }
}
