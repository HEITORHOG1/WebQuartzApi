using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace Work.Extensions
{
    public static class QuartzServiceExtensions
    {
        public static void AddQuartzConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddQuartz(q =>
            {
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
            // Adicione o serviço IScheduler ao contêiner de serviços

            services.AddSingleton<IHostedService, QuartzHostedService>();
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        }
    }
}