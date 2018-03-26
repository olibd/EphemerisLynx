using System;
using System.Threading.Tasks;

namespace Lynx.Core.Networking
{
    public interface IPollingService
    {
        Task<bool> Poll();
        event EventHandler<EventArgs> PollingFailure;
        event EventHandler<EventArgs> PollingSuccess;
    }
}
