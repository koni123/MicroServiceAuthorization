using 'iam.bicep'

param entraTags = ['thesis']
param tags = { 
  Name: 'thesis'
}

// https://learn.microsoft.com/en-us/graph/templates/reference/applications?view=graph-bicep-1.0#microsoftgraphapprole
// "When creating a new app role, a new GUID identifier must be provided."
param applications = [
  {
    id: 'Service1'
    name: 'Service1App'
    displayName: 'Service1 Identity'
    appRoles: [{ id: '9fa87d0a-3737-444f-b522-39bb999a9112', name: 'Service1.Read' }]
  }
  {
    id: 'Service2'
    name: 'Service2App'
    displayName: 'Service2 Identity'
    appRoles: [{ id: '039a00ea-2a90-46c5-a98b-57273378eb03', name: 'Service2.Read' }]
  }
  {
    id: 'Service3'
    name: 'Service3App'
    displayName: 'Service3 Identity'
    appRoles: [{ id: '27fbe3f8-9e08-45fb-ae2a-351099843313', name: 'Service3.Read' }]
  }
]

param keyVaultName = 'kv-entra-apps-sdc'
