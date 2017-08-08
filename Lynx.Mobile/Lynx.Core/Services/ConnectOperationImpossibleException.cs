using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynx.Core.Services
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
