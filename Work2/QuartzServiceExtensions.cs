using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work2
{
    public static class QuartzServiceExtensions
    {
        public static void AddQuartzConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ISchedulerService, SchedulerService>();
        }
    }
}
