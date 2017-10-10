using System;
using System.Net.Http;
using System.Threading;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.PeerVerification;

namespace Lynx.API
{
    public abstract class InfoRequestWorker
    {
        public static void CompleteInfoRequest(InfoRequester infoRequester, string callbackEndpoint)
        {
            ManualResetEvent waitHandle = new ManualResetEvent(false);
            ID id = null;
            infoRequester.handshakeComplete += (sender, e) =>
            {
                id = e.Id;
                waitHandle.Set();
            };

            if (waitHandle.WaitOne(100))
                new HttpClient().PostAsync(callbackEndpoint, new System.Net.Http.StringContent(id.Address));
            else
                new HttpClient().PostAsync(callbackEndpoint, new System.Net.Http.StringContent("error"));
        }
    }
}
