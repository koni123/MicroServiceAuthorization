targetScope = 'resourceGroup'

param tags object
param iamKeyVaultName string
param services array
param appServicePlanName string

module functionAppStorageAccounts 'modules/storage-account.bicep' = [
  for service in services: {
    name: '${service}SA'
    params: {
      tags: tags
      storageAccountName: 'stgfnapp${service}sdc'
    }
  }
]

module appServicePlan 'modules/app-service-plan.bicep' = {
  name: 'AppServicePlan'
  params: {
    tags: tags
    appServicePlanName: appServicePlanName
    sku: {
      name: 'Y1'
      tier: 'Dynamic'
    }
  }
}

resource iamKv 'Microsoft.KeyVault/vaults@2023-02-01' existing = {
  name: iamKeyVaultName
}

var settingName = 'MICROSOFT_PROVIDER_AUTHENTICATION_SECRET'

module serviceFunctionApps './modules/function-app.bicep' = [
  for (service, index) in services: {
    name: '${service}FunctionApp'
    params: {
      tags: tags
      functionAppName: 'fnapp-${service}-sdc'
      appSettings: [
        {
          name: settingName
          value: '@Microsoft.KeyVault(VaultName=${iamKv.name};SecretName=${service}--ClientSecret)'
        }
        {
          name: 'EntraKeyVaultName'
          value: iamKv.name
        }
      ]
      appServicePlanId: appServicePlan.outputs.appServicePlanId
      functionStorageName: functionAppStorageAccounts[index].outputs.storageAccountName
    }
  }
]

module serviceFunctionAppAuths './modules/app-auth.bicep' = [
  for (service, index) in services: {
    name: '${service}AppAuth'
    params: {
      appName: 'fnapp-${service}-sdc'
      clientId: iamKv.getSecret('${service}--ClientId')
      clientSecretSettingName: settingName
    }
  }
]

module iamKvAccessPolicies 'modules/key-vault-rbac.bicep' = [
  for (service, index) in services: {
    name: '${service}EntraKeyVaultRbac'
    params: {
      roleName: 'Key Vault Secrets User'
      keyVaultName: iamKeyVaultName
      principalType: 'ServicePrincipal'
      objectId: serviceFunctionApps[index].outputs.funcAppObjectId
    }
  }
]

module kvBaseUrls 'modules/key-vault-secret.bicep' = [
  for (service, index) in services: {
    name: '${service}EntraKeyVaultSetBaseUrl'
    params: {
      keyVaultName: iamKv.name
      secrets: [
        {
          name: '${service}--BaseUrl'
          value: 'https://${serviceFunctionApps[index].outputs.funcAppName}.azurewebsites.net'
        }
      ]
    }
  }
]
