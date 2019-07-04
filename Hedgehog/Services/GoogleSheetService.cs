using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Hedgehog.Models;

namespace Hedgehog.Services
{
    public class GoogleSheetService :IGoogleSheetService
    {
        private readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private readonly string ApplicationName = "Hedgehog";
        private readonly string SpreadSheetId = "";
        private readonly string sheet = "Sheet1";
        
        private SheetsService service;
        private GoogleCredential credential;

        public GoogleSheetService()
        {
            using (var stream = new FileStream("hedgehog-gcred.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }

            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
        }

        public void ReadEntries()
        {
            var range = $"{sheet}!A:F";
            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(SpreadSheetId, range);

            var response = request.Execute();
            IList<IList<object>> values = response.Values;

            if (values != null && values.Count > 0)
            {
                foreach(var row in values)
                {
                    System.Console.WriteLine($"{row[0]} | {row[1]} | {row[2]} | {row[3]} | {row[4]}");
                }
            }
            else
            {
                System.Console.WriteLine("No data found.");
            }
        }

        public void CreateEntry(TestResult dataPoint)
        {
            var range = $"{sheet}!A:F";
            var valueRange = new ValueRange();

            IList<object> insertable = new List<object>() {DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), dataPoint.Latency, dataPoint.DownSpeed, dataPoint.UpSpeed, dataPoint.ServerName, dataPoint.ClientName};

            valueRange.Values = new List<IList<object>> {insertable};

            var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadSheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

            var appendResponse = appendRequest.Execute();

            System.Console.WriteLine($"Appended {appendResponse.Updates.UpdatedRows} rows to spreadsheet");
        }

    }

}