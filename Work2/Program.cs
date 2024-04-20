using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl.Matchers;
using Work2;

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

        var jobKey = new JobKey("Work1", "Work1");

        // Verifica se o job já existe
        if (!await scheduler.CheckExists(jobKey))
        {
            var job = JobBuilder.Create<Work1Job>()
                .WithIdentity(jobKey)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("Work1", "Work1")
                .WithCronSchedule("0 0/5 * ? * * *") // A cada cinco minutos
                .ForJob(jobKey)
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
        else
        {
            // Se o job já existe, você pode decidir atualizar o trigger ou simplesmente logar que o job já está configurado
            logger.LogInformation("Job already exists and is scheduled.");
        }

        await StartSchedulerWithListener(host.Services);

        Console.WriteLine("Pressione [Enter] para sair...");
        Console.ReadLine();

        await host.StopAsync();
    }

    private static async Task StartSchedulerWithListener(IServiceProvider services)
    {
        var schedulerService = services.GetRequiredService<ISchedulerService>();
        var scheduler = await schedulerService.GetScheduler();

        // Adicionando o listener ao scheduler
        var jobListener = new Work1JobListener();
        scheduler.ListenerManager.AddJobListener(jobListener, EverythingMatcher<JobKey>.AllJobs());

        await scheduler.Start();
    }
}