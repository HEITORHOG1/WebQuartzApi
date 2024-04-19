using Quartz;

namespace JobsQuatz.Jobs
{
    /// <summary>
    /// Interface para representar um job HTTP
    /// </summary>
    public interface IHttpJob : IJob
    {
        void LogNextExecutionTime(string jobName, DateTime nextExecutionTime);

        void LogJobExecution();
    }
}