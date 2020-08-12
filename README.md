# COM814-Project
[![BCH compliance](https://bettercodehub.com/edge/badge/ConorMcErlean/HelpingHand?branch=master&token=cea74bac43361759c8e7a06048dd117fb92e296d)](https://bettercodehub.com/)

Written as a MSc. Professional Software Programming final project.

## To Run:
### Current Instance
[https://helpinghandni.azurewebsites.net/] The current live instance for testing.

### Locally
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


## Current Build
### Known Bugs
- [ ]  When you submit a report without getting you're current location, it may submit with false co-ordinates
- [ ] The map is currently broken on the view report page
- [ ] Changing a user's password causes an error
- [ ] Delete Report page needs updated styling
- [ ] Forms need padding on mobile
- [ ] Need an alternative solution to get the user's email to change settings
- [ ] Improve view model validation

### Report Tasks still to be completed
- [ ] User testing section
- [ ] Heading 3 Numbers needed fixed
- [ ] User feedback needs added to appendix
- [ ] Use case diagrams need added to appendix

# Ideas for future work
If you, or someone you know would like an example project to work on, here are some possible modifications that could be made.

The following is taken from the dissertation that goes alongside this project:

Suggestion | Explaination
------------ | -------------
API |By having a public facing API, it opens the option to build new and different front-end experiences. It could be a native IOS or Android application, a smart sensor for street placement or something completely unthought of. By opening the doors and providing access to the API, many options are possible.
XAMARIN NATIVE / BLAZOR PWA | The next step would be to build an application that more seamlessly blends into the mobile experience. This would be possible using the cross-platform Xamarin, or the progressive web application framework Blazor. Both would provide a more native experience for IOS and Android.
ANDROID INSTANT APP & IOS APP CLIPS | As of IOS 14, and for the past few releases of Android small, single use applications have been supported, without a full application download. This seems perfectly suitable to the present project due to the native experience when you need it, but minus the install. 
SMART ASSISTANT INTEGRATION | With Googles Assistant, and Apples Siri on almost every phone in the developed world, this would be another mobile focused opportunity for future development. Being able to tell either assistant “A homeless person needs help here”, and it generating a report is a simple but obvious integration that could be completed in the future.
DATA ENCRYPTION | Currently data is neither properly encrypted during transfer, or at rest on the database. I would have loved to get into the technical side of encryption, but time did not permit. This is however a great area for future development.
REPORTING | As mentioned in the introduction, street counts are rarely conducted within Northern Ireland, and as a result, information on the number of rough sleepers is not particularly detailed. With simple modifications it would be possible to generate a weekly or nightly report with this information anonymised. Thus, potentially providing a fix to one of Northern Ireland’s other key homelessness issues, lack of information.
SECURITY AUDIT | While every step was taken to ensure security when using the application, I am no security expert, and it would be worthwhile to get someone who has a deeper understanding of system vulnerabilities to assess the project.
MARKET PIVOT | While mentioned in the ‘business case’ section, it is worth mentioning again that the core of this application, creating a report from a location and some information is a broadly useful offering, and future development may lie in converting this project to a pot-hole submission tool, a graffiti submission tool, or even as a tool for submitting a location that needs first aid response in a large event. Possibilities are endless for what could be done.

## Other Issues
The following are the thing that would be done differently were the project repeated.

ITEM | ISSUE | REQUIRED ACTIONS
------------ | ------------- | -------------
USER INTERFACE | While the current user interface is clear and functional, it does not strictly follow any one set of style guidelines, and so may present some inconsistencies. | Follow a specific set of user interface guidelines, such as the material design guidelines published by Google, and use the materialise styling framework rather than bootstrap.
HANGFIRE | Hangfire, the service for handling background tasks in the application is incredibly useful and incredibly powerful. However, it currently uses local storage to store its task list. In a live deployment this could create issues as we would need to pay for a slightly larger storage on the server deployment. This was not done in the current implementation due to a current bug on the MySQL storage option. | Use one of the many supported storage options to link to a secondary database to store Hangfire jobs.
AUTOMATED TESTING | Automated testing was not given the attention it deserved earlier in the project, and as a result more time was wasted in the long term. A true lesson in ‘technical debt’. | Write automated tests earlier and use automated tests more frequently. Discover and fix issues much earlier in the process.
INTEGRATION TESTING | Integration testing currently covers all user requirements to ensure the succeed correctly, but they don’t currently ensure they fail correctly. | Write integration tests that ensure the system fails gracefully.
UNIT TESTING | Unit testing still requires the context dependency. | Use Moq to mock the context for unit tests to ensure reproducibility.
REPORT MANAGEMENT | It was assumed that managing and preventing an excessive number of reports from the same area would reduce useful information, rather than clarify it. However, we cannot be certain this is so. | Discuss with an actual organisation whether ignoring duplicate reports is helpful. If so, add a to the create method a check for similar reports.
EMAIL SERVICE | Mailgun was used due to its free license of the GitHub student developer pack. While this was useful for quickly adding the functionality to the proof of concept application, the real-world use may incur charges increasing the cost to run. | As the project would be ran by non-profit organisations, discuss with Mailgun the non-profit pricing scheme.If unsuitable, build a custom method to perform this action using System.net tools.
SMALL INTERFACES | One of the better code foundation’s recommendations was to reduce the number of parameters in interfaces. Some of the methods take four different parameters, making them more sensitive to changes, thus reducing maintainability. | Refactor these interfaces using a data transfer object to better improve maintainability and reduce code fragility.
SHORTER UNITS OF CODE | Some of the methods are too long, which makes it difficult to spot where and why something may go wrong. This reduces maintainability. | Refactoring long methods, and breaking them into smaller helper methods would reduce the length of individual units of code, improving maintainability.
MAP LINKING | The HighMaps map API that is used to display all reports on a map does not support links. Therefore there is no way to select a report on the map and view in greater detail as can be done in the list. | Potentially swap to a different map provider that supports this functionality.
