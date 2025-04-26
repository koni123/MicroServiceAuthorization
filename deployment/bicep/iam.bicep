targetScope = 'resourceGroup'

param entraTags array
param tags object
param applications array
param keyVaultName string

module serviceApps 'modules/entra-app.bicep' = [
  for app in applications: {
    name: '${app.name}EntraApp'
    params: {
      displayName: app.displayName
      tags: entraTags
      appRoles: app.appRoles
    }
  }
]

// grant app permissions
// =====================

// access to own apis
module roleGrantsOwn 'modules/entra-app-role-grant.bicep' = [
  for (app, index) in applications: {
    name: '${app.name}RoleGrantToSelf'
    params: {
      appRole: app.appRoles[0]
      clientAppUniqueName: serviceApps[index].outputs.uniqueName
      resourceAppUniqueName: serviceApps[index].outputs.uniqueName
    }
  }
]

// service 1 towards service 2
module roleGrants1to2 'modules/entra-app-role-grant.bicep' = {
  name: 'RoleGrant1to2'
  params: {
    appRole: applications[1].appRoles[0]
    clientAppUniqueName: serviceApps[0].outputs.uniqueName
    resourceAppUniqueName: serviceApps[1].outputs.uniqueName
  }
}

// service 1 towards service 3
module roleGrants1to3 'modules/entra-app-role-grant.bicep' = {
  name: 'RoleGrant1to3'
  params: {
    appRole: applications[2].appRoles[0]
    clientAppUniqueName: serviceApps[0].outputs.uniqueName
    resourceAppUniqueName: serviceApps[2].outputs.uniqueName
  }
}

// service 3 towards service 1
module roleGrants3to1 'modules/entra-app-role-grant.bicep' = {
  name: 'RoleGrant3to1'
  params: {
    appRole: applications[0].appRoles[0]
    clientAppUniqueName: serviceApps[2].outputs.uniqueName
    resourceAppUniqueName: serviceApps[0].outputs.uniqueName
  }
}

// create entra key vault for app configs
module entraKeyVault 'modules/key-vault.bicep' = {
  name: 'EntraKeyVaultDeployment'
  params: {
    tags: tags
    keyVaultName: keyVaultName
  }
}

// grant me all policy for convenience
module iamRbac 'modules/key-vault-rbac.bicep' = {
  name: 'EntraKeyVaultAdminAccessSecrets'
  params: {
    keyVaultName: entraKeyVault.outputs.name
    objectId: '013b9d7a-e1c6-4f04-a21e-1571d88ce088'
    roleName: 'Key Vault Secrets Officer'
    principalType: 'User'
  }
}

module kvSecretsForServices 'modules/key-vault-secret.bicep' = [
  for (service, index) in applications: {
    name: '${service.name}EntraKeyVaultSecrets'
    params: {
      keyVaultName: entraKeyVault.outputs.name
      secrets: [
        {
          name: '${service.id}--ClientId'
          value: serviceApps[index].outputs.appId
        }
        {
          name: '${service.id}--TenantId'
          value: tenant().tenantId
        }
        {
          name: '${service.id}--Scope'
          value: '${serviceApps[index].outputs.appIdUri}/.default'
        }
        {
          name: '${service.id}--RoleName'
          value: '${applications[index].appRoles[0].name}'
        }
      ]
    }
  }
]
