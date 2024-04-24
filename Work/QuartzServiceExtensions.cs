using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Spi;

namespace Work
{
    public static class QuartzServiceExtensions
    {
        public static void AddQuartzConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddQuartz(q =>
            {
                q.SchedulerId = "Scheduler-Work1";
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
            services.AddSingleton<IJobFactory, DIJobWork1Factory>();
            services.AddTransient<WorkJJob>();
            services.AddSingleton<IJobFactory>(provider => new DIJobWork1Factory(provider));
            services.AddSingleton<ISchedulerService>(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                var schedulerFactory = provider.GetRequiredService<ISchedulerFactory>();
                var serviceProvider = provider; // Directly use 'provider' which is IServiceProvider
                var instanceName = "Scheduler-Work1"; // Custom instance name
                var tablePrefix = "QRTZ_"; // Custom table prefix
                return new SchedulerService(schedulerFactory, config, serviceProvider, instanceName, tablePrefix);
            });

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
            services.Configure<QuartzOptions>(options =>
            {
                options.Scheduling.IgnoreDuplicates = true; // default: false
                options.Scheduling.OverWriteExistingData = true; // default: true
            });
            // Configuração opcional para adicionar log detalhado
            services.AddLogging(configure => configure.AddConsole())
                .Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);
        }
    }
}