using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl;
using System.Collections.Specialized;

namespace Work2
{
    public class SchedulerService : ISchedulerService
    {
        private IScheduler _scheduler;
        private readonly IConfiguration _configuration;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _instanceName;
        private readonly string _tablePrefix;

        public SchedulerService(
            ISchedulerFactory schedulerFactory,
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            string instanceName,
            string tablePrefix)
        {
            _schedulerFactory = schedulerFactory;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _instanceName = instanceName;
            _tablePrefix = tablePrefix;
        }

        public async Task<IScheduler> GetScheduler()
        {
            if (_scheduler == null || _scheduler.IsShutdown)
            {
                await InitializeScheduler();
            }
            return _scheduler;
        }

        public async Task InitializeScheduler()
        {
            var props = new NameValueCollection
            {
                ["quartz.serializer.type"] = _configuration["Quartz:SerializerType"],
                ["quartz.scheduler.instanceName"] = _instanceName,
                ["quartz.threadPool.type"] = _configuration["Quartz:quartz.threadPool.type"],
                ["quartz.threadPool.threadCount"] = _configuration.GetValue<string>("Quartz:ThreadPoolThreadCount"), // Assuming it's stored as a string
                ["quartz.jobStore.type"] = _configuration["Quartz:quartz.jobStore.type"],
                ["quartz.jobStore.driverDelegateType"] = _configuration["Quartz:quartz.jobStore.driverDelegateType"],
                ["quartz.jobStore.tablePrefix"] = _tablePrefix,
                ["quartz.jobStore.dataSource"] = "myDS",
                ["quartz.dataSource.myDS.connectionString"] = _configuration["Quartz:quartz.dataSource.myDS.connectionString"], // Corrected key
                ["quartz.dataSource.myDS.provider"] = _configuration["Quartz:quartz.dataSource.myDS.provider"],
                ["quartz.jobStore.useProperties"] = _configuration["Quartz:quartz.jobStore.useProperties"],
                ["quartz.jobStore.performSchemaValidation"] = _configuration["Quartz:quartz.jobStore.performSchemaValidation"],
                ["quartz.jobStore.clustered"] = _configuration["Quartz:quartz.jobStore.clustered"],
                ["quartz.jobStore.clusterCheckinInterval"] = _configuration["Quartz:quartz.jobStore.clusterCheckinInterval"]
            };

            var factory = new StdSchedulerFactory(props);
            _scheduler = await factory.GetScheduler();
            _scheduler.JobFactory = new DIJobFactory(_serviceProvider);
            await _scheduler.Start();
        }
    }
}