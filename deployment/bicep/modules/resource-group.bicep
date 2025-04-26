targetScope='subscription'

@description('Resource group name')
param resourceGroupName string

@description('Resource group location')
param resourceGroupLocation string

@description('Resource tags')
param tags object

resource newRG 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  name: resourceGroupName
  location: resourceGroupLocation
  tags: tags
}
