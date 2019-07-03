using System;
using System.IO;
using System.Linq;
using System.Threading;
using Hedgehog.Models;
using Hedgehog.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Hedgehog
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            Console.WriteLine("Gotta go fast!");
            ServiceRunner sr = new ServiceRunner();
            sr.StartMonitoringService();
        }
    }
}
