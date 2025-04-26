@description('The name of the app service.')
param appServicePlanName string

@description('Location for resource.')
param location string = resourceGroup().location

@description('SKU of app service plan')
param sku object

@description('Resource tags')
param tags object

resource hostingPlan 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  sku: sku
  properties: {}
}

output appServicePlanId string = hostingPlan.id
