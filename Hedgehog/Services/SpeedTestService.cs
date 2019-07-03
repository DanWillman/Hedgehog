using System;
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

        public SpeedTestService()
        {
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
            //options.AddArgument("--user-agent=Mozilla/5.0 (iPad; CPU OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A5355d Safari/8536.25");
            options.BinaryLocation = @"/Applications/Google Chrome.app/Contents/MacOS/Google Chrome";
            ChromeDriverService cds = ChromeDriverService.CreateDefaultService("/Applications/selenium", "chromedriverv75");

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
                    Console.WriteLine($"Found {downResults.Count} results for download speed.");
                    foreach (var down in downResults)
                        downSpeed += $"{down.Text}";

                    Console.WriteLine($"Download speed(s) {downSpeed}Mbps");

                    var upResults = driver.FindElements(By.CssSelector("[class='result-data-large number result-data-value upload-speed']"));
                    Console.WriteLine($"Found {upResults.Count} results for upload speed.");
                    foreach (var up in upResults)
                        upSpeed += $"{up.Text}";

                    Console.WriteLine($"Upload speed(s) {upSpeed}Mbps");

                    var pingResults = driver.FindElements(By.CssSelector("[class='result-data-large number result-data-value ping-speed']"));
                    Console.WriteLine($"Found {pingResults.Count} results for ping");
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
    }
}