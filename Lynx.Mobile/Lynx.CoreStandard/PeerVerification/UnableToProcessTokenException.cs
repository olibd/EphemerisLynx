using System;
using System.Runtime.Serialization;

namespace Lynx.Core.PeerVerification
{
    [Serializable]
    internal class UnableToProcessTokenException : UserFacingException
    {
        public UnableToProcessTokenException()
        {
        }

        public UnableToProcessTokenException(string message) : base(message)
        {
        }

        public UnableToProcessTokenException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}