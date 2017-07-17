using System;
using System.Collections.Generic;
using Lynx.Core.Services.Interfaces;

namespace Lynx.Core.Services
{
    public class Requester : IRequester
    {
        public Requester()
        {
        }

        public IAck Ack { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string CreateSyn(string privateKey, string publicKey)
        {
            throw new NotImplementedException();


        }

        public IAck ProcessAck(string ack)
        {
            throw new NotImplementedException();
        }

        public void SendAttributes(List<Attribute> attributes)
        {
            throw new NotImplementedException();
        }
    }
}
