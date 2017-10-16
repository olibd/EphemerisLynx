using Lynx.Core.Communications.Packets.Interfaces;
using System.Threading.Tasks;
using System;
using Lynx.Core.Communications.Packets;

namespace Lynx.Core.PeerVerification.Interfaces
{
    /// <summary>
    /// IVerifiers is in charge of handling the verification request coming from
    /// other peers. It will process the Syn request and verify the data supplied
    /// by the peer.
    /// </summary>
    public interface IReceiver
    {
        event EventHandler<IdentityProfileReceivedEvent> IdentityProfileReceived;
        event EventHandler<CertificatesSent> CertificatesSent;
        event EventHandler<InfoRequestReceivedEvent> InfoRequestReceived;
        event EventHandler<InfoRequestAuthorizedEvent> InfoRequestAuthorized;
        ISynAck SynAck { get; }
        InfoRequestSynAck InfoRequestSynAck { get; }
        /// <summary>
        /// Parses a JSON-encoded SYN and verifies its integrity.
        /// </summary>
        /// <param name="syn">The JSON-encoded SYN</param>
        Task ProcessSyn(string syn);
        Task Certify(string[] keysOfAttributesToCertifify);
        void AuthorizeReadRequest(string[] keysOfAttributesToAuthorize);
    }
}
