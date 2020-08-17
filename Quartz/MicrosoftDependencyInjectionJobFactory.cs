using Quartz;
using Quartz.Spi;
using System;

namespace StsKlassifikation.Quartz
{
    internal class MicrosoftDependencyInjectionJobFactory : IJobFactory
    {
        private readonly IServiceProvider sp;

        public MicrosoftDependencyInjectionJobFactory(IServiceProvider serviceProvider)
        {
            this.sp = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return sp.GetService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            (job as IDisposable)?.Dispose();
        }
    }
}