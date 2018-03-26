using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lynx.Core.Networking
{
    public class NetworkRessourcesPollingService : ScheduledPollingService
    {
        private List<IPollingService> _pollingServices;

        public override event EventHandler<EventArgs> PollingFailure;
        public override event EventHandler<EventArgs> PollingSuccess;
        private enum State { SINGLE_POLL, SCHEDULED_POLLS_INIT, RESPONDING, NOT_RESPONDING };
        private State _previousState = State.NO_STATE;

        public NetworkRessourcesPollingService()
        {
            _pollingServices = new List<IPollingService>();
        }

        /// <summary>
        /// Poll the collection of network services added to the instance.
        /// Stops on first failure.
        /// </summary>
        /// <returns>The poll.</returns>
        public override async Task<bool> Poll()
        {
            State state = _previousState;
            foreach (IPollingService service in _pollingServices)
            {
                if (!await service.Poll())
                {
                    if (PollingFailure != null && _previousState != State.NOT_RESPONDING)
                    {
                        PollingFailure.Invoke(this, null);
                    }
                    state = State.NOT_RESPONDING;
                    return false;
                }
            }

            if (PollingSuccess != null && _previousState != State.RESPONDING)
            {
                PollingSuccess.Invoke(this, null);
            }
            state = State.RESPONDING;

            if (SINGLE_POLL)

                return true;
        }

        /// <summary>
        /// Poll at the specified interval (milliseconds).
        /// </summary>
        /// <returns>The poll.</returns>
        /// <param name="interval">Interval.</param>
        public override void Poll(double interval)
        {
            _previousState = false;
            _firstPoll = true;
            base.Poll(interval);
        }

        /// <summary>
        /// Add the specified service to the collection of services to poll.
        /// </summary>
        /// <returns>The add.</returns>
        /// <param name="service">Service.</param>
        public void Add(IPollingService service)
        {
            _pollingServices.Add(service);
        }

    }
}
