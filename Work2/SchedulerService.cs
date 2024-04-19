using Microsoft.Extensions.Configuration;
using Quartz.Impl;
using Quartz.Spi;
using Quartz;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work2
{
    public class SchedulerService : ISchedulerService
    {
        private readonly IScheduler _scheduler;

        public SchedulerService(ISchedulerFactory schedulerFactory, IJobFactory jobFactory, IConfiguration configuration)
        {
            var props = new NameValueCollection
            {
                ["quartz.serializer.type"] = configuration["Quartz:quartz.serializer.type"],
                ["quartz.scheduler.instanceName"] = configuration["Quartz:quartz.scheduler.instanceName"],
                ["quartz.threadPool.type"] = configuration["Quartz:quartz.threadPool.type"],
                ["quartz.threadPool.threadCount"] = configuration["Quartz:quartz.threadPool.threadCount"],
                ["quartz.jobStore.misfireThreshold"] = configuration["Quartz:quartz.jobStore.misfireThreshold"],
                ["quartz.jobStore.type"] = configuration["Quartz:quartz.jobStore.type"],
                ["quartz.jobStore.useProperties"] = configuration["Quartz:quartz.jobStore.useProperties"],
                ["quartz.jobStore.dataSource"] = configuration["Quartz:quartz.jobStore.dataSource"],
                ["quartz.jobStore.tablePrefix"] = configuration["Quartz:quartz.jobStore.tablePrefix"],
                ["quartz.dataSource.myDS.connectionString"] = configuration.GetConnectionString("DefaultConnection"),
                ["quartz.dataSource.myDS.provider"] = configuration["Quartz:quartz.dataSource.myDS.provider"]
            };

            schedulerFactory = new StdSchedulerFactory(props);
            _scheduler = schedulerFactory.GetScheduler().Result;
            _scheduler.JobFactory = jobFactory;
            _scheduler.Start().Wait();
        }

        public IScheduler GetScheduler()
        {
            return _scheduler;
        }
    }
}
