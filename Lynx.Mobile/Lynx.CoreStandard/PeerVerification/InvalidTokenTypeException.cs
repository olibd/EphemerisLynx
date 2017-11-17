using System;

namespace Lynx.Core.PeerVerification
{
    internal class InvalidTokenTypeException : UserFacingException
    {
        public InvalidTokenTypeException()
        {
        }

        public InvalidTokenTypeException(string message) : base(message)
        {
        }

        public InvalidTokenTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}