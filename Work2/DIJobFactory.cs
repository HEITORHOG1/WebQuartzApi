using Quartz;
using Quartz.Spi;

namespace Work2
{
    public class DIJobFactory : IJobFactory
    {
        protected readonly IServiceProvider _serviceProvider;

        public DIJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;
            var job = (IJob)_serviceProvider.GetService(jobDetail.JobType);
            if (job == null)
            {
                throw new InvalidOperationException($"Não foi possível resolver o job work2 '{jobDetail.JobType}'.");
            }
            return job;
        }

        public void ReturnJob(IJob job)
        {
            (job as IDisposable)?.Dispose();
        }
    }
}