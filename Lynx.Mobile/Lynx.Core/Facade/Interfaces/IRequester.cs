using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynx.Core.Facade.Interfaces
{
    interface IRequester : ISession
    {
        /// <summary>
        /// The Ack object populated by ProcessAck
        /// </summary>
        IAck Ack { get; }

        /// <summary>
        /// Creates a verification request SYN from the user's ID and encodes it into a JSON string
        /// </summary>
        /// <param name="privateKey">The user's Ethereum private key</param>
        /// <param name="publicKey">The user's Ethereum public key</param>
        /// <returns>A JSON string representing the SYN payload</returns>
        string CreateSyn(string privateKey, string publicKey);

        /// <summary>
        /// Parses a JSON-encoded ACK and verifies its integrity. Stores the result in Ack.
        /// </summary>
        /// <param name="ack">The JSON-encoded ACK</param>
        /// <returns>True if the ACK passed the integrity test, false otherwise</returns>
        bool ProcessAck(string ack);

        /// <summary>
        /// JSON-Encodes and sends attributes and attribute contents to the verifier for certification
        /// </summary>
        /// <param name="attributes">The attributes to transmit</param>
        void SendAttributes(List<Attribute> attributes);

    }
}
