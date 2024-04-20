using Quartz;

namespace JobsQuatz.JobListener
{
    public class HttpJobListener : IJobListener
    {
        public string Name => "HttpJobListener";

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            // Chamado quando a execução do job foi vetada pelo trigger
            return Task.CompletedTask;
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            // Chamado antes do job ser executado
            return Task.CompletedTask;
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            // Chamado depois do job ser executado
            return Task.CompletedTask;
        }
    }
}