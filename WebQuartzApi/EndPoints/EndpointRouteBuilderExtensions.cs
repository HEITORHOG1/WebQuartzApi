namespace WebQuartzApi.EndPoints
{
    /// <summary>
    /// classe de extensão para definição dos endpoints
    /// </summary>
    public static class EndpointRouteBuilderExtensions
    {
        /// <summary>
        /// map dos endpoints de works
        /// </summary>
        /// <param name="app"></param>
        public static void MapJobEndpoints(this IEndpointRouteBuilder app)
        {
            var jobEndpoints = new JobEndpoints(new Logger<JobEndpoints>(new LoggerFactory()));

            app.MapGet("/api/jobs", jobEndpoints.GetJobs)
                .WithTags("Jobs")
                .WithOpenApi()
                .WithDescription("Obtém todos os trabalhos registrados.");

            app.MapGet("/api/jobs/{jobName}/{groupName}", jobEndpoints.GetJobDetail)
                .WithTags("Jobs")
                .WithOpenApi()
                .WithDescription("Obtém os detalhes de um trabalho específico.");

            app.MapPut("/api/jobs/pause/{triggerName}/{groupName}", jobEndpoints.PauseJob)
                .WithTags("Jobs")
                .WithOpenApi()
                .WithDescription("Pausa um trabalho.");

            app.MapPut("/api/jobs/waiting/{triggerName}/{groupName}", jobEndpoints.WaitingJob)
                .WithTags("Jobs")
                .WithOpenApi()
                .WithDescription("Coloca um trabalho em espera.");

            app.MapGet("/api/jobs/paused", jobEndpoints.GetPausedJobs)
                .WithTags("Jobs")
                .WithOpenApi()
                .WithDescription("Obtém os trabalhos pausados.");

            app.MapPut("/api/jobs/stop/{triggerName}/{groupName}", jobEndpoints.StopJob)
                .WithTags("Jobs")
                .WithOpenApi()
                .WithDescription("Para a execução de um trabalho.");

            app.MapGet("/api/jobs/running", jobEndpoints.GetRunningJobs)
                .WithTags("Jobs")
                .WithOpenApi()
                .WithDescription("Obtém os trabalhos em execução.");

            app.MapPut("/api/jobs/start/{triggerName}/{groupName}", jobEndpoints.StartJob)
                .WithTags("Jobs")
                .WithOpenApi()
                .WithDescription("Inicia um trabalho.");

            app.MapGet("/api/jobs/exists/{jobName}/{groupName}", jobEndpoints.CheckExists)
                .WithTags("Jobs")
                .WithOpenApi()
                .WithDescription("Verifica se um trabalho existe.");

            app.MapPut("/api/jobs/resume/{triggerName}/{groupName}", jobEndpoints.ResumeJob)
                .WithTags("Jobs")
                .WithOpenApi()
                .WithDescription("Retoma um trabalho.");

            app.MapDelete("/api/jobs/{jobName}/{groupName}", jobEndpoints.DeleteJob)
                .WithTags("Jobs")
                .WithOpenApi()
                .WithDescription("Deleta um job.");

            app.MapGet("/api/jobs/{jobName}/{groupName}/triggers", jobEndpoints.GetTriggersOfJob)
                .WithTags("Jobs")
                .WithOpenApi()
                .WithDescription("Obtém os triggers de um job específico.");
        }
    }
}