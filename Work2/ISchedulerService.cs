﻿using Microsoft.Extensions.Configuration;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work2
{
    public interface ISchedulerService
    {
        // Método para obter a instância do agendador
        IScheduler GetScheduler();
    }
}
