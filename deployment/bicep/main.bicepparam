using 'main.bicep'

param tags = { 
  Name: 'thesis'
}
param iamKeyVaultName = 'kv-entra-apps-sdc'
param services = ['Service1', 'Service2', 'Service3']
param appServicePlanName = 'asp-fnapp-services-sdc'
