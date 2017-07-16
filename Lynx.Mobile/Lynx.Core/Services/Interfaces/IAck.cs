using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.Services.Interfaces
{
    public interface IAck : IHandshakePayload
    {
        /// <summary>
        /// The verifier's name (for identification purposes)
        /// </summary>
        IContent Name { get; set; }
    }
}