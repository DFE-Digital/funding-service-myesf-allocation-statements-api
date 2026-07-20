# Manage Your Education and Skills Funding Allocation Statements API

The Manage Your Education and Skills Funding (MYESF) Allocation Statements API is used by the MYESF web application to allow the following:

- Retrieving/Updating the allocation statements.
- Invokes the search provider using the allocation type.
- Invokes the local authority search (text/code) using the funding statement type filter.

## Provider

[The Department for Education](https://www.gov.uk/government/organisations/department-for-education)

## About this project

This project is an ASP.NET Core 8 web application utilising Azure App Service for deployment.

The web application runs on an Azure App service on Azure.

**Note:** The project is currently being updated to be containerised via Docker where the deployment method and target will change, this document will be updated when these changes have been finalised.

# Local Configuration Guide

In order to run the application locally a valid `appsettings.json` file will need to be created in the `AllocationStatementsApi` project. Below, and included in the repo, there is `appsettings.example.json` which can be used as a base and populated with the required values, which can be retrieved from the Azure Portal.

## Application Settings (`appsettings.json`)

```json
{
  "PdsApplicationInsights": {
    "InstrumentationKey": "",
    "Environment": "local"
  },
  "Logging": {
    "ApplicationInsights": {
      "LogLevel": {
        "Default": "Information",
        "Microsoft": "Error"
      }
    },
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AzureAd": {
    "Audience": "",
    "ClientId": "",
    "ClientSecret": "",
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": ""
  },
  "AllocationsCosmosDb": {
    "ServiceEndpoint": "",
    "DatabaseName": "",
    "CollectionName": "",
    "AuthKeyOrResourceToken": ""
  },
  "AllocationsAzureSearchService": {
    "LocalAuthoritySearchFirstPageSize": "",
    "LocalAuthoritySearchIndexName": "",
    "Name": "",
    "ProviderSearchIndexName": "",
    "QueryApiKey": ""
  }
}
```

### Setting Details

- **`PdsApplicationInsights:InstrumentationKey`**  
  The key value for Application Insights resource for logging purposes.

- **`PdsApplicationInsights:Environment`**  
  The environment which the app is running on for Application Insights for logging purposes.

- **`Logging:ApplicationInsights:LogLevel:Default`**
  The default logging level for the service when logging to Application Insights; refer to the [Microsoft Documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=net-9.0-pp) for an explanation of the different levels.

- **`Logging:ApplicationInsights:LogLevel:Microsoft`**
  The default logging level for Microsoft specific information when logging to Application Insights; refer to the [Microsoft Documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=net-9.0-pp) for an explanation of the different levels.

- **`Logging:LogLevel:Default`**
  The default logging level for the service; refer to the [Microsoft Documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=net-9.0-pp) for an explanation of the different levels.

- **`AllowedHosts`** 
  The configuration setting used by web frameworks and development tools to restrict incoming HTTP requests based on the HTTP Host header. It serves as a shield against HTTP Host header attacks and DNS rebinding exploits.

- **`AzureAd:Audience`**  
  The intended recipient of the azure authentication token.
 
- **`AzureAd:ClientId`**  
  The application (client) ID registered in azure ad.

- **`AzureAd:ClientSecret`**  
  The confidential credential used for the application to authenticate its identity.

- **`AzureAd:Instance`**  
  The URL of the azure ad service used to authenticate.

- **`AzureAd:TenantId`**  
  The unique identifier for your azure ad tenant.

- **`AllocationsCosmosDb:ServiceEndpoint`** 
  The uri for the allocation cosmos db resource.

- **`AllocationsCosmosDb:DatabaseName`** 
  The exact name of the target database container inside the Azure Cosmos DB account.

- **`AllocationsCosmosDb:CollectionName`** 
  The unique name of the target container inside the designated Cosmos DB database where allocation records are physically stored.

- **`AllocationsCosmosDb:AuthKeyOrResourceToken`** 
  The secret value for allocation cosmos db resource.

- **`AllocationsAzureSearchService:LocalAuthoritySearchFirstPageSize`**
  The specific number of records returned on the initial page of a Local Authority search request.

- **`AllocationsAzureSearchService:LocalAuthoritySearchIndexName`**
  Specifies the target Azure AI Search index containing the Local Authority data.

- **`AllocationsAzureSearchService:Name`**
  Specifies the unique name of the Azure AI Search service instance deployed in Azure.

- **`AllocationsAzureSearchService:ProviderSearchIndexName`**
  Specifies the target Azure AI Search index containing the Provider data.
 
- **`AllocationsAzureSearchService:QueryApiKey`**
  The read-only query API key required to authenticate inbound search requests against the Azure AI Search service instance.

## Build and Test

To build and test locally, you can either use Visual Studio, Visual Studio Code or simply use dotnet CLI `dotnet build` and `dotnet test` more information in dotnet CLI can be found at <https://docs.microsoft.com/en-us/dotnet/core/tools/>.

## Contribute

To contribute,

- If you are part of the team then create a branch for changes and then submit your changes for review by creating a pull request.
- If you are external to the organisation then fork this repository and make necessary changes and then submit your changes for review by creating a pull request.