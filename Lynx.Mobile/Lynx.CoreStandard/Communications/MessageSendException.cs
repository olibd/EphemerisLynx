using System;
using System.Runtime.Serialization;
using IO.Ably;

namespace Lynx.Core.Communications
{
    internal class MessageSendException : UserFacingException
    {
        public MessageSendException()
        {
        }

        public MessageSendException(string message) : base(message)
        {
        }

        public MessageSendException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}