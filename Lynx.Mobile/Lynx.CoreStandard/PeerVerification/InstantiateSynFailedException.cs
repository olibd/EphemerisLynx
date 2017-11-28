using System;
using System.Runtime.Serialization;

namespace Lynx.Core.PeerVerification
{
    [Serializable]
    internal class InstantiateSynFailedException : UserFacingException
    {
        public InstantiateSynFailedException()
        {
        }

        public InstantiateSynFailedException(string message) : base(message)
        {
        }

        public InstantiateSynFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}