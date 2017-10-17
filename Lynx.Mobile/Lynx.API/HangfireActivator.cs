using System;
namespace Lynx.API
{
    /// <summary>
    /// The Hangfire activator is a HangFire job activator which is used to activate
    /// jobs while supplying the .NET Core ServiceProvider so that the job class
    /// can resolve dependencies.
    /// </summary>
    public class HangfireActivator : Hangfire.JobActivator
    {
        private readonly IServiceProvider _serviceProvider;

        public HangfireActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override object ActivateJob(Type type)
        {
            return _serviceProvider.GetService(type);
        }
    }
}
