using System;
using System.Runtime.Serialization;

namespace Lynx.Core.PeerVerification
{
    [Serializable]
    internal class SynAckEncryptionFailedException : UserFacingException
    {
        public SynAckEncryptionFailedException()
        {
        }

        public SynAckEncryptionFailedException(string message) : base(message)
        {
        }

        public SynAckEncryptionFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}