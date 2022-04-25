    # goes interactive to get logged in.
if ($null -eq $accountInfo)
{
    $accountInfo = az login
}

$accountstringarray = [string]$accountInfo
$accountitem = ConvertFrom-Json -inputobject $accountstringarray
$tenantid = $accountitem.tenantId
# getting users for subscription
$userstringarray = az ad user list --output json
$userstring = [string]$userstringarray
$useritems = ConvertFrom-Json -inputobject $userstring

# finding current admin user
$currentUser = $userItems | select-object tenantId, objectId, otherMails | where-object {$_.othermails -contains $accountitem.user.name}
$adminUserId = $currentUser.objectId

$environment = Read-Host -Prompt "Environment name (team|test|prod)" 
$product = "miw"
$productName = "myinfoworld"
$resourceGroupName = $product + '-' + $environment + '-' + 'rg'
$location = "ukwest"


$resourceGroup =  az group show --resource-group $resourceGroupName
Write-Host "Tenant Id  [$tenantid]"
Write-Host "Admin User Id [$adminUserId]"
Write-Host "Location [$location]"
Write-Host "Resource Group [$resourceGroup]"

if ($null -eq $resourceGroup)
{
    Write-Host 'Creating new resource group'
    # New-AzResourceGroup -Name $resourceGroupName  -Location $location -Tag @{Empty=$null; Department="technical"}
    az group create --resource-group $resourceGroupName --location $location
}

Write-Host "--parameters adminUser=$adminUserId tenantid=$tenantid environment=$environment product=$product productName=$productName"

# now deploy the common features
Write-Host 'Validating common.template.json ... '
    
$validation = az deployment group validate --resource-group $resourceGroupName --template-file .\common-template.json --parameters adminUser=$adminUserId tenantid=$tenantid environment=$environment product=$product productName=$productName
if ($null -ne $validation && $null -eq $validation.error )
{
    Write-Host 'Deployng common.template.json ... '
    $deployment = az deployment group create --resource-group $resourceGroupName --template-file .\common-template.json --parameters adminUser=$adminUserId tenantid=$tenantid environment=$environment product=$product productName=$productName
    Write-Host $deployment.properties.provisioningState
}
# now deploy the services
