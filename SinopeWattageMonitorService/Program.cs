using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace SinopeWattageMonitorService
{
    public class Program
    {
        private const string _logFolder = "logs";
        public static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            if (!Directory.Exists(_logFolder))
                Directory.CreateDirectory(_logFolder);

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging(
                        logBuilder => logBuilder.AddFile(_logFolder + "/logs-{0:yyyy}-{0:MM}-{0:dd}.log",
                        fileLoggerOpts => fileLoggerOpts.FormatLogFileName = fName => string.Format(fName, DateTime.Now))
                    );
                    services.AddHostedService<Worker>();
                });
    }
}
