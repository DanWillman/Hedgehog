using System;
using System.Threading;
using Hedgehog.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hedgehog.Services
{
    public class ServiceRunner
    {
        private readonly IConfiguration config;
        private readonly ISpeedTestService speedService;
        private readonly ILoggingService logService;

        public ServiceRunner(IConfiguration config, ISpeedTestService speedService, ILoggingService loggingService)
        {
            this.config = config;
            logService = loggingService;
            this.speedService = speedService;
        }

        /// <summary>
        /// Begins montioring service to check and log internet speeds
        /// </summary>
        public void StartMonitoringService()
        {
            do
            {
                try
                {
                    TestResult results = speedService.GetData();
                    logService.LogResults(results);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"Error encountered - {ex.Message}");
                }
                Console.WriteLine("Test done, waiting for next test");
                Thread.Sleep(TimeSpan.FromMinutes(double.Parse(config["ServiceRunnerDelay"])));
            } while (true);
        }
    }
}