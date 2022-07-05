# Run the following commented out line directly on command line to log on to Azure with OITSAGAHEARM0@va.gov, not within script file
## Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass

# cd C:\Development\crm-udo-code\Tools\PowerShell
# .\AppServiceUpdateAppSettings.ps1 -subscriptionName DEVTEST-GOV-INTERNAL -resourceGroupName VEIS-DEVTEST-GOV-INTERNAL-INT-EAST-API-RG -CSVLocation C:\Development\crm-udo-code\Tools\PowerShell\loadAppSettings.csv
 param (
    [Parameter(Mandatory=$true)]
    [string]$subscriptionName,
    [Parameter(Mandatory=$true)]
    [string]$resourceGroupName,
    [Parameter(Mandatory=$true)]
    [string]$CSVLocation
 )

Connect-AzAccount -EnvironmentName AzureUSGovernment
Select-AzSubscription $subscriptionName

Write-Host ("App Service update starting for Subscription " + $subscriptionName + " and resource group " + $resourceGroupName + "...")

# Import the contents of the csv file and store it in the $appSettingsList variable.
$appSettingsList = Import-Csv $CSVLocation

$currentAppName = "";
$newAppSettings = @{};
Foreach ($row in $appSettingsList)
{
    #If we need to get a new App Service, update the one that we have been iterating through and then query for the new one
    If ($row.AppName -ne $currentAppName)
    {
        #If this is not the first row being processed in the CSV, update the WebApp
        If($currentAppName -ne "")
        {
            Write-Host ("Updating " + $currentAppName + "'s App Settings in azure...")
            Set-AzWebApp -AppSettings $newAppSettings -Name $currentAppName -ResourceGroupName $resourceGroupName
            Write-Host ("Site configuration updated for " + $currentAppName)
        }

        # Retrieve site configuration for new web app
        Write-Host ("Retrieving site configuration for " + $row.AppName)
        $currentAppName = $row.AppName;
        $webApp = Get-AzWebApp -ResourceGroupName $resourceGroupName -Name $currentAppName;
        $existingAppSettings = $webApp.SiteConfig.AppSettings;
        $newAppSettings.Clear();
        #Add all existing App Settings to the newAppSettings variable so none are removed on update
        Foreach ($setting in $existingAppSettings)
        {
            $newAppSettings[$setting.Name] = $setting.Value
        }
    }

    Write-Host("...processing application setting: " + $row.SettingName)
    #Update new/existing app setting with whatever is provided in the csv file
    $newAppSettings[$row.SettingName] = $row.SettingValue
}

Write-Host ("App Service update complete for subscription " + $subscriptionName + " and resource group " + $resourceGroupName + ".")