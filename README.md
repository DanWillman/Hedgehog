# Hedgehog
I don't trust my ISP. I rarely get close to my promised speeds, and my service quality is so intermitent that I frequently find my internet speeds slowing to pre-cambrian times. Enter Hedgehog (because we've gotta go fast). Credit for the idea goes to @dalinicus who pointed me towards Selenium to setup headless browser testing, allowing my to hit speedtest.net instead of trying to stand up my own speed service. 

Hedgehog runs a ChromeDriver from Selenium to kick off a speedtest.net test, waits for it to finish and then logs the results to a google sheet.

Results include: TimeStamp, Latency(ms), Down Speed(Mbps), Up Speed(Mbps), Test Server Name, Client Name.
![Results Sample](https://i.imgur.com/C5iCmcO.png)

With the data loaded to a google sheet, it's trivial to create charts and graphs to help visualize your data:
![Chart Sample](https://i.imgur.com/UoUc8RH.png)

The default polling time is 2 hours, this can be adjusted in `appsettings.json` to suit your needs

### Danger Will Robinson
If you're like me and on a data cap, be careful with how quickly you configure your polling to take place. I initially started out with 2 minutes for testing and forgot to turn it off before going to bed, it ran through 50+ GB of data overnight. 2 Hours works for me, find one that works for you. 

## Setting up OAuth for Google Sheets
I followed this [blog](https://www.twilio.com/blog/2017/03/google-spreadsheets-and-net-core.html) to setup google sheet authentication. It's fairly straight forward, once you have your credentials json file, you can add it to user secrets. 

## Setting up and accessing secrets
Effectively follow this [blog](https://www.twilio.com/blog/2018/05/user-secrets-in-a-net-core-console-app.html) to configure them. .NET core handles secrets in a separate directory from the project, and ties the `secrets.json` back to the project with a UserSecretesId property on the .csproj file. 

Our secrets file will be similar to:
```json
{
  "GoogleSheet:SheetId": "SomeSheetId",
  "GCred:Type": "CredentialType",
  "GCred:ProjectId": "ProjectId",
  "GCred:PrivateKeyId": "PrivateKeyId",
  "GCred:PrivateKey": "PrivateKey",
  "GCred:ClientEmail": "ClientEmail",
  "GCred:ClientId": "ClientId",
  "GCred:AuthUrl": "AuthUrl",
  "GCred:TokenUrl": "TokenUrl",
  "GCred:ProviderCertUrl": "CertUrl",
  "GCred:ClientCertUrl": "CertUrl"
}
```

This will map to placeholders in our `appsettings.json`:
```json
  "GoogleSheet": {
    "SheetId": "",
    "SheetName": "Sheet1"
  },
  "GCred": {
    "Type": "",
    "ProjectId": "",
    "PrivateKeyId": "",
    "PrivateKey": "",
    "ClientEmail": "",
    "ClientId": "",
    "AuthUrl": "",
    "TokenUrl": "",
    "ProviderCertUrl": "",
    "ClientCertUrl": ""
  },
```

In turn these are mapped to models in `Models/Configuration/` which are used to parse and load secrets to the configuration builder in `Program.cs`:
```C#
builder.AddUserSecrets<GoogleSheet>();
builder.AddUserSecrets<GCred>();
```

They are then accessed via the IOptions extension:
```C#
public GoogleSheetService(IOptions<GCred> credentials, IOptions<GoogleSheet> sheetConfig)
{
    ...
    
    credential = GoogleCredential.FromJson(JsonConvert.SerializeObject(credentials.Value))
            .CreateScoped(Scopes);
    ...
}
```

## Setting up environment variables/appsettings
Paths to the Chrome install and Selenium drivers are stored in the `appsettings.{os.}json` files. 
```json
"ChromeDriver": {
    "ChromeBinaryLocation": "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe",
    "SeleniumLocation": "C:\\Program Files\\Selenium",
    "SeleniumDriverName": "chromedriverv75.exe"
},
```
These will need to be updated to reflect the pathing for the machine you're running on. `appsettings.macos.json` Contains overrides for running on MacOS. Currently there is no settings file for linux, they could simply be set in `appsetings.json`.