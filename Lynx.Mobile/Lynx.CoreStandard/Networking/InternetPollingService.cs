using System;
using System.Net;
using System.Threading.Tasks;
using Lynx.Core.Networking;

namespace Lynx.Core.Networking
{
    public class InternetPollingService : IPollingService
    {
        public event EventHandler<EventArgs> PollingFailure;
        public event EventHandler<EventArgs> PollingSuccess;

        public Task<bool> Poll()
        {
            return Task.Run(() =>
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        using (client.OpenRead("http://clients3.google.com/generate_204"))
                        {
                            if (PollingSuccess != null)
                                PollingSuccess.Invoke(this, null);
                            return true;
                        }
                    }
                }
                catch
                {
                    if (PollingFailure != null)
                        PollingFailure.Invoke(this, null);
                    return false;
                }
            });
        }
    }
}
