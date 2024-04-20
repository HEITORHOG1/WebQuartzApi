using Microsoft.Extensions.Logging;
using Quartz;

namespace Work2
{
    /// <summary>
    /// Classe que representa um job HTTP
    /// </summary>
    [DisallowConcurrentExecution]
    public class Work1Job : IJob
    {
        private readonly ILogger<Work1Job> _logger;

        public Work1Job(ILogger<Work1Job> logger)
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
            // Supondo que você tenha acesso ao fuso horário de Brasília aqui.
            var brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            var nextFireTimeUtc = context.Trigger.GetNextFireTimeUtc();
            if (nextFireTimeUtc.HasValue)
            {
                var nextExecutionTime = TimeZoneInfo.ConvertTimeFromUtc(nextFireTimeUtc.Value.DateTime, brasiliaTimeZone);
                LogNextExecutionTime(context.JobDetail.Key.Name, nextExecutionTime);
            }

            LogJobExecution();

            _logger.LogInformation("Job executado com sucesso");
        }

        private void LogNextExecutionTime(string jobName, DateTime nextExecutionTime)
        {
            _logger.LogInformation($"Próxima execução do job {jobName}: {nextExecutionTime:dd/MM/yyyy HH:mm:ss}");
        }

        private void LogJobExecution()
        {
            _logger.LogInformation("Executando o job...");
            _logger.LogInformation($"Data e hora atual: {DateTime.Now}");
        }
    }
}