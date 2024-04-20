using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace Work
{
    public static class QuartzServiceExtensions
    {
        public static void AddQuartzConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddQuartz(q =>
            {
                q.SchedulerId = "Scheduler-Work";
                q.UseMicrosoftDependencyInjectionJobFactory();
                q.UseSimpleTypeLoader();
                q.UsePersistentStore(store =>
                {
                    store.UseProperties = configuration.GetValue<bool>("Quartz:quartz.jobStore.useProperties");
                    store.UseSqlServer(configuration["Quartz:quartz.dataSource.myDS.connectionString"]);
                    store.UseJsonSerializer();
                    store.UseClustering(c =>
                    {
                        c.CheckinInterval = TimeSpan.FromSeconds(20);
                        c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
                    });
                });
                q.UseDefaultThreadPool(tp => tp.MaxConcurrency = configuration.GetValue<int>("Quartz:quartz.threadPool.threadCount"));
            });

            // Configure o QuartzHostedService com opções apropriadas
            services.AddSingleton<ISchedulerService, SchedulerService>();
            services.AddSingleton<IHostedService, QuartzHostedService>((serviceProvider) =>
            {
                var lifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();
                var schedulerFactory = serviceProvider.GetRequiredService<ISchedulerFactory>();
                var options = new QuartzHostedServiceOptions
                {
                    WaitForJobsToComplete = true
                };
                return new QuartzHostedService(lifetime, schedulerFactory, Options.Create(options));
            });

            // Configuração opcional para adicionar log detalhado
            services.AddLogging(configure => configure.AddConsole())
                .Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);
        }
    }
}