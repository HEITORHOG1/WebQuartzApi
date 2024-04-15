using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Quartz;

namespace WebQuartzApi
{
    public interface ISchedulerListener
    {
        Task JobScheduled(Trigger trigger);

        Task JobUnscheduled(string triggerName, string triggerGroup);

        Task TriggerFinalized(Trigger trigger);

        Task TriggersPaused(string triggerName, string triggerGroup);

        Task TriggersResumed(string triggerName, string triggerGroup);

        Task JobsPaused(string jobName, string jobGroup);

        Task JobsResumed(string jobName, string jobGroup);

        Task SchedulerError(string msg, SchedulerException cause);

        Task SchedulerShutdown();
    }
}