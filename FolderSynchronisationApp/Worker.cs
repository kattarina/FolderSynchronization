using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FolderSynchronisationApp
{
    public class Worker : BackgroundService 
    {
        private readonly ILogger<Worker> _logger; 
        public IServiceProvider Provider { get; set; }

        public IFolderSynchronizer _serviceRepository { get; set; }
        private readonly SettingConfig _settingConfig;


        public Worker(IServiceProvider provider, ILogger<Worker> logger, IOptions<SettingConfig> settingConfig, IFolderSynchronizer serviceRepository)
        {
            Provider = provider;
            _logger = logger;
             
            _serviceRepository =  Provider.GetService<IFolderSynchronizer>();
            _settingConfig = settingConfig.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now); 

                 
                int syncInterval = _settingConfig.SyncInterval;

                if (syncInterval<0)
                { 
                    
                    _logger.LogInformation($"SyncInterval was out of range, application is Shutting down");
                    Environment.Exit(0);
                }

                Task t = _serviceRepository.DoWork();


                await Task.Delay(syncInterval, stoppingToken);
            }
        }
    }
}
