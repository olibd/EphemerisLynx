using System;

namespace Lynx.Core.PeerVerification
{
    internal class TokenPublicKeyMismatchException : Exception
    {
        public TokenPublicKeyMismatchException()
        {
        }

        public TokenPublicKeyMismatchException(string message) : base(message)
        {
        }

        public TokenPublicKeyMismatchException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}