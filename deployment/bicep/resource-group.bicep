targetScope = 'subscription'

param tags object
param location string
param resourceGroupName string

module rg 'modules/resource-group.bicep' = {
  name: 'rg-deployment'
  params: {
    resourceGroupLocation: location
    resourceGroupName: resourceGroupName
    tags: tags
  }
}
