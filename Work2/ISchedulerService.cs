using Quartz;

namespace Work2
{
    public interface ISchedulerService
    {
        Task InitializeScheduler();

        Task<IScheduler> GetScheduler();
    }
}