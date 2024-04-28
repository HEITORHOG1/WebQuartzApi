using Microsoft.Extensions.Logging;
using Quartz;

namespace Work2
{
    [DisallowConcurrentExecution]
    internal class WorkJob : IJob
    {
        private readonly ILogger<WorkJob> _logger;

        public WorkJob(ILogger<WorkJob> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Método para executar o job
        /// </summary>
        /// <param name="context">O contexto do job</param>
        /// <returns>Uma tarefa assíncrona representando a execução do job</returns>
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                var nextFireTimeUtc = context?.Trigger?.GetNextFireTimeUtc();
                if (nextFireTimeUtc.HasValue)
                {
                    var nextExecutionTime = TimeZoneInfo.ConvertTimeFromUtc(nextFireTimeUtc.Value.DateTime, brasiliaTimeZone);
                    LogNextExecutionTime(context.JobDetail.Key.Name, nextExecutionTime);
                }

                LogJobExecution();
                _logger?.LogInformation("Job work2  executado com sucesso");
            }
            catch (Exception ex)
            {
                _logger?.LogError($"Error executing job work2 : {ex.Message}", ex);
                throw; // Consider re-throwing to let the scheduler know the job failed.
            }
        }

        private void LogNextExecutionTime(string jobName, DateTime nextExecutionTime)
        {
            _logger.LogInformation($"Próxima execução do job work2  {jobName}: {nextExecutionTime:dd/MM/yyyy HH:mm:ss}");
        }

        private void LogJobExecution()
        {
            _logger.LogInformation("Executando o job work2 ...");
            _logger.LogInformation($"Data e hora atual: {DateTime.Now}");
        }
    }
}