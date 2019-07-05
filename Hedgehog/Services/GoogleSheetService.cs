using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Hedgehog.Models;
using Hedgehog.Models.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Hedgehog.Services
{
    public class GoogleSheetService :IGoogleSheetService
    {
        private readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private readonly string ApplicationName = "Hedgehog";
        
        private SheetsService service;
        private GoogleCredential credential;
        private GoogleSheet sheetConfig;

        public GoogleSheetService(IOptions<GCred> credentials, IOptions<GoogleSheet> sheetConfig)
        {
            this.sheetConfig = sheetConfig.Value;

            credential = GoogleCredential.FromJson(JsonConvert.SerializeObject(credentials.Value))
                    .CreateScoped(Scopes);

            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
        }

        public void ReadEntries()
        {
            var range = $"{sheetConfig.SheetName}!A:F";
            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(sheetConfig.SheetId, range);

            var response = request.Execute();
            IList<IList<object>> values = response.Values;

            if (values != null && values.Count > 0)
            {
                foreach(var row in values)
                {
                    Console.WriteLine($"{row[0]} | {row[1]} | {row[2]} | {row[3]} | {row[4]}");
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }
        }

        public void CreateEntry(TestResult dataPoint)
        {
            var range = $"{sheetConfig.SheetName}!A:F";
            var valueRange = new ValueRange();

            IList<object> insertable = new List<object>() {DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), dataPoint.Latency, dataPoint.DownSpeed, dataPoint.UpSpeed, dataPoint.ServerName, dataPoint.ClientName};

            valueRange.Values = new List<IList<object>> {insertable};

            var appendRequest = service.Spreadsheets.Values.Append(valueRange, sheetConfig.SheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

            var appendResponse = appendRequest.Execute();

            Console.WriteLine($"Appended {appendResponse.Updates.UpdatedRows} rows to spreadsheet");
        }

    }

}