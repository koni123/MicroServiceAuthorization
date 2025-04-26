@description('Name for the Key Vault.')
param keyVaultName string

@description('Location for the Key Vault.')
param location string = resourceGroup().location

@description('SKU for the Key Vault.')
param keyVaultSku string = 'standard'

@description('Resource tags')
param tags object

resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' = {
  name: keyVaultName
  location: location
  tags: tags
  properties: {
    tenantId: subscription().tenantId
    sku: {
      family: 'A'
      name: keyVaultSku
    }
    accessPolicies: []
    enablePurgeProtection: null
    enableSoftDelete: false
    enabledForTemplateDeployment: true
    enableRbacAuthorization: true
  }
}

output name string = keyVault.name
