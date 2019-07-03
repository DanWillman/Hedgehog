using System;
using Hedgehog.Models;

namespace Hedgehog.Services
{
    public interface ILoggingService
    {
        void LogResults(TestResult results);
        void LogError(Exception ex);
    }
}