@allowed([
  'dev'
  'at'
  'test'
  'demo'
  'oat'
  'release'
])
param resourceEnvironmentName string

param serviceName string

param azureAdTenantId string
param azureAdClientId string

@secure()
param azureAdClientSecret string

@secure()
param PDSAppInsightsInstrumentationKey string

@secure()
param PDSAllocationCosmoDbAuthKey string

@secure()
param AllocationsAzureSearchApiKey string

param location string = resourceGroup().location

var resourceNamePrefix = toLower('pds-${resourceEnvironmentName}-${serviceName}')
var appServiceName = '${resourceNamePrefix}-as'
var appServicePlanName = '${resourceNamePrefix}-asp'

// App Service Plan Details
resource appServicePlan 'Microsoft.Web/serverfarms@2021-02-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'S1'
    tier: 'Standard'
    size: 'S1'
  }
}

// Web App Details
resource webApp 'Microsoft.Web/sites@2021-02-01' = {
  name: appServiceName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      netFrameworkVersion: 'v6.0'
      appSettings: [
        // Azure AD configuration
        {
          name: 'AzureAd:Instance'
          value: environment().authentication.loginEndpoint
        }
        {
          name: 'AzureAd:TenantId'
          value: azureAdTenantId
        }
        {
          name: 'AzureAd:ClientId'
          value: azureAdClientId
        }
        {
          name: 'AzureAd:Audience'
          value: azureAdClientId
        }
        {
          name: 'AzureAd:ClientSecret'
          value: azureAdClientSecret
        }

        // PDS App Insights configuration
        {
          name: 'PdsApplicationInsights:InstrumentationKey'
          value: PDSAppInsightsInstrumentationKey
        }
        {
          name: 'PdsApplicationInsights:Environment'
          value: resourceEnvironmentName
        }

        // Logging
        {
          name: 'Logging:LogLevel:Default'
          value: 'Information'
        }
        {
          name: 'Logging:ApplicationInsights:LogLevel:Default'
          value: 'Information'
        }
        {
          name: 'Logging:ApplicationInsights:LogLevel:Microsoft'
          value: 'Error'
        }

        // CosmosDB configuration
        {
          name: 'AllocationsCosmosDb:ServiceEndpoint'
          value: 'https://pds-cosmos-allocations-${resourceEnvironmentName}.documents.azure.com:443/'
        }
        {
          name: 'AllocationsCosmosDb:AuthKeyOrResourceToken'
          value: PDSAllocationCosmoDbAuthKey
        }
        {
          name: 'AllocationsCosmosDb:DatabaseName'
          value: 'adults'
        }
        {
          name: 'AllocationsCosmosDb:CollectionName'
          value: 'statements'
        }

        // Azure Search Service
        {
          name: 'AllocationsAzureSearchService:Name'
          value: 'pds-azs-allocations-${resourceEnvironmentName}'
        }
        {
          name: 'AllocationsAzureSearchService:QueryApiKey'
          value: AllocationsAzureSearchApiKey
        }
        {
          name: 'AllocationsAzureSearchService:LocalAuthoritySearchIndexName'
          value: 'la-index'
        }
        {
          name: 'AllocationsAzureSearchService:ProviderSearchIndexName'
          value: 'provider-index'
        }
        {
          name: 'AllocationsAzureSearchService:LocalAuthoritySearchFirstPageSize'
          value: '10'
        }
      ]
    }
  }
}

// Output the Web App URL
output webAppUrl string = 'https://${webApp.name}.azurewebsites.net'
