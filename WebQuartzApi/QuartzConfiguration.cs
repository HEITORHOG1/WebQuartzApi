using Quartz;

namespace WebQuartzApi
{
    /// <summary>
    /// classe de configuração do Quartz
    /// </summary>
    public static class QuartzConfiguration
    {
        /// <summary>
        /// serviço de configuração do Quartz
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddConfigureQuartz(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddQuartz(q =>
            {
                // Utiliza a fábrica de jobs que permite a injeção de dependência do .NET
                q.UseMicrosoftDependencyInjectionJobFactory(options =>
                {
                    // Permite construtores padrão se nenhum outro construtor está disponível
                    options.AllowDefaultConstructor = true;
                });

                // Configura o armazenamento persistente com SQL Server
                q.UsePersistentStore(store =>
                {
                    // Utiliza propriedades ao invés de serialização binária
                    store.UseProperties = true;
                    // Configura a conexão do SQL Server usando a connection string definida
                    store.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                    // Utiliza serialização JSON para os dados de job
                    store.UseJsonSerializer();
                    // Desativa a validação do esquema, útil durante o desenvolvimento
                    store.PerformSchemaValidation = false;
                });
            });

            // Registra o serviço do Quartz como um serviço hospedado, para iniciar e parar com a aplicação
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            // Registra o scheduler como um singleton para facilitar a injeção e uso em toda a aplicação
            services.AddSingleton(provider => provider.GetRequiredService<ISchedulerFactory>().GetScheduler().Result);

            return services;
        }
    }
}