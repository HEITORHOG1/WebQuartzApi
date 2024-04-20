using Quartz;

namespace Work2
{
    public class Work1JobListener : IJobListener
    {
        public string Name => "Work1";

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            // Chamado quando a execução do job foi vetada pelo trigger
            Console.WriteLine("JobExecutionVetoed: O job foi vetado pelo trigger.");
            return Task.CompletedTask;
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            // Chamado antes do job ser executado
            Console.WriteLine("JobToBeExecuted: O job está prestes a ser executado.");
            return Task.CompletedTask;
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            // Chamado depois do job ser executado
            Console.WriteLine("JobWasExecuted: O job foi executado com sucesso.");

            if (jobException != null)
            {
                Console.WriteLine($"JobWasExecuted: Ocorreu uma exceção durante a execução do job: {jobException.Message}");
            }

            return Task.CompletedTask;
        }
    }
}