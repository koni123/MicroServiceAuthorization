@description('The name of the function app.')
param functionAppName string

@description('The id of the app service plan.')
param appServicePlanId string

@description('The name of the storage account for function app.')
param functionStorageName string

@description('App settings for function app.')
param appSettings array

@description('Location for all resources.')
param location string = resourceGroup().location

@description('The language worker runtime to load in the function app. Defaults to isolated worker.')
param runtime string = 'dotnet-isolated'

@description('Resource tags')
param tags object

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' existing = {
  name: functionStorageName
}

var baseSettings = [
  {
    name: 'AzureWebJobsStorage'
    value: 'DefaultEndpointsProtocol=https;AccountName=${functionStorageName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
  }
  {
    name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
    value: 'DefaultEndpointsProtocol=https;AccountName=${functionStorageName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
  }
  {
    name: 'WEBSITE_CONTENTSHARE'
    value: toLower(functionAppName)
  }
  {
    name: 'FUNCTIONS_EXTENSION_VERSION'
    value: '~4'
  }
  {
    name: 'FUNCTIONS_WORKER_RUNTIME'
    value: runtime
  }
]

resource functionApp 'Microsoft.Web/sites@2021-03-01' = {
  name: toLower(functionAppName)
  location: location
  tags: tags
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlanId
    siteConfig: {
      appSettings: union(baseSettings, appSettings)
      ftpsState: 'FtpsOnly'
      minTlsVersion: '1.2'
      netFrameworkVersion: '8.0'
    }
    httpsOnly: true
  }
}

output funcAppObjectId string = functionApp.identity.principalId
output funcAppName string = functionApp.name
