﻿using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
using System.Data;
using WebQuartzApi.Model;

namespace WebQuartzApi.EndPoints
{
    /// <summary>
    /// metodos para definição dos endpoints de jobs
    /// </summary>
    public static class JobEndpoints
    {
        /// <summary>
        /// metodo para obter os jobs
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<IResult> GetJobs([FromServices] IScheduler scheduler)
        {
            var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
            var jobs = new List<JobRequest>();
            foreach (var jobKey in jobKeys)
            {
                try
                {
                    var detail = await scheduler.GetJobDetail(jobKey);
                    if (detail != null)
                    {
                        var cronExpressionString = string.Empty;
                        var jobTriggers = await scheduler.GetTriggersOfJob(jobKey);

                        foreach (var trigger in jobTriggers)
                        {
                            if (trigger is CronTriggerImpl cronTrigger)
                            {
                                cronExpressionString = cronTrigger.CronExpressionString;
                                break;
                            }
                        }

                        jobs.Add(new JobRequest
                        {
                            JobName = detail.Key.Name,
                            GroupName = detail.Key.Group,
                            Description = detail.Description,
                            CronExpression = cronExpressionString
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving job '{jobKey}': {ex.GetType().FullName}: {ex.Message}");
                }
            }
            return Results.Ok(jobs);
        }



        /// <summary>
        /// metodo para deletar um job
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<IResult> DeleteJob([FromServices] IScheduler scheduler, HttpContext context)
        {
            var jobName = context.Request.RouteValues["jobName"]?.ToString();
            var groupName = context.Request.RouteValues["groupName"]?.ToString();
            if (string.IsNullOrWhiteSpace(jobName) || string.IsNullOrWhiteSpace(groupName))
            {
                return Results.BadRequest(new { Message = "Job name or group name is not specified." });
            }

            var jobKey = new JobKey(jobName, groupName);
            var result = await scheduler.DeleteJob(jobKey);
            if (result)
            {
                return Results.Ok(new { Message = "Job deleted successfully." });
            }
            else
            {
                return Results.NotFound(new { Message = "Job not found." });
            }
        }

        /// <summary>
        /// Metodo para pausar um job.
        /// </summary>
        /// <param name="scheduler">O agendador do Quartz.</param>
        /// <param name="request">Os dados do job que está sendo pausado.</param>
        /// <returns>Um resultado da ação de pausar o job.</returns>
        public static async Task<IResult> PauseJob([FromServices] IScheduler scheduler, JobPausedRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.JobName) || string.IsNullOrWhiteSpace(request.GroupName))
            {
                return Results.BadRequest(new { Message = "Job name or group name is not specified." });
            }

            var jobKey = new JobKey(request.JobName, request.GroupName);
            try
            {
                await scheduler.PauseJob(jobKey);
                return Results.Ok(new { Message = "Job paused successfully." });
            }
            catch (SchedulerException ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        /// <summary>
        /// metodo para colocar waiting um job
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<IResult> WaitingJob([FromServices] IScheduler scheduler, JobResumedRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.JobName) || string.IsNullOrWhiteSpace(request.GroupName))
            {
                return Results.BadRequest(new { Message = "Job name or group name is not specified." });
            }

            var jobKey = new JobKey(request.JobName, request.GroupName);
            try
            {
                await scheduler.ResumeJob(jobKey);
                return Results.Ok(new { Message = "Job resumed successfully." });
            }
            catch (SchedulerException ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        /// <summary>
        /// metodo para obter os jobs pausados
        /// </summary>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public static async Task<IResult> GetPausedJobs([FromServices] IScheduler scheduler)
        {
            var pausedJobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
            var pausedJobs = new List<JobRequest>();
            foreach (var jobKey in pausedJobKeys)
            {
                var jobDetail = await scheduler.GetJobDetail(jobKey);
                var jobTriggers = await scheduler.GetTriggersOfJob(jobKey);
                var isJobPaused = jobTriggers.Any(trigger => trigger.GetNextFireTimeUtc() == null);

                if (isJobPaused)
                {
                    var jobDataMap = jobDetail.JobDataMap;
                    var cronExpressionString = string.Empty;

                    foreach (var trigger in jobTriggers)
                    {
                        if (trigger is CronTriggerImpl cronTrigger)
                        {
                            cronExpressionString = cronTrigger.CronExpressionString;
                            break;
                        }
                    }

                    var pausedJob = new JobRequest
                    {
                        JobName = jobDetail.Key.Name,
                        GroupName = jobDetail.Key.Group,
                        Description = jobDetail.Description,
                        CronExpression = cronExpressionString
                    };
                    pausedJobs.Add(pausedJob);
                }
            }
            return Results.Ok(pausedJobs);
        }

        /// <summary>
        /// Metodo para parar um job.
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<IResult> StopJob([FromServices] IScheduler scheduler, JobStoppedRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.JobName) || string.IsNullOrWhiteSpace(request.GroupName))
            {
                return Results.BadRequest(new { Message = "Job name or group name is not specified." });
            }

            var jobKey = new JobKey(request.JobName, request.GroupName);
            try
            {
                await scheduler.Interrupt(jobKey);
                return Results.Ok(new { Message = "Job stopped successfully." });
            }
            catch (SchedulerException ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        /// <summary>
        /// metodo para obter os jobs em execução
        /// </summary>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public static async Task<IResult> GetRunningJobs([FromServices] IScheduler scheduler)
        {
            var runningJobs = new List<JobRequest>();
            var jobGroups = await scheduler.GetJobGroupNames();
            foreach (var group in jobGroups)
            {
                var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(group));
                foreach (var jobKey in jobKeys)
                {
                    var jobDetail = await scheduler.GetJobDetail(jobKey);
                    var jobTriggers = await scheduler.GetTriggersOfJob(jobKey);
                    var isJobRunning = jobTriggers.Any(trigger => trigger.GetNextFireTimeUtc() != null);
                    var cronExpressionString = string.Empty;

                    foreach (var trigger in jobTriggers)
                    {
                        if (trigger is CronTriggerImpl cronTrigger)
                        {
                            cronExpressionString = cronTrigger.CronExpressionString;
                            break;
                        }
                    }

                    if (isJobRunning)
                    {
                        var runningJob = new JobRequest
                        {
                            JobName = jobDetail.Key.Name,
                            GroupName = jobDetail.Key.Group,
                            Description = jobDetail.Description,
                            CronExpression = cronExpressionString
                        };
                        runningJobs.Add(runningJob);
                    }
                }
            }
            return Results.Ok(runningJobs);
        }

        /// <summary>
        /// metodo para iniciar um job
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<IResult> StartJob([FromServices] IScheduler scheduler, JobStartedRequest request)
        {
            var jobKey = new JobKey(request.JobName, request.GroupName); // WorkerId representa a identificação única do work
            var triggerKey = new TriggerKey($"{request.JobName}-trigger", request.GroupName);

            if (!await scheduler.CheckExists(jobKey))
            {
                return Results.BadRequest(new { Message = "Job does not exist." });
            }

            // Verifique o estado do trigger
            var triggerState = await scheduler.GetTriggerState(triggerKey);
            if (triggerState == TriggerState.Paused)
            {
                // Resume o job se estiver pausado
                await scheduler.ResumeJob(jobKey);
                return Results.Ok(new { Message = "Job resumed and will be triggered according to its schedule." });
            }
            else if (triggerState == TriggerState.Normal)
            {
                // Se o job estiver pronto para ser executado e não estiver bloqueado, dispare-o imediatamente
                await scheduler.TriggerJob(jobKey);
                return Results.Ok(new { Message = "Job triggered successfully." });
            }
            else
            {
                return Results.Conflict(new { Message = "Job is currently running, blocked, or in an unknown state." });
            }
        }

        private static async Task<string> GetCronExpressionFromDatabase(string jobName, string groupName)
        {
            using (var dbConnection = GetDbConnection())
            {
                var sql = "SELECT CRON_EXPRESSION FROM QRTZ_CRON_TRIGGERS WHERE TRIGGER_NAME = @TriggerName AND TRIGGER_GROUP = @TriggerGroup";
                var parameters = new { TriggerName = jobName, TriggerGroup = groupName };
                var cronExpression = await dbConnection.QueryFirstOrDefaultAsync<string>(sql, parameters);
                return cronExpression;
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