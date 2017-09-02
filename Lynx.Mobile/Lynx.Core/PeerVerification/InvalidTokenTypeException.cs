using System;

namespace Lynx.Core.PeerVerification
{
    internal class InvalidTokenTypeException : Exception
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