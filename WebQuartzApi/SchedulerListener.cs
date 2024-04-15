using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Quartz;

namespace WebQuartzApi
{
    public class SchedulerListener : ISchedulerListener
    {
        private IScheduler _scheduler;

        public SchedulerListener(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public Task JobScheduled(Trigger trigger)
        {
            return Task.CompletedTask;
        }

        public Task JobsPaused(string jobName, string jobGroup)
        {
            throw new NotImplementedException();
        }

        public Task JobsResumed(string jobName, string jobGroup)
        {
            throw new NotImplementedException();
        }

        public Task JobUnscheduled(string triggerName, string triggerGroup)
        {
            throw new NotImplementedException();
        }

        public Task SchedulerError(string msg, Quartz.SchedulerException cause)
        {
            throw new NotImplementedException();
        }

        public Task SchedulerShutdown()
        {
            throw new NotImplementedException();
        }

        public Task TriggerFinalized(Trigger trigger)
        {
            throw new NotImplementedException();
        }

        public Task TriggersPaused(string triggerName, string triggerGroup)
        {
            throw new NotImplementedException();
        }

        public Task TriggersResumed(string triggerName, string triggerGroup)
        {
            throw new NotImplementedException();
        }
    }
}