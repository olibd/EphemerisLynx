using System;
using System.Collections.Generic;
using Lynx.Core.Facade;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.Services.Interfaces;
using Newtonsoft.Json;

namespace Lynx.Core.Services
{
    public class Syn : HandshakeToken, ISyn
    {
        public string NetworkAddress
        {
            get
            {
                return Payload["netAddr"];
            }

            set
            {
                Payload["netAddr"] = value;
            }
        }
    }
}
