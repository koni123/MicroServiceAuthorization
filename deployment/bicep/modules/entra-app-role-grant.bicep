extension microsoftGraphV1

@description('The identifier (GUID) for the app role that\'s assigned to the principal.')
param appRole object

@description('Client app unique name')
param clientAppUniqueName string

@description('Resource app unique name')
param resourceAppUniqueName string

resource clientApp 'Microsoft.Graph/applications@v1.0' existing = {
  uniqueName: clientAppUniqueName
}

resource clientSp 'Microsoft.Graph/servicePrincipals@v1.0' existing = {
  appId: clientApp.appId
}

resource resourceApp 'Microsoft.Graph/applications@v1.0' existing = {
  uniqueName: resourceAppUniqueName
}

resource resourceSp 'Microsoft.Graph/servicePrincipals@v1.0' existing = {
  appId: resourceApp.appId
}

// assign the role
resource roleAssignment 'Microsoft.Graph/appRoleAssignedTo@v1.0' = {
  appRoleId: appRole.id
  principalId: clientSp.id
  resourceId: resourceSp.id
}

// grant the role
resource roleGrant 'Microsoft.Graph/oauth2PermissionGrants@v1.0' = {
  clientId: clientSp.id
  consentType: 'AllPrincipals'
  resourceId: resourceSp.id
  scope: appRole.name
}
