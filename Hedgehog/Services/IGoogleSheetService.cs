using Hedgehog.Models;

namespace Hedgehog.Services
{
    public interface IGoogleSheetService
    {
        void CreateEntry(TestResult dataPoint);

        void ReadEntries();
    }
}