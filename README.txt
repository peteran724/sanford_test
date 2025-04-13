[*****INSTRUCTION*****]

BlueCorp solution includes 3 projects

BlueCorp.Common project is a library based on .NET 8, it contians some common classes and tools

BlueCorp.D365 project is a RESTful api based on asp.net core web api and .NET8, it can collect data and dispatch json file to Blue.ThirdPL project.
It is using AutoMapper to map between DTO and Domain object, using Polly to circuit to dispatch json file to Blue.ThirdPL project.

Blue.ThirdPL project is a RESTful api based on asp.net core web api and .NET8, it can covert json file dispatched from BlueCorp.D365 project into csv file and upload csv file to related folder, and move csv file into related folder with schedule job. 
It is using Hangfire for schedule job and CsvHelper to deal with csv file.

=====================================================================================================

[*****TEST*****]

Open the BlueCorp.sln file with VS2022, Press F5 to lanuch it and use the BlueCorp.postman_collection.json script file with PostMan to test.


=====================================================================================================

[*****TIME SPENT*****]

Design: about 1 hour

Coding：about 4 hours

Test: about 1 hour


=====================================================================================================

[*****ANYTHING IF I HAVE MORE TIME*****]

A. Do unit test, integration test and performance test
B. Provide the UI for BlueCorp.D365
C. Use JWT authentication for BlueCorp.D365
D. Build the microservices with CloudNative/Serverless technology
   D1. Uee Azure SQL(PASS) to store encrypted data instead of memory repository for BlueCorp.D365
   D2. Encrypt the csv file and store them in the Azure Storage
   D3. Use Azure Key Vault to store the API Key for BlueCorp.D365 calling BlueCorp.ThirdPL
   D4. Use Azure Monitor(Azure Application Insighs) for monitor, metric, log analyse and alert
   D5. Use Azure Service Bus as a Message Broker to keep data consistent and decouping between BlueCorp.D365 and BlueCorp.ThirdPL
   D6. Use Azure API Management to manage the backend services
   D7. Deploy the project running in Azure App Service
   D8. Use Azure Functions to convert json file into csv file














