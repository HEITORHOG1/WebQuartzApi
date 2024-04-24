using Quartz;
using Quartz.Spi;

namespace Work
{
    public class DIJobWork1Factory : IJobFactory
    {
        protected readonly IServiceProvider _serviceProvider;

        public DIJobWork1Factory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;
            var job = (IJob)_serviceProvider.GetService(jobDetail.JobType);
            if (job == null)
            {
                throw new InvalidOperationException($"Não foi possível resolver o job '{jobDetail.JobType}'.");
            }
            return job;
        }

        public void ReturnJob(IJob job)
        {
            (job as IDisposable)?.Dispose();
        }
    }
}