using System;

namespace Lynx.Core.Communications
{
    class ConnectOperationImpossibleException : Exception
    {
        public ConnectOperationImpossibleException()
        {
        }

        public ConnectOperationImpossibleException(string message) : base(message)
        {
        }

        public ConnectOperationImpossibleException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
