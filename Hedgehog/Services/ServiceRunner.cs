using System;
using System.Threading;
using Hedgehog.Models;
using Microsoft.Extensions.Configuration;

namespace Hedgehog.Services
{
    public class ServiceRunner
    {
        private readonly IConfiguration config;
        private readonly ISpeedTestService speedService;
        private readonly ILoggingService logService;

        public ServiceRunner()
        {
            speedService = new SpeedTestService();
            logService = new LoggingService();
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
                Thread.Sleep(TimeSpan.FromMinutes(1));
            } while (true);
        }
    }
}