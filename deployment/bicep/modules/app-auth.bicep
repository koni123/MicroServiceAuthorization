@description('The name of the app.')
param appName string

@secure()
@description('The Azure Entra client id for Authentication')
param clientId string

@secure()
@description('The app setting name that contains the client secret of the relying party application.')
param clientSecretSettingName string

resource site 'Microsoft.Web/sites@2021-03-01' existing = {
  name: toLower(appName)
}

resource authsettings 'Microsoft.Web/sites/config@2022-03-01' = {
  parent: site
  name: 'authsettingsV2'
  properties: {
    platform: {
      enabled: true
      runtimeVersion: '2'
    }
    identityProviders: {
      azureActiveDirectory: {
        enabled: true
        registration: {
          clientId: clientId
          clientSecretSettingName: clientSecretSettingName
          openIdIssuer: 'https://sts.windows.net/${tenant().tenantId}/v2.0'
        }
        validation: {
          allowedAudiences: ['api://${clientId}']
        }
      }
    }
  }
}
