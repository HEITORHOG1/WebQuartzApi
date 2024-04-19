using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Work2;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        // Inicializar o serviço do agendador Quartz
        var schedulerService = host.Services.GetService<ISchedulerService>();
        var scheduler = schedulerService.GetScheduler();

        // Disparar o job "ONE" do grupo "FPS"
        await TriggerJob(scheduler, "ONE", "FPS");

        // Iniciar o host
        await host.RunAsync();
    }

    private static async Task TriggerJob(IScheduler scheduler, string jobName, string groupName)
    {
        var jobKey = new JobKey(jobName, groupName);
        if (await scheduler.CheckExists(jobKey))
        {
            await scheduler.TriggerJob(jobKey);
            Console.WriteLine($"Job '{jobName}' in group '{groupName}' was triggered successfully.");
        }
        else
        {
            Console.WriteLine($"Job '{jobName}' in group '{groupName}' does not exist.");
        }
    }

    // Aqui está a definição do método CreateHostBuilder
    private static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>(); // Registra o ISchedulerFactory padrão do Quartz
            services.AddSingleton<IJobFactory, MyJobFactory>(); // Registra a sua implementação de IJobFactory
            services.AddSingleton<ISchedulerService, SchedulerService>(); // Registra o seu serviço de agendador personalizado
            // Registra os jobs como serviços para que a MyJobFactory possa criá-los.
            services.AddTransient<MyJobFactory>(); // Substitua "YourJobClass" pela classe real do seu job.
            // Outras configurações e serviços...
        });

}