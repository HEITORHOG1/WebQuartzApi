using Microsoft.Extensions.DependencyInjection;
using Quartz.Spi;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work2
{
    public class MyJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public MyJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        // Chamado pelo agendador no momento de executar um job.
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;
            var jobType = jobDetail.JobType;
            try
            {
                // Retorna uma instância do job, solicitando-a do contêiner de DI.
                return _serviceProvider.GetRequiredService(jobType) as IJob;
            }
            catch (Exception ex)
            {
                throw new SchedulerException($"Problema ao instanciar o job '{jobDetail.Key}' do tipo '{jobType.FullName}'.", ex);
            }
        }

        // Chamado pelo agendador quando um job é terminado e descartado.
        public void ReturnJob(IJob job)
        {
            // O contêiner de DI cuida do ciclo de vida do job, então não é necessário fazer nada aqui.
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}
