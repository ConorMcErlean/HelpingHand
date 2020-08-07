# COM814-Project
[![BCH compliance](https://bettercodehub.com/edge/badge/ConorMcErlean/HelpingHand?branch=master&token=cea74bac43361759c8e7a06048dd117fb92e296d)](https://bettercodehub.com/)

Written as a MSc. Professional Software Programming final project.

## To Run:
Ensure you have .NET core 3.1 installed.

Please retrieve your own API keys for Google Maps, What3Words, MailGun & Testmail and add to the Secrets.cs file in TagTool.Data.
Please add a MySQL Database connection string to appsettings.json in TagTool.Web.

From there use the commands
```
dotnet build
dotnet run -p .\TagTool.Web\TagTool.Web.csproj
```
To build and run.

Alternatively, use the Docker Container 
```
confusedconor/dockerstorage:latest
```
To run a pre-built version.

Images were got under paid license (provided by GitHub Student Developer Pack) from Icons8.
If reusing images, ensure you meet the license:
[https://icons8.com/license](https://icons8.com/license)
