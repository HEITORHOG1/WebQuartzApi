namespace WebQuartzApi.EndPoints
{
    /// <summary>
    /// classe de extensão para definição dos endpoints
    /// </summary>
    public static class EndpointRouteBuilderExtensions
    {
        /// <summary>
        /// metodo para mapear os endpoints de jobs
        /// </summary>
        /// <param name="app"></param>
        public static void MapJobEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/jobs", JobEndpoints.GetJobs)
                .WithTags("Jobs")
                .WithOpenApi()
                .WithDescription("Obtém todos os trabalhos registrados.");

            app.MapGet("/api/jobs/jobsexecution", JobEndpoints.GetRunningJobs)
                .WithTags("Jobs")
                .WithOpenApi()
                .WithDescription("Obtém os trabalhos em execução.");

            app.MapGet("/api/jobs/jobspaused", JobEndpoints.GetPausedJobs)
                .WithTags("Jobs")
                .WithOpenApi()
                .WithDescription("Obtém os trabalhos pausados.");

            app.MapPost("/api/jobs", JobEndpoints.CreateJob)
                .WithTags("Jobs")
                .WithOpenApi()
                .WithDescription("Cria um novo trabalho.");

            app.MapPost("/api/jobs/start", JobEndpoints.ResumeJob)
                .WithTags("Jobs")
                .WithOpenApi()
                .WithDescription("Inicia um trabalho.");

            app.MapPut("/api/jobs/pause", JobEndpoints.PauseJob)
                .WithTags("Jobs")
                .WithOpenApi()
                .WithDescription("Pausa um trabalho.");

            app.MapPut("/api/jobs/waiting", JobEndpoints.WaitingJob)
                .WithTags("Jobs")
                .WithOpenApi()
                .WithDescription("Coloca um trabalho em espera.");

            app.MapPut("/api/jobs/stop", JobEndpoints.StopJob)
                .WithTags("Jobs")
                .WithOpenApi()
                .WithDescription("Para a execução de um trabalho.");

            app.MapDelete("/api/jobs/{jobName}/{groupName}", JobEndpoints.DeleteJob)
                .WithTags("Jobs")
                .WithOpenApi()
                .WithDescription("Exclui um trabalho pelo nome e grupo.");
        }
    }
}