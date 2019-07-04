using System;
using System.IO;
using System.Linq;
using System.Threading;
using Hedgehog.Models;
using Hedgehog.Models.Configuration;
using Hedgehog.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hedgehog
{
    public class Program
    {
        public static IConfiguration Configuration { get; set; }
        public static IServiceCollection Services { get; set; }

        public static void Main(string[] args)
        {
            Configure();
            var serviceProvider = Services.BuildServiceProvider();
            ServiceRunner sr = serviceProvider.GetService<ServiceRunner>();

            Console.WriteLine("Gotta go fast!");
            sr.StartMonitoringService();
        }

        private static void Configure()
        {
            var devEnvironmentVariable = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            var isDevelopment = string.IsNullOrWhiteSpace(devEnvironmentVariable) || devEnvironmentVariable.ToLower().Contains("dev");

            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            if (isDevelopment)
            {
                builder.AddUserSecrets<GoogleSheet>();
                builder.AddUserSecrets<GCred>();
            }

            Configuration = builder.Build();
            ConfigureServices();
        }

        private static void ConfigureServices()
        {
            Services = new ServiceCollection();
            Services
                .Configure<GoogleSheet>(Configuration.GetSection(nameof(GoogleSheet)))
                .Configure<GCred>(Configuration.GetSection(nameof(GCred)))
                .AddOptions()
                .AddTransient<ILoggingService, LoggingService>()
                .AddTransient<ISpeedTestService, SpeedTestService>()
                .AddTransient<IGoogleSheetService, GoogleSheetService>()
                .AddTransient<ServiceRunner>()
                .AddSingleton(Configuration)
                .BuildServiceProvider();
        }
    }
}
