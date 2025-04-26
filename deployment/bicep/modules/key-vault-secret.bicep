@description('Name for the Azure Key Vault.')
param keyVaultName string

@description('Secrets to add into key vault. Can be an array of arrays.')
param secrets array = []

resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' existing = {
  name: keyVaultName
}

resource keyVaultSecrets 'Microsoft.KeyVault/vaults/secrets@2023-02-01' = [
  for secret in secrets: {
    parent: keyVault
    name: secret.name
    properties: {
      value: secret.value
    }
  }
]
