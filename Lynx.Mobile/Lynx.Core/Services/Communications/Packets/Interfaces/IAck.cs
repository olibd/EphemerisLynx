using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.Services.Communications.Packets.Interfaces
{
    public interface IAck : IHandshakeToken
    {
        /// <summary>
        /// The verifier's name (for identification purposes)
        /// </summary>
        IContent Name { get; set; }
    }
}