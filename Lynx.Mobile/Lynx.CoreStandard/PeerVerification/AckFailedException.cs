using System;
using System.Runtime.Serialization;

namespace Lynx.Core.PeerVerification
{
    [Serializable]
    internal class AckFailedException : UserFacingException
    {
        public AckFailedException()
        {
        }

        public AckFailedException(string message) : base(message)
        {
        }

        public AckFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}