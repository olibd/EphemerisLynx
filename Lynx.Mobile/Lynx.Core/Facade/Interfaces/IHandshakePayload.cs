using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.Facade.Interfaces
{
    interface IHandshakePayload
    {
        /// <summary>
        /// The ID of the user sending the packet
        /// </summary>
        ID Id { get; set; }

        /// <summary>
        /// The Ethereum public key of the user sending the handshake
        /// </summary>
        string PublicKey { get; set; }
    }
}
