using Quartz;
using Quartz.Spi;

namespace Work
{
    public class DIJobWorkFactory : IJobFactory
    {
        protected readonly IServiceProvider _serviceProvider;

        public DIJobWorkFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;
            var job = (IJob)_serviceProvider.GetService(jobDetail.JobType);
            if (job == null)
            {
                throw new InvalidOperationException($"Não foi possível resolver o job work '{jobDetail.JobType}'.");
            }
            return job;
        }

        public void ReturnJob(IJob job)
        {
            (job as IDisposable)?.Dispose();
        }
    }
}