using System;
using System.IO;
using Hedgehog.Models;
using Microsoft.Extensions.Configuration;

namespace Hedgehog.Services
{    
    public class LoggingService : ILoggingService
    {
        private readonly string logPath = $"{Environment.CurrentDirectory}/SpeedTests.txt";
        private IConfiguration config;
        private GoogleSheetService sheetService;

        public LoggingService()
        {
            sheetService = new GoogleSheetService();
        }

        public void LogError(Exception ex)
        {
            using (StreamWriter sw = new StreamWriter(logPath, true))
            {
                sw.WriteLine($"{DateTime.Now}|XXXX|XXXX|XXXX|XXXX|{ex.GetType().ToString()}|{ex.Message}");
            }
            Console.WriteLine($"Error Logged|{ex.Message}");
        }

        public void LogResults(TestResult data)
        {
            sheetService.CreateEntry(data);
            Console.WriteLine($"Result Logged|{data.DownSpeed}Mbps down");
        }
    }
}