using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Hedgehog
{
    public class Program
    {
        private static readonly string logPath = @"/Users/dan/Desktop/InternetSpeed.txt";
        public static void Main(string[] args)
        {
            Console.WriteLine("Gotta go fast!");
            InitFile();

            do
            {
                try
                {
                    TestResult results = GetData();
                    LogResults(results);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error encountered - {ex.Message}");
                    LogError(ex);
                }
                Console.WriteLine("Test done, waiting for next test");
                Thread.Sleep(TimeSpan.FromMinutes(1));
            } while (true);
        }

        public static TestResult GetData()
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
                Latency = latency
            };
        }

        public static void InitFile()
        {
            if (!File.Exists(logPath))
            {
                using(StreamWriter sw = new StreamWriter(logPath, true))
                {
                    sw.WriteLine("LogTime|Latency(ms)|Down Speed(Mbps)|Up Speed(Mbps)|Server Tested|Error Type|Error Message");
                }
            }
        }

        public static void LogResults(TestResult data)
        {
            using (StreamWriter sw = new StreamWriter(logPath, true))
            {
                sw.WriteLine($"{DateTime.Now}|{data.Latency}|{data.DownSpeed}|{data.UpSpeed}|{data.ServerName}||");
            }
            Console.WriteLine($"Result Logged|{data.DownSpeed}Mbps down");
        }

        public static void LogError(Exception ex)
        {
            using (StreamWriter sw = new StreamWriter(logPath, true))
            {
                sw.WriteLine($"{DateTime.Now}|XXXX|XXXX|XXXX|XXXX|{ex.GetType().ToString()}|{ex.Message}");
            }
            Console.WriteLine($"Error Logged|{ex.Message}");
        }
    }
}
