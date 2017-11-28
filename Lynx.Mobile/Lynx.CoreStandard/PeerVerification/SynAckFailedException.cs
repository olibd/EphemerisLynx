using System;
using System.Runtime.Serialization;

namespace Lynx.Core.PeerVerification
{
    [Serializable]
    internal class SynAckFailedException : UserFacingException
    {
        public SynAckFailedException()
        {
        }

        public SynAckFailedException(string message) : base(message)
        {
        }

        public SynAckFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}