using Quartz;

namespace Work2
{
    public interface ISchedulerService
    {
        Task<IScheduler> GetScheduler();
    }
}