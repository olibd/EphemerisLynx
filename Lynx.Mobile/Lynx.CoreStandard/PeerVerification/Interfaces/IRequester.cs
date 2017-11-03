using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lynx.Core.Communications.Packets.Interfaces;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace Lynx.Core.PeerVerification.Interfaces
{
    /// <summary>
    /// IRequester is in charge of initiating the peer to peer verification
    /// requests. It is a representation of the Requesting entity in a peer to
    /// peer verification transaction. As such, the Requester will also be in
    /// charge of sending the data to be verified the the other party, the 
    /// Verifier.
    /// </summary>
    public interface IRequester
    {
        /// <summary>
        /// Creates a verification request SYN from the user's ID and encodes it into a JSON string
        /// </summary>
        /// <returns>A JSON string representing the SYN payload</returns>
        string CreateEncodedSyn();
        void ResumeSession(string sessionID);
        string SuspendSession();
        IAck Ack { get; set; }
    }
}
