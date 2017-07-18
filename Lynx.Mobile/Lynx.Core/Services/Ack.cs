using System;
using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.Services.Interfaces;
using Newtonsoft.Json;

namespace Lynx.Core.Services
{
    public class Ack : HandshakeToken, IAck
    {
        private IContent _name;
        public IContent Name
        {
            get
            {
                return _name;
            }

            set
            {
                Payload["name"] = JsonConvert.SerializeObject(value);
                _name = value;
            }
        }
    }
}
