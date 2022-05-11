# import interactive, tools
. .\spec-functions.ps1

$specFile = ".\spec.json"

try {
   
    $ErrorActionPreference = "STOP"
    $error.Clear()
    
    # first test the spec file exists. if not prompt for it then verify it
    if (Test-Path -Path $specFile -PathType Leaf)
    {
        $spec = [DeploymentSpecification] (Get-Content -Raw -Path $specFile | ConvertFrom-Json)
        
        if ($null -eq $spec)
        {
            throw [CustomException]::new( "Failed to deserialize the spec file",  "Please provide a valid json file.")
        }        
        
        # Authenticate - goes ineractive
        $account = Get-Authenticated
        # Get owning user information from the account
        $users = Get-UsersForAccount         
        $ownerUser = Get-AccountOwnerUser -users $users -account $account
        $spec.SubscriptionIds =   @($account.id) 
        $spec.TenantId = $account.tenantId
        $spec.OwnerUserId = $ownerUser.objectId      

        #
        # Begin making changes, validate as you go.
        #

        Write-Host "Deploying singleton tenant dependency resources "        
        # log analytics workspaces, app insights, configuration etc.

        # now handle the product level resources, common then service
        foreach($environmentName in $spec.EnvironmentNames)
        {
            $resourceGroupName = $spec.ProductCode.ToLower() + '-' + $environmentName.ToLower() + '-'  + 'rg'

            $tags = @{Department="Operations"; ProductName=$spec.ProductName ; ProductCode=$spec.ProductCode ; Environment=$environmentName; }
            
            $response =  Get-ResourceGroup -resourceGroupName $resourceGroupName

            if ($response)
            {
                Write-Host "Updating exiting resource group $resourceGroupName"
                $deployed = Set-ResourceGroup -resourceGroupName $resourceGroupName  -tags $tags               
            }
            else {
                Write-Host "Creating new resource group $resourceGroupName"
                $deployed = Add-ResourceGroup -resourceGroupName $resourceGroupName `
                                                -location $spec.Location `
                                                -tags $tags
            }

            if ($deployed)
            {
                $operation = "COMMON recources to environment Name: '$environmentName' for Instance: '$instance'..."
                   
                Write-Host "Deploying $($operation)"

                $deployed =  Deploy-ProductCommonResources $resourceGroupName  `
                                                            $environmentName  `
                                                            $tags `
                                                            $spec.productCode `
                                                            $spec.productName `
                                                            $spec.CommonResourceTemplate `
                                                            $spec.TenantId `
                                                            $spec.OwnerUserId
                
                if ($deployed)
                {
                    Write-Host "Deployed $($operation)"
                    # now the service instances
                    for ($instance = 1; $instance -le $spec.InstancesPerEnvironment; $instance++) {
                        foreach($serviceName in $spec.ServiceNames)
                        {
                            $operation = "SERVICE recources to environment Name: '$environmentName' for Service: '$serviceName' Instance: '$instance'..."
                            $tags = @{Department="Operations"; ProductName=$spec.ProductName ; ProductCode=$spec.ProductCode ; Environment=$environmentName; Instance=$instance }
            
    
                            Write-Host "Deploying $($operation)"
                            
                            $deployed =  Deploy-ProductServiceResources $resourceGroupName  `
                                                                        $environmentName  `
                                                                        $serviceName `
                                                                        $instance  `
                                                                        $tags `
                                                                        $spec.productCode `
                                                                        $spec.productName `
                                                                        $spec.ServiceResourceTemplate 
                            if ($deployed)
                            {
                                Write-Host "Deployed $($operation)"
                            } 
                            else {
                                throw [CustomException]::new(
                                    "Failed in $($operation)",
                                    "Please correct the problem and retry")
                            }
                        }
                    }
                }
                else {
                    throw [CustomException]::new( "Failed to deploy the common recources : '$environmentName'","Please correct the problem and retry")
                }
            }
            else {
                throw  [CustomException]::new( "Failed to deploy the resource group $resourceGroupName","Please correct the problem and retry")
            }
            Write-Host "Deployment Finished for '$environmentName'"
        }
        Write-Host "Deployment Finished."
    }
    else {
        throw [CustomException]::new( "The spec file '$specFile' was not found",  "Please specifiy a valid file location.")
    }
}
catch [System.Net.WebException],[System.IO.IOException] {
    Write-Host "An IO Exception has occured with the file system or network access. '$($error)'"
}
catch [CustomException] {
   # NOTE: To access your custom exception you must use $_.Exception
   Write-Host "$($_.Exception.Message),  $($_.Exception.additionalData)" 
}
catch {
    Write-Host "An error occurred that could not be resolved. '$($error)'"
}