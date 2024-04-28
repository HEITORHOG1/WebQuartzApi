using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using WebQuartzApi.Model.Request;
using WebQuartzApi.Model.Response;

namespace WebQuartzApi.EndPoints
{
    /// <summary>
    /// metodos para definição dos endpoints de jobs
    /// </summary>
    public class JobEndpoints
    {
        private readonly ILogger<JobEndpoints> _logger;

        public JobEndpoints(ILogger<JobEndpoints> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// metodo para obter os jobs
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<IEnumerable<GetJobsResponse>> GetJobs()
        {
            _logger.LogInformation("Obtendo todos os jobs");
            try
            {
                using (var connection = GetDbConnection())
                {
                    var jobs = await connection.QueryAsync<GetJobsResponse>(@"SELECT SCHED_NAME, JOB_NAME, JOB_GROUP, DESCRIPTION, JOB_CLASS_NAME, IS_DURABLE, IS_NONCONCURRENT, IS_UPDATE_DATA, REQUESTS_RECOVERY FROM [dbo].[QRTZ_JOB_DETAILS]");
                    _logger.LogInformation($"Encontrados {jobs.Count()} jobs");
                    return jobs;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao obter todos os jobs");
                throw;
            }
        }

        public async Task<GetJobDetailResponse> GetJobDetail([FromBody] GetJobDetailRequest request)
        {
            _logger.LogInformation($"Obtendo detalhes do job {request.JobName} no grupo {request.GroupName}");
            try
            {
                using (var connection = GetDbConnection())
                {
                    var sql = @"SELECT SCHED_NAME, JOB_NAME, JOB_GROUP, DESCRIPTION, JOB_CLASS_NAME, IS_DURABLE, IS_NONCONCURRENT, IS_UPDATE_DATA, REQUESTS_RECOVERY, JOB_DATA FROM [dbo].[QRTZ_JOB_DETAILS] WHERE JOB_NAME = @JobName AND JOB_GROUP = @JobGroup";
                    var parameters = new { JobName = request.JobName, JobGroup = request.GroupName };
                    var job = await connection.QuerySingleOrDefaultAsync<GetJobDetailResponse>(sql, parameters);
                    if (job != null)
                    {
                        _logger.LogInformation($"Job {request.JobName} no grupo {request.GroupName} encontrado");
                        return job;
                    }
                    else
                    {
                        _logger.LogWarning($"Job {request.JobName} no grupo {request.GroupName} não encontrado");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao obter o job {request.JobName} no grupo {request.GroupName}");
                throw;
            }
        }

        /// <summary>
        /// Metodo para pausar um job.
        /// </summary>
        /// <param name="scheduler">O agendador do Quartz.</param>
        /// <param name="request">Os dados do job que está sendo pausado.</param>
        /// <returns>Um resultado da ação de pausar o job.</returns>
        public async Task<PauseJobResponse> PauseJob([FromBody] PauseJobRequest request)
        {
            _logger.LogInformation($"Pausando job {request.TriggerName} no grupo {request.GroupName}");
            try
            {
                using (var connection = GetDbConnection())
                {
                    var sql = "UPDATE [dbo].[QRTZ_TRIGGERS] SET TRIGGER_STATE = 'PAUSED' WHERE TRIGGER_NAME = @TriggerName AND TRIGGER_GROUP = @TriggerGroup";
                    var parameters = new { TriggerName = request.TriggerName, TriggerGroup = request.GroupName };
                    var result = await connection.ExecuteAsync(sql, parameters);
                    if (result > 0)
                    {
                        _logger.LogInformation($"Job {request.TriggerName} no grupo {request.GroupName} pausado com sucesso");
                        return new PauseJobResponse { IsPaused = true };
                    }
                    else
                    {
                        _logger.LogWarning($"Job {request.TriggerName} no grupo {request.GroupName} não encontrado");
                        return new PauseJobResponse { IsPaused = false };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao pausar o job {request.TriggerName} no grupo {request.GroupName}");
                throw;
            }
        }

        /// <summary>
        /// metodo para colocar waiting um job
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<WaitingJobResponse> WaitingJob([FromBody] WaitingJobRequest request)
        {
            _logger.LogInformation($"Colocando job {request.TriggerName} no grupo {request.GroupName} em espera");
            try
            {
                using (var connection = GetDbConnection())
                {
                    var sql = "UPDATE [dbo].[QRTZ_TRIGGERS] SET TRIGGER_STATE = 'WAITING' WHERE TRIGGER_NAME = @TriggerName AND TRIGGER_GROUP = @TriggerGroup";
                    var parameters = new { TriggerName = request.TriggerName, TriggerGroup = request.GroupName };
                    var result = await connection.ExecuteAsync(sql, parameters);
                    if (result > 0)
                    {
                        _logger.LogInformation($"Job {request.TriggerName} no grupo {request.GroupName} colocado em espera com sucesso");
                        return new WaitingJobResponse { IsWaiting = true };
                    }
                    else
                    {
                        _logger.LogWarning($"Job {request.TriggerName} no grupo {request.GroupName} não encontrado");
                        return new WaitingJobResponse { IsWaiting = false };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao colocar o job {request.TriggerName} no grupo {request.GroupName} em espera");
                throw;
            }
        }

        /// <summary>
        /// metodo para obter os jobs pausados
        /// </summary>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public async Task<IEnumerable<GetPausedJobsResponse>> GetPausedJobs()
        {
            _logger.LogInformation("Obtendo todos os jobs pausados");
            try
            {
                using (var connection = GetDbConnection())
                {
                    var jobs = await connection.QueryAsync<GetPausedJobsResponse>(@"SELECT SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP, JOB_NAME, JOB_GROUP, TRIGGER_STATE FROM [dbo].[QRTZ_TRIGGERS] WHERE TRIGGER_STATE = 'PAUSED'");
                    _logger.LogInformation($"Encontrados {jobs.Count()} jobs pausados");
                    return jobs;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao obter todos os jobs pausados");
                throw;
            }
        }

        /// <summary>
        /// Metodo para parar um job.
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<StopJobResponse> StopJob([FromBody] StopJobRequest request)
        {
            _logger.LogInformation($"Parando job {request.TriggerName} no grupo {request.GroupName}");
            try
            {
                using (var connection = GetDbConnection())
                {
                    var sql = "UPDATE [dbo].[QRTZ_TRIGGERS] SET TRIGGER_STATE = 'COMPLETE' WHERE TRIGGER_NAME = @TriggerName AND TRIGGER_GROUP = @TriggerGroup";
                    var parameters = new { TriggerName = request.TriggerName, TriggerGroup = request.GroupName };
                    var result = await connection.ExecuteAsync(sql, parameters);
                    if (result > 0)
                    {
                        _logger.LogInformation($"Job {request.TriggerName} no grupo {request.GroupName} parado com sucesso");
                        return new StopJobResponse { IsStopped = true };
                    }
                    else
                    {
                        _logger.LogWarning($"Job {request.TriggerName} no grupo {request.GroupName} não encontrado");
                        return new StopJobResponse { IsStopped = false };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao parar o job {request.TriggerName} no grupo {request.GroupName}");
                throw;
            }
        }

        /// <summary>
        /// metodo para obter os jobs em execução
        /// </summary>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public async Task<IEnumerable<GetRunningJobsResponse>> GetRunningJobs()
        {
            _logger.LogInformation("Obtendo todos os jobs em execução");
            try
            {
                using (var connection = GetDbConnection())
                {
                    var jobs = await connection.QueryAsync<GetRunningJobsResponse>(@"SELECT SCHED_NAME, TRIGGER_NAME, TRIGGER_GROUP, JOB_NAME, JOB_GROUP, TRIGGER_STATE FROM [dbo].[QRTZ_TRIGGERS] WHERE TRIGGER_STATE = 'WAITING'");
                    _logger.LogInformation($"Encontrados {jobs.Count()} jobs em execução");
                    return jobs;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao obter todos os jobs em execução");
                throw;
            }
        }

        /// <summary>
        /// metodo para iniciar um job
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<StartJobResponse> StartJob([FromBody] StartJobRequest request)
        {
            _logger.LogInformation($"Iniciando job {request.TriggerName} no grupo {request.GroupName}");
            try
            {
                using (var connection = GetDbConnection())
                {
                    var sql = "UPDATE [dbo].[QRTZ_TRIGGERS] SET TRIGGER_STATE = 'EXECUTING' WHERE TRIGGER_NAME = @TriggerName AND TRIGGER_GROUP = @TriggerGroup";
                    var parameters = new { TriggerName = request.TriggerName, TriggerGroup = request.GroupName };
                    var result = await connection.ExecuteAsync(sql, parameters);
                    if (result > 0)
                    {
                        _logger.LogInformation($"Job {request.TriggerName} no grupo {request.GroupName} iniciado com sucesso");
                        return new StartJobResponse { IsStarted = true };
                    }
                    else
                    {
                        _logger.LogWarning($"Job {request.TriggerName} no grupo {request.GroupName} não encontrado");
                        return new StartJobResponse { IsStarted = false };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao iniciar o job {request.TriggerName} no grupo {request.GroupName}");
                throw;
            }
        }

        public async Task<ResumeJobResponse> ResumeJob([FromBody] ResumeJobRequest request)
        {
            _logger.LogInformation($"Resumindo job {request.TriggerName} no grupo {request.GroupName}");
            try
            {
                using (var connection = GetDbConnection())
                {
                    var sql = "UPDATE [dbo].[QRTZ_TRIGGERS] SET TRIGGER_STATE = 'WAITING' WHERE TRIGGER_NAME = @TriggerName AND TRIGGER_GROUP = @TriggerGroup";
                    var parameters = new { TriggerName = request.TriggerName, TriggerGroup = request.GroupName };
                    var result = await connection.ExecuteAsync(sql, parameters);
                    if (result > 0)
                    {
                        _logger.LogInformation($"Job {request.TriggerName} no grupo {request.GroupName} retomado com sucesso");
                        return new ResumeJobResponse { IsResumed = true };
                    }
                    else
                    {
                        _logger.LogWarning($"Job {request.TriggerName} no grupo {request.GroupName} não encontrado");
                        return new ResumeJobResponse { IsResumed = false };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao retomar o job {request.TriggerName} no grupo {request.GroupName}");
                throw;
            }
        }

        public async Task<CheckExistsResponse> CheckExists([FromBody] CheckExistsRequest request)
        {
            _logger.LogInformation($"Verificando se o job {request.JobName} no grupo {request.GroupName} existe");
            try
            {
                using (var connection = GetDbConnection())
                {
                    var sql = "SELECT COUNT(*) FROM [dbo].[QRTZ_JOB_DETAILS] WHERE JOB_NAME = @JobName AND JOB_GROUP = @JobGroup";
                    var parameters = new { JobName = request.JobName, JobGroup = request.GroupName };
                    var count = await connection.ExecuteScalarAsync<int>(sql, parameters);
                    if (count > 0)
                    {
                        _logger.LogInformation($"Job {request.JobName} no grupo {request.GroupName} existe");
                        return new CheckExistsResponse { Exists = true };
                    }
                    else
                    {
                        _logger.LogInformation($"Job {request.JobName} no grupo {request.GroupName} não existe");
                        return new CheckExistsResponse { Exists = false };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao verificar se o job {request.JobName} no grupo {request.GroupName} existe");
                throw;
            }
        }

        public async Task<DeleteJobResponse> DeleteJob([FromBody] DeleteJobRequest request)
        {
            _logger.LogInformation($"Deletando job {request.JobName} no grupo {request.GroupName}");
            try
            {
                using (var connection = GetDbConnection())
                {
                    var sql = "DELETE FROM [dbo].[QRTZ_JOB_DETAILS] WHERE JOB_NAME = @JobName AND JOB_GROUP = @JobGroup";
                    var parameters = new { JobName = request.JobName, JobGroup = request.GroupName };
                    var result = await connection.ExecuteAsync(sql, parameters);
                    if (result > 0)
                    {
                        _logger.LogInformation($"Job {request.JobName} no grupo {request.GroupName} deletado com sucesso");
                        return new DeleteJobResponse { IsDeleted = true };
                    }
                    else
                    {
                        _logger.LogWarning($"Job {request.JobName} no grupo {request.GroupName} não encontrado");
                        return new DeleteJobResponse { IsDeleted = false };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao deletar o job {request.JobName} no grupo {request.GroupName}");
                throw;
            }
        }

        public async Task<IEnumerable<GetTriggersOfJobResponse>> GetTriggersOfJob([FromBody] GetTriggersOfJobRequest request)
        {
            _logger.LogInformation($"Obtendo triggers do job {request.JobName} no grupo {request.GroupName}");
            try
            {
                using (var connection = GetDbConnection())
                {
                    var sql = @"SELECT TRIGGER_NAME, TRIGGER_GROUP, TRIGGER_STATE FROM [dbo].[QRTZ_TRIGGERS] WHERE JOB_NAME = @JobName AND JOB_GROUP = @JobGroup";
                    var parameters = new { JobName = request.JobName, JobGroup = request.GroupName };
                    var triggers = await connection.QueryAsync<GetTriggersOfJobResponse>(sql, parameters);
                    _logger.LogInformation($"Encontrados {triggers.Count()} triggers para o job {request.JobName} no grupo {request.GroupName}");
                    return triggers;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ocorreu um erro ao obter os triggers do job {request.JobName} no grupo {request.GroupName}");
                throw;
            }
        }

        private static IDbConnection GetDbConnection()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            return new SqlConnection(connectionString);
        }
    }
}