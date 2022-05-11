# tool and utility functions

class DeploymentSpecification
{    
        [string] $Title
        [string] $Description
        [string] $TenantId
        [string] $SubscriptionIds
        [string] $OwnerUserId
        [string] $ProductName
        [string] $ProductCode
        [string] $DomainName
        [string] $Location
        [string[]] $EnvironmentNames
        [int] $InstancesPerEnvironment
        [string[]] $ServiceNames
        [string] $CommonResourceTemplate
        [string] $ServiceResourceTemplate    
}

class CustomException : Exception {
    [string] $additionalData

    CustomException($Message, $additionalData) : base($Message) {
        $this.additionalData = $additionalData
    }
}
function Get-ObjectFromString{    
    # Deserialises a JSON string to an object
    Param( [Parameter(Mandatory)][ArgumentCompleter({'{ "someValue": "SomeData"}'})][string] $inputValue)    
    $outputValue = ConvertFrom-Json -InputObject $inputValue
    return $outputValue
}    
function Get-ObjectFromStringArray{    
    # handles string object arrays returned like from the az command
    Param( [Parameter(Mandatory)][ArgumentCompleter({'[{ "someValue":','"SomeData"}]'})][string[]] $inputValue)    
    $value =  [string]$inputValue     
    return Get-ObjectFromString -inputValue $value
}    

function Get-ObjectFromObjectArray{    
    # handles string object arrays returned like from the az command
    Param( [Parameter(Mandatory)][ArgumentCompleter({'[{ "someValue":','"SomeData"}]'})][object[]] $inputValue)    
    $value =  [string]$inputValue     
    return Get-ObjectFromString -inputValue $value
}    
function Get-Provisioned {
    param (
        [object] $response
    )
    return ("Succeeded" -eq $response.properties.provisioningState)
}
function Get-Deployed {
    param (
        [object] $response
    )
    return ("Succeeded" -eq $response.properties.deploymentStatus)
}
function Deploy-ProductCommonResources{
    Param ( 
            [Parameter(Mandatory)][string] $resourceGroupName,
            [Parameter(Mandatory)][string] $environmentName,
            [Parameter(Mandatory)][hashtable] $tags,
            [Parameter(Mandatory)][string] $productCode ,
            [Parameter(Mandatory)][string] $productName,
            [Parameter(Mandatory)][string] $commonResourceTemplate,
            [Parameter(Mandatory)][string] $tenantid,
            [Parameter(Mandatory)][string] $owneruserid
        )            
        
    Write-Host "Validating '$commonResourceTemplate' ... "

    $tagString = Get-JsonStringFromHashtable -tags $tags
    
    $response = az deployment group validate `
        --resource-group $resourceGroupName `
        --template-file $commonResourceTemplate `
        --parameters  "adminUser=$owneruserid" "tenantid=$tenantid" "environment=$environmentName" "productCode=$productCode" "tags=$tagString"
        
    $result = Get-ObjectFromStringArray -inputValue $response
    $deployed = Get-Provisioned $result

    if ($deployed)
    {
        Write-Host "Deploying '$commonResourceTemplate' ... "
        $response = az deployment group create `
            --resource-group $resourceGroupName `
            --template-file $commonResourceTemplate `
            --parameters  "adminUser=$owneruserid" "tenantid=$tenantid" "environment=$environmentName"  "productCode=$productcode" "tags=$tagString"
        
        $result = Get-ObjectFromStringArray -inputValue $response
        $deployed = Get-Provisioned $result
    }
    else {
        throw  [CustomException]::new("The COMMON template failed validation", "Please fix the common template and re-try.")
    }
    return $deployed
}
function Deploy-ProductServiceResources {

    Param (
            [Parameter(Mandatory)][string] $resourceGroupName,
            [Parameter(Mandatory)][string] $environmentName,
            [Parameter(Mandatory)][string] $serviceName,
            [Parameter(Mandatory)][int] $instance,
            [Parameter(Mandatory)][hashtable] $tags,
            [Parameter(Mandatory)][string] $productCode ,
            [Parameter(Mandatory)][string] $productName,
            [Parameter(Mandatory)][string] $serviceResourceTemplate
        )       
        
        Write-Host "Validating '$($serviceResourceTemplate)' ... "

        $tagString = Get-JsonStringFromHashtable -tags $tags

        $productCode = $spec.ProductCode
        
        $response = az deployment group validate `
            --resource-group $resourceGroupName `
            --template-file $serviceResourceTemplate `
            --parameters  "environment=$environmentName" "instance=$instance" "serviceName=$serviceName" "productCode=$productCode" "tags=$tagString"
            
        $result = Get-ObjectFromStringArray -inputValue $response
        $deployed = Get-Provisioned $result
    
        if ($deployed)
        {
            Write-Host "Deploying '$($serviceResourceTemplate)' as $($serviceName)-deployment ... "
            $response = az deployment group create `
                --name "$($serviceName)-deployment" `
                --resource-group $resourceGroupName `
                --template-file $serviceResourceTemplate `
                --parameters "environment=$environmentName" "instance=$instance" "serviceName=$serviceName" "productCode=$productcode" "tags=$tagString"
            
            $result = Get-ObjectFromStringArray -inputValue $response
            $deployed = Get-Provisioned $result
        }
        else {
            throw  [CustomException]::new("The SERVICE template failed validation", "Please fix the service template and re-try.")
        }
        return $deployed
}
function Get-Authenticated {
    $response = az login
    return Get-ObjectFromStringArray -inputValue $response
}
function Get-UsersForAccount {
    $response = az ad user list --output json
    return Get-ObjectFromStringArray -inputValue $response
}
function Get-AccountOwnerUser{
    Param( 
            [Parameter(Mandatory)][object] $users,
            [Parameter(Mandatory)][object] $account
            )

    return $users | select-object tenantId, objectId, otherMails | where-object {$_.othermails -contains $account.user.name}
}
function Get-ResourceGroup {
    param (
        [Parameter(Mandatory)] [string]$resourceGroupName
    )
    $response = az group show --resource-group $resourceGroupName
    if ($null -eq $response) 
    {
        return $false
    }
    else {        
        return $true
    }
}

function Add-ResourceGroup {
    param (
        [Parameter(Mandatory)][string] $resourceGroupName, 
        [Parameter(Mandatory)][string] $location,
        [Parameter(Mandatory)][hashtable] $tags
    )
    $tagString = $null
    $tags.getEnumerator() | ForEach-Object {        
        if ($tagString.Length -gt 0)
        {
            $tagString +=  " "    
        }
        $tagString +=  $_.key + "=" + $_.value          
    }    
    $response = az group create --resource-group $resourceGroupName --location $location --tags $tagString.split(' ')
    # hack due to AZ problems with Python
    if ($response -match "Failed to load python executable." )
    {
       $newArray = $response[0..($response.Length-6)]
       $response = $newArray
    }

    $result = Get-ObjectFromObjectArray -inputValue $response       
    $success = Get-Provisioned $result
    return $success
}

function Set-ResourceGroup {
    param (
        [Parameter(Mandatory)][string] $resourceGroupName, 
        [Parameter(Mandatory)][hashtable] $tags
    )
    $tagString = $null
    $tags.getEnumerator() | ForEach-Object {        
        if ($tagString.Length -gt 0)
        {
            $tagString +=  " "    
        }
        $tagString +=  $_.key + "=" + $_.value          
    }    
    $response = az group update --resource-group $resourceGroupName  --tags $tagString.split(' ')
    # hack due to AZ problems with Python
     if ($response -match "Failed to load python executable." )
     {
        $newArray = $response[0..($response.Length-6)]
        $response = $newArray
     }

    $result = Get-ObjectFromObjectArray -inputValue $response       
    $success = Get-Provisioned $result
    return $success
}
function Get-JsonStringFromHashtable {
    param (
        [Parameter(Mandatory)][hashtable] $tags,
        [string]$delimiter = ",",
        [string]$quote = "'"
        
    )
    $tagString = "{ "
    $tags.getEnumerator() | ForEach-Object { 
        if ($tagString.Length -gt 2)
        {
            $tagString +=  $delimiter   
        }
        $tagString += $quote + $_.key + "$($quote):" + $quote + $_.value + $quote
    }    
    $tagString += "}"
    return $tagString
}