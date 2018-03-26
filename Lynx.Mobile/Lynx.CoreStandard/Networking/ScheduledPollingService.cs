using System;
using System.Threading.Tasks;
using System.Timers;

namespace Lynx.Core.Networking
{
    public abstract class ScheduledPollingService : IPollingService
    {

        public abstract event EventHandler<EventArgs> PollingFailure;
        public abstract event EventHandler<EventArgs> PollingSuccess;
        private Timer _timer;

        public abstract Task<bool> Poll();

        /// <summary>
        /// Poll at the specified interval (milliseconds).
        /// </summary>
        /// <returns>The poll.</returns>
        /// <param name="interval">Interval.</param>
        public virtual void Poll(double interval)
        {
            _timer = new Timer(interval);
            _timer.AutoReset = true;
            _timer.Elapsed += async (sender, e) =>
            {
                await Poll();
            };
            _timer.Start();
        }

        /// <summary>
        /// Stop the polling.
        /// </summary>
        public void Stop()
        {
            _timer.Close();
        }
    }
}
