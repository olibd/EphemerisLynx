﻿using Lynx.Core.Communications.Packets.Interfaces;

namespace Lynx.Core.PeerVerification.Interfaces
{
    /// <summary>
    /// IVerifiers is in charge of handlign the verification request coming from
    /// other peers. It will process the Syn request and verify the data supplied
    /// by the peer.
    /// </summary>
    public interface IVerifier
    {
        /// <summary>
        /// The Syn object populated by ProcessSyn
        /// </summary>
        ISyn Syn { get; set; }

        /// <summary>
        /// Parses a JSON-encoded SYN and verifies its integrity.
        /// </summary>
        /// <param name="syn">The JSON-encoded SYN</param>
        /// <returns>The Syn object</returns>
        ISyn ProcessSyn(string syn);

        /// <summary>
        /// Creates and transmits an ACK in response to a previously processed SYN
        /// </summary>
        /// <param name="privateKey">The user's Ethereum private key</param>
        /// <param name="publicKey">The user's Ethereum public key</param>
        void Acknowledge(string privateKey, string publicKey);

    }
}