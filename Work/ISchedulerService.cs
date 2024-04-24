using Quartz;

namespace Work
{
    public interface ISchedulerService
    {
        Task<IScheduler> GetScheduler();
    }
}