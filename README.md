# Azure DevOps Webhook Integration

## Overview
The Azure DevOps Webhook Integration project is designed to handle webhooks from Azure DevOps, process the payload, and perform various actions such as saving work items to a database and updating work items via the Azure DevOps API. This project is built using .NET 8 and C# 12.0.

## Features
-	Webhook Handling: Receives and validates webhook payloads from Azure DevOps.
-	Work Item Processing: Saves and analyzes work items received from the webhook.
-	Azure DevOps API Integration: Updates work items using the Azure DevOps REST API.
-	Database Integration: Saves work items to a database for persistence.

## Technologies Used
-	.NET 8
-	C# 12.0
-	ASP.NET Core
-	Azure DevOps REST API

## Getting Started
1.	Clone the repository:

```bash
   git clone https://github.com/junior151280/azure-devops-webhook-integration.git
   cd azure-devops-webhook-integration
```
2.	Configure the application:
-	Update the ```appsettings.json``` file with your Azure DevOps and Database configurations.
3.	Run the application:
```bash
dotnet run
```
## Configuration
Ensure you have the following settings in your ```appsettings.json```:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "XToken": "your-token",
  "AzureDevOpsRestApi": {
    "PersonalAccessToken": "your-personal-access-token",
    "Organization": "your-organization",
    "Project": "your-project",
    "AzureDevOpsApiUrl": "https://dev.azure.com/"
  },
  "Database": {
    "ConnectionString": "database://localhost:27017",
    "DatabaseName": "AzureDevOps"
  },
  "AllowedHosts": "*",
  "ApplicationInsights": {
    "ConnectionString": "your-organization"
  }
}
```
## Usage
-	Webhook Endpoint: The application exposes an endpoint to receive webhook payloads from Azure DevOps.
-	Logging: Logs important information and warnings to help with debugging and monitoring.
## Contributing
Contributions are welcome! Please fork the repository and submit pull requests.
## License
This project is licensed under the MIT License.
