extension microsoftGraphV1

@description('The display name of the Azure Entra application')
param displayName string

@description('Tags')
param tags array = []

@description('Application roles, defined with roleName and roleId objects')
param appRoles array = []

@description('Allowed member types')
param allowedMemberTypes array = ['Application']

// create app first with bare minimum required props
resource appCreation 'Microsoft.Graph/applications@v1.0' = {
  displayName: displayName
  uniqueName: replace(displayName, ' ', '')
}

resource appCreationServicePrincipal 'Microsoft.Graph/servicePrincipals@v1.0' = {
  appId: appCreation.appId
}

// append app configuration
resource appConfig 'Microsoft.Graph/applications@v1.0' = {
  displayName: appCreation.displayName
  uniqueName: appCreation.uniqueName
  tags: tags
  appRoles: [
    for role in appRoles: {
      allowedMemberTypes: allowedMemberTypes
      description: role.name
      displayName: role.name
      id: role.id
      isEnabled: true
      value: role.name
    }
  ]
  identifierUris: ['api://${appCreation.appId}']
}

output appIdUri string = 'api://${appCreation.appId}'
output uniqueName string = appCreation.uniqueName
output appId string = appCreation.appId
