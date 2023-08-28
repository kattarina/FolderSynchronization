using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchronisationApp
{
    public class ServiceJob : IJob
    {
        public IServiceProvider Provider { get; set; }

        public IServiceRepository _serviceRepository { get; set; }
        private readonly ILogger<ServiceJob> _logger;

        public ServiceJob(IServiceProvider provider,
                          ILogger<ServiceJob> logger,
                          IServiceRepository serviceRepository
                          )
        {

            Provider = provider;
            _logger = logger;
            _serviceRepository = Provider.GetService<IServiceRepository>();
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"Job Started at : {DateTime.Now}");

            Task t = _serviceRepository.DoWork();



            _logger.LogInformation($"Job Completed at : {DateTime.Now}");

            return Task.CompletedTask;
        }
    }
}
