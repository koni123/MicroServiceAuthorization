$scriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$paramFilePath = Join-Path -Path $scriptDirectory -ChildPath "resetSecrets.json"

$parameters = Get-Content -Path $paramFilePath -Raw | ConvertFrom-Json
$services = $parameters.services.value
$vaultName = $parameters.keyVaultName.value

foreach ($service in $services) {
    $secretIdName = "$service--ClientId"

    # existing client id for service
    $clientId = az keyvault secret show --name $secretIdName --vault-name $vaultName --query value --output tsv

    if (-not $clientId) {
        Write-Host "Client id with name '$secretIdName' not found in Key Vault '$vaultName'."
        continue
    }

    $newPassword = az ad app credential reset --id $clientId --query password --output tsv
    if (-not $newPassword) {
        Write-Host "Failed to reset credentials for application $service with ClientId '$clientId'."
        continue
    }

    $newSecretName = "$service--ClientSecret"
    az keyvault secret set --name $newSecretName --vault-name $vaultName --value $newPassword | Out-Null
    
    Write-Host "Updated secret '$newSecretName' for service '$service' in Key Vault."
}

Write-Host "Client secret reset completed."
