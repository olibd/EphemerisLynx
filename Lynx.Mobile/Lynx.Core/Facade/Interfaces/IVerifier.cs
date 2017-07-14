using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynx.Core.Facade.Interfaces
{
    interface IVerifier
    {
        /// <summary>
        /// The Syn object populated by ProcessSyn
        /// </summary>
        ISyn Syn { get; }

        /// <summary>
        /// Parses a JSON-encoded SYN and verifies its integrity. Stores the result in Ack.
        /// </summary>
        /// <param name="syn">The JSON-encoded SYN</param>
        /// <returns>True if the SYN passed the integrity test, false otherwise</returns>
        bool ProcessSyn(string syn);

        /// <summary>
        /// Creates and transmits an ACK in response to a previously processed SYN
        /// </summary>
        /// <param name="privateKey">The user's Ethereum private key</param>
        /// <param name="publicKey">The user's Ethereum public key</param>
        void Acknowledge(string privateKey, string publicKey);
    }
}
