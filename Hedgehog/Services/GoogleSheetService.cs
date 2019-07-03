using System;
using Google.Apis.Sheets.v4;
namespace Hedgehog.Services
{
    public class GoogleSheetService
    {
        private readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private readonly string ApplicationName = "Hedgehog";
        private readonly string SpreadSheetId = "";
    }

}