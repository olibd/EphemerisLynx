using System;
using System.Runtime.Serialization;

namespace Lynx.Core.Crypto
{
    internal class InvalidTokenFormatException : UserFacingException
    {
        public InvalidTokenFormatException()
        {
        }

        public InvalidTokenFormatException(string message) : base(message)
        {
        }

        public InvalidTokenFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}