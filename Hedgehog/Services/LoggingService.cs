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

        public LoggingService(IConfiguration config)
        {
            this.config = config;
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
            using (StreamWriter sw = new StreamWriter(logPath, true))
            {
                sw.WriteLine($"{DateTime.Now}|{data.Latency}|{data.DownSpeed}|{data.UpSpeed}|{data.ServerName}||");
            }
            Console.WriteLine($"Result Logged|{data.DownSpeed}Mbps down");
        }

        private void InitFile()
        {
            if (!File.Exists(logPath))
            {
                using(StreamWriter sw = new StreamWriter(logPath, true))
                {
                    sw.WriteLine("LogTime|Latency(ms)|Down Speed(Mbps)|Up Speed(Mbps)|Server Tested|Error Type|Error Message");
                }
            }
        }
    }
}