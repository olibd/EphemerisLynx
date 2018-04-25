using System;
using System.Runtime.Serialization;

namespace Lynx.Core.ViewModels
{
    [Serializable]
    internal class NoAccountExistsException : Exception
    {
        public NoAccountExistsException()
        {
        }

        public NoAccountExistsException(string message) : base(message)
        {
        }

        public NoAccountExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoAccountExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}