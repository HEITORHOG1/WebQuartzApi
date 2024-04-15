﻿using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl;
using System.Collections.Specialized;

namespace Work
{
    public static class SchedulerService
    {
        private static IScheduler _scheduler;
        private static IConfiguration _configuration;

        public static async Task<IScheduler> InitializeScheduler(IConfiguration configuration)
        {
            _configuration = configuration;

            if (_scheduler != null && !_scheduler.IsShutdown)
            {
                return _scheduler;
            }

            var props = new NameValueCollection
            {
                ["quartz.serializer.type"] = _configuration["Quartz:SerializerType"],
                ["quartz.scheduler.instanceName"] = _configuration["Quartz:InstanceName"],
                ["quartz.threadPool.type"] = _configuration["Quartz:ThreadPoolType"],
                ["quartz.threadPool.threadCount"] = _configuration["Quartz:ThreadPoolThreadCount"],
                ["quartz.jobStore.type"] = _configuration["Quartz:JobStoreType"],
                ["quartz.jobStore.driverDelegateType"] = _configuration["Quartz:DriverDelegateType"],
                ["quartz.jobStore.tablePrefix"] = _configuration["Quartz:TablePrefix"],
                ["quartz.jobStore.dataSource"] = _configuration["Quartz:DataSource"],
                ["quartz.dataSource.myDS.connectionString"] = _configuration["Quartz:ConnectionString"],
                ["quartz.dataSource.myDS.provider"] = _configuration["Quartz:DataSourceProvider"],
                ["quartz.jobStore.useProperties"] = _configuration["Quartz:UseProperties"],
                ["quartz.jobStore.performSchemaValidation"] = _configuration["Quartz:PerformSchemaValidation"]
            };

            var factory = new StdSchedulerFactory(props);
            _scheduler = await factory.GetScheduler();
            await _scheduler.Start();
            return _scheduler;
        }

        public static IScheduler GetScheduler()
        {
            return _scheduler;
        }
    }
}