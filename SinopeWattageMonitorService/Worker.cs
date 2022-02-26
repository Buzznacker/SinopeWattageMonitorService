using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SinopeWattageMonitorService.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SinopeWattageMonitorService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly List<int> _lastWattages = new();

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var firstRun = true;
            var apiClient = new ApiClient("https://neviweb.com", _logger);         

            while (!stoppingToken.IsCancellationRequested)
            {
                if(firstRun)
                {
                    firstRun = false;
                } else
                {
                    try
                    {
                        var wattage = apiClient.GetWattage();
                        _logger.LogInformation($"wattage is: {wattage}");

                        _lastWattages.Add(wattage);

                        if (_lastWattages.Count >= 3)
                        {
                            var countBad = 0;
                            foreach(var watt in _lastWattages)
                            {
                                if(watt <= 210)
                                {
                                    countBad++;
                                }
                            }

                            if(countBad >= 3)
                            {
                                apiClient.TogglePower(false);
                                Thread.Sleep(10000);
                                apiClient.TogglePower(true);
                                // Wait 5 min, roughly takes that time for the miner to start mining...
                                Thread.Sleep(300000);
                            }
                            _lastWattages.Clear();
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"Exception Catched:\n{e.Message}\n{e.StackTrace}");
                    }
                }
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
