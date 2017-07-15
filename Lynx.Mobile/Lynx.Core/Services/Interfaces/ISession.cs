using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynx.Core.Facade.Interfaces
{
    interface ISession
    {
        /// <summary>
        /// Opens a session using the specified address
        /// </summary>
        /// <param name="networkAddress">The session address</param>
        void Open(string networkAddress);

        /// <summary>
        /// Closes the current session
        /// </summary>
        void Close();
    }
}
