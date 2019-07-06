using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Hedgehog.Models;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Hedgehog.Services
{
    public class SpeedTestService : ISpeedTestService
    {
        private readonly IConfiguration config;
        private Random rand;

        public SpeedTestService(IConfiguration config)
        {
            this.config = config;
            rand = new Random();
        }

        public TestResult GetData()
        {
            string upSpeed = "";
            string downSpeed = "";
            string latency = "";
            string serverAddress = "";
            string url = @"https://www.speedtest.net/";

            ChromeOptions options = new ChromeOptions();
            options.AddArguments("headless", "no-sandbox");
            options.AddArgument($"user-agent={GetRandomUserAgent()}");
            options.BinaryLocation = $"{config["ChromeDriver:ChromeBinaryLocation"]}";
            ChromeDriverService cds = ChromeDriverService.CreateDefaultService($"{config["ChromeDriver:SeleniumLocation"]}", config["ChromeDriver:SeleniumDriverName"]);

            using (var driver = new ChromeDriver(cds, options, TimeSpan.FromSeconds(30.0)))
            {
                driver.Navigate().GoToUrl(url);
                Console.WriteLine("Finding Go button");

                var elements = driver.FindElements(By.CssSelector("[class*='js-start-test']"));
                Console.WriteLine($"{elements.Count} Elements found");
                var e = elements.First();

                Console.WriteLine($"Found {e.Text}, clicking...");
                e.Click();
                Console.WriteLine($"Clicked {e.Text}, awaiting results for 1 minutes");
                Thread.Sleep(TimeSpan.FromMinutes(1));

                var downResults = driver.FindElements(By.CssSelector("[class='result-data-large number result-data-value download-speed']"));
                if (downResults.Count > 0)
                {
                    foreach (var down in downResults)
                        downSpeed += $"{down.Text}";

                    Console.WriteLine($"Download speed(s) {downSpeed}Mbps");

                    var upResults = driver.FindElements(By.CssSelector("[class='result-data-large number result-data-value upload-speed']"));
                    
                    foreach (var up in upResults)
                        upSpeed += $"{up.Text}";

                    Console.WriteLine($"Upload speed(s) {upSpeed}Mbps");

                    var pingResults = driver.FindElements(By.CssSelector("[class='result-data-large number result-data-value ping-speed']"));
                    
                    foreach (var ping in pingResults)
                        latency += $"{ping.Text}";

                    serverAddress = driver.FindElementByClassName("hostUrl").Text;
                    Console.WriteLine($"Tested against {serverAddress}");
                }
                else
                {
                    Console.WriteLine("No results found");
                }

                Console.WriteLine("Job's done.");
            }

            return new TestResult()
            {
                DownSpeed = downSpeed,
                UpSpeed = upSpeed,
                ServerName = serverAddress,
                Latency = latency,
                ClientName = Environment.MachineName
            };
        }

        private string GetRandomUserAgent()
        {
            List<string> userAgents = new List<string>()
            {
                "Mozilla/5.0 (Windows NT 4.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2049.0 Safari/537.36",
                "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.517 Safari/537.36",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_8_2) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/24.0.1309.0 Safari/537.17",
            };
            int randomIndex = rand.Next(userAgents.Count - 1);

            Console.WriteLine($"Using user agent - {userAgents[randomIndex]}");

            return userAgents[randomIndex];
        }
    }
}