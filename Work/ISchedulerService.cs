using Microsoft.Extensions.Configuration;
using Quartz;

namespace Work
{
    public interface ISchedulerService
    {
        Task InitializeScheduler();

        Task<IScheduler> GetScheduler();
    }
}