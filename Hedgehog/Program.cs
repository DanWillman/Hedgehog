using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
            ConfigureEnivronmentSettings(builder);
            
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

        private static void ConfigureEnivronmentSettings(ConfigurationBuilder builder)
        {
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                builder.AddJsonFile("appsettings.macos.json", optional: true, reloadOnChange: true);
                Console.WriteLine("Loaded MacOS app setting overrides");
            }
        }
    }
}
