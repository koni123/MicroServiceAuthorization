@description('Storage account name')
param storageAccountName string

@description('Location')
param location string = resourceGroup().location

@description('Storage Account type')
@allowed([
  'Standard_LRS'
  'Standard_GRS'
  'Standard_RAGRS'
])
param storageAccountType string = 'Standard_LRS'

@description('Resource tags')
param tags object

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' = {
  name: toLower(replace(storageAccountName, '-', ''))
  location: location
  tags: tags
  sku: {
    name: storageAccountType
  }
  kind: 'Storage'
  properties: {
    supportsHttpsTrafficOnly: true
    defaultToOAuthAuthentication: true
    allowBlobPublicAccess: false
  }
}

output storageAccountName string = storageAccount.name
