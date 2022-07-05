# Run the following commented out lines directly on command line to log on to Azure with OITSAGAHEARM0@va.gov, not within script file
## Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
## Connect-AzAccount -EnvironmentName AzureUSGovernment
## Select-AzSubscription DEVTEST-GOV-INTERNAL 

#.\AppServiceSetARRAffinityOff.ps1 -resourceGroupName VEIS-DEVTEST-GOV-INTERNAL-INT-EAST-API-RG -environment dev

 param (
    [string]$resourceGroupName,
    [string]$environment
 )

$LOBs = "appeals", "awards", "birls", "cadd", "claimdocuments", "claimestablishment", "claims", "contact", "denials", "dependentmaintenance",
        "emis", "examappointments", "flashes", "fnod", "idprooforchestration", "intenttofile", "legacypayments", "letters", "militaryservice", "mvi",
        "notes", "payments", "peoplelistpayeecode", "ratings", "schedulejob", "servicerequest", "ssrs", "usertool", "vbmsefolder", "vbms", "veteransnapshot",
        "virtualva"

echo ("App Service update starting for resource group " + $resourceGroupName + "...")

Foreach ($webAppName in $LOBs)
{
    echo (" ")
    
    $webAppName = $webAppName + "lob-" + $environment + "-udo"

    # Retrieve site configuration for web app
    echo ("Retrieving site configuration for " + $webAppName)
    $webApp = Get-AzWebApp -ResourceGroupName $resourceGroupName -Name $webAppName
    $affinity = $webApp.ClientAffinityEnabled
    echo ("...current setting for 'ClientAffinityEnabled' = " + $affinity)

    $affinity = $false
    $webApp.ClientAffinityEnabled = $affinity # turn ARR Affinity Off

    # Updating site configuration for web app
    echo ("......updating 'ClientAffinityEnabled' to " + $affinity)
    $webApp = Set-AzWebApp -WebApp $webApp -ErrorAction Stop
    echo ("Site configuration updated for " + $webAppName)
}

echo ("App Service update complete for resource group " + $resourceGroupName + ".")