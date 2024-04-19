using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Work;
using Work.Extensions;

internal class Program
{
    private static IConfiguration configuration;
    private static ILogger logger;

    private static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((hostContext, services) =>
            {
                configuration = hostContext.Configuration;
                services.AddQuartzConfiguration(configuration);
                services.AddSingleton<ISchedulerService, SchedulerService>();
                services.AddLogging();
            })
            .Build();

        logger = host.Services.GetRequiredService<ILogger<Program>>();

        // Inicia o host para começar a executar o serviço
        await host.StartAsync();

        // Garante que o scheduler seja obtido e iniciado
        var schedulerService = host.Services.GetRequiredService<ISchedulerService>();
        var scheduler = await schedulerService.GetScheduler();

        // Agendar ou disparar jobs como necessário
        await ScheduleJob(host.Services);

        Console.WriteLine("Pressione [Enter] para sair...");
        Console.ReadLine();

        await host.StopAsync();
    }

    private static async Task ScheduleJob(IServiceProvider services)
    {
        var schedulerService = services.GetRequiredService<ISchedulerService>();
        var scheduler = await schedulerService.GetScheduler();

        await TriggerExistingJob(services, "two", "fps");
    }

    private static async Task TriggerExistingJob(IServiceProvider services, string jobName, string groupName)
    {
        var schedulerService = services.GetRequiredService<ISchedulerService>();
        var scheduler = await schedulerService.GetScheduler();
        var jobKey = new JobKey(jobName, groupName);

        // Primeiro, verifica se o job existe
        if (!await scheduler.CheckExists(jobKey))
        {
            Console.WriteLine($"Job {jobName} in group {groupName} does not exist.");
            return;
        }

        // Se existir, obter os triggers associados a esse job
        var triggers = await scheduler.GetTriggersOfJob(jobKey);

        if (triggers.Count == 0)
        {
            Console.WriteLine($"No triggers found for job: {jobName} in group: {groupName}");
            return;
        }

        foreach (var trigger in triggers)
        {
            var triggerState = await scheduler.GetTriggerState(trigger.Key);
            if (triggerState == TriggerState.Normal || triggerState == TriggerState.Complete)
            {
                Console.WriteLine($"Triggering job: {jobName} in group: {groupName}");
                await scheduler.TriggerJob(jobKey);
            }
            else
            {
                Console.WriteLine($"Job {jobName} in group {groupName} trigger state: {triggerState}");
                // Se o job estiver pausado, considere retomar dependendo de sua lógica de negócios
                if (triggerState == TriggerState.Paused)
                {
                    Console.WriteLine($"Resuming job: {jobName} in group: {groupName}");
                    await scheduler.ResumeJob(jobKey);
                    // E então, disparar o job
                    await scheduler.TriggerJob(jobKey);
                }
            }
        }
    }

    public class ExampleJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            // Lógica do job aqui
            Console.WriteLine("Job executed");
        }
    }

    //private static async Task TriggerJob(string jobName, string groupName, IServiceProvider services)
    //{
    //    var schedulerService = services.GetRequiredService<ISchedulerService>();
    //    var scheduler =  schedulerService.GetScheduler();

    //    try
    //    {
    //        var jobKey = new JobKey(jobName, groupName);
    //        if (await scheduler.CheckExists(jobKey))
    //        {
    //            var jobDetail = await scheduler.GetJobDetail(jobKey);
    //            if (jobDetail != null)
    //            {
    //                var jobDataMap = jobDetail.JobDataMap;
    //                jobDataMap.Put("IsExecuting", true);

    //                // Registrar uma mensagem de log informando que o job está sendo executado
    //                Console.WriteLine($"Job {jobName} in group {groupName} started executing.");

    //                await scheduler.TriggerJob(jobKey);

    //                // Registrar uma mensagem de log informando que o job foi concluído com sucesso
    //                Console.WriteLine($"Job {jobName} in group {groupName} executed successfully.");
    //            }
    //            else
    //            {
    //                Console.WriteLine($"Job {jobName} in group {groupName} does not exist.");
    //            }
    //        }
    //        else
    //        {
    //            Console.WriteLine($"Job {jobName} in group {groupName} does not exist.");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        // Registrar uma mensagem de log informando que ocorreu um erro durante a execução do job
    //        logger.LogError(ex, $"Error executing job {jobName} in group {groupName}.");

    //        Console.WriteLine(ex.Message);
    //    }
    //}
}