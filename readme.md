# Azure Entra application registrations with roles and permissions with Bicep

## Introduction
This is a technical implementation of the master's thesis "AUTOMATED IDENTITY AND ACCESS MANAGEMENT FOR MICROSERVICES IN AZURE:
Entra application registrations with roles and permissions with Bicep"

This repository contains the Bicep templates and PowerShell scripts used to deploy Azure resources for the thesis 
project. In addition to that, there is a simple C# function app that implements the authorization testing functionality.

## Solution Overview
This solution consists of following main components:
- **Bicep Templates**: Used for deploying necessary Azure resources.
- **PowerShell Scripts**: Used for managing secrets.
- **C# Function Apps**: Implementing the authorization testing functionality.

## Deployment

### Prerequisites
- Azure CLI
- PowerShell
- .NET SDK

### Steps

These steps deploy the necessary Azure resources to test the functionality.

#### Clone the Repository
 ```sh
 git clone https://github.com/koni123/MicroServiceAuthorization
 cd .\MicroServiceAuthorization\deployment\bicep\
 ```

#### Service principal setup
For simplicity, only one service principal was used to deploy all thesis related resources. You can create the service principal
either with Azure CLI or from Azure portal. Following are the steps to create the service principal with Azure CLI. Note 
that one role needs to be always granted in Azure Entra UI. Next command returns the appId and password for the service
principal so make sure to save them in a secure place for later use.
```
az ad sp create-for-rbac --name thesisResourceDeployments --role contributor --scopes /subscriptions/<subscription-id>
```

Following are the granted roles for service principal.
(Note that tighter scoping and custom roles allow much more fine-grained access control and should be used in production deployments to prevent permission leaks.)

***Contributor*** (subscription level):
```
az role assignment create --assignee <sp-id> --role "Contributor" --scope /subscriptions/<subscription-id>
```

***Role Based Access Control Administrator*** (resource group level):
```
az role assignment create --assignee <sp-id> --role "Role Based Access Control Administrator" --scope /subscriptions/<subscription-id>/resourceGroups/thesis-deployments
```

***Key Vault Secrets Officer*** (resource group level):
```
az role assignment create --assignee <sp-id> --role "Key Vault Secrets Officer" --scope /subscriptions/<subscription-id>/resourceGroups/thesis-deployments
```

In addition to role assignments above, to administer the Azure Entra applications, the service principal was 
granted ***Cloud Application Administrator*** in Azure Entra. It is not possible currently to grant this role using Azure CLI.

#### Login to Azure CLI
Replace tenant-id with your just created service principal.
 ```
 az login --service-principal -u <sp-id> -p <sp-password> --tenant <tenant-id>
 ```

#### Deploy Resource group
Bicep template `resource-group.bicep` is used to deploy the necessary Azure resources.
For `resource-group.bicepparam` you can set the resource group name and location.

```sh
az deployment sub create --location <location> --template-file resource-group.bicep --parameters resource-group.bicepparam
```

#### Create Azure Entra applications and key vault secrets.
Bicep template `iam.bicep` is used to deploy the Azure Entra apps + key vault + secrets. 
For `iam.bicepparam` you can set the service names, application roles and iam key vault name.
Note that Application Roles need to have generated GUIDs when creating.
```sh
az deployment group create --resource-group <resource-group-name> --template-file iam.bicep --parameters iam.bicepparam
```

#### Run the PowerShell script to reset and store secrets in Azure Key Vault.
For `resetSecrets.json` you can set the service names and key vault name where to put the secrets.
```sh
powershell -ExecutionPolicy Bypass -File ..\scripts\resetSecrets.ps1
```

#### Create the Function App and other necessary resources.
Replace resource-group-name with the resource group name created in the first step.
For `main.bicepparam` you need to set the iam key vault name created with `iam.bicep`, app service plan name
and service names as set in `iam.bicepparam`.
```sh
az deployment group create --resource-group <resource-group-name> --mode Incremental --template-file main.bicep --parameters main.bicepparam
```

#### Delete the Resource Group and resources
Replace resource-group-name with the resource group name created in the first step.
Note that you need to delete the created Entra applications separately from portal and Entra UI.
Alternative approach is to use Azure CLI to first soft delete and then purge apps from portal.
```sh
az ad sp delete --id <appId>
```

## Function Apps

Function app is a dotnet-isolated .NET 8 function app that implements the authorization testing functionality. These
can be run locally or deployed to Azure. The function app is a simple HTTP trigger that calls the authorization
testing endpoints of other functions and returns the authorization results.

### Service Tester Function
The `ServiceTesterFunction.cs` is a simple class that calls all services and returns the results.

### Testing the solution
You can send a GET request to the function app URL to get the authorization test results for given service.
```
https://fnapp-service1-sdc.azurewebsites.net/api/service-tester
```

In cloud environment the app service auth is guarding the function app. To make a successful call to the service, 
you need to provide an authorization token in the request Authorization header. You can read more about generating token from documentation in
[Microsoft identity platform](https://learn.microsoft.com/en-us/entra/identity-platform/v2-oauth2-client-creds-grant-flow#first-case-access-token-request-with-a-shared-secret)

