$deploymentName = 'LongRunningFunctionApp' + (Get-Date -UFormat "%s")

$parameters = {

}

az login

az deployment group create `
    --name $deploymentName `
    --resource-group rg-longRunningFunctionApp `
    --template-file .\azuredeploy.json `
    --parameters azuredeploy.parameters.json