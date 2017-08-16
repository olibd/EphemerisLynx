using Lynx.Core.Communications.Packets.Interfaces;
using System.Threading.Tasks;
using System;

namespace Lynx.Core.PeerVerification.Interfaces
{
    /// <summary>
    /// IVerifiers is in charge of handling the verification request coming from
    /// other peers. It will process the Syn request and verify the data supplied
    /// by the peer.
    /// </summary>
    public interface IVerifier
    {
        event EventHandler<IdentityProfileReceivedEvent> IdentityProfileReceived;

        /// <summary>
        /// Parses a JSON-encoded SYN and verifies its integrity.
        /// </summary>
        /// <param name="syn">The JSON-encoded SYN</param>
        /// <returns>The Syn object</returns>
        Task ProcessSyn(string syn);
    }
}
