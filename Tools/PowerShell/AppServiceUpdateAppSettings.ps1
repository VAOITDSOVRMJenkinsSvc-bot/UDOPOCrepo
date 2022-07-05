# Run the following commented out lines directly on command line to log on to Azure with OITSAGAHEARM0@va.gov, not within script file
## Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
## Connect-AzAccount -EnvironmentName AzureUSGovernment
## Select-AzSubscription DEVTEST-GOV-INTERNAL

# cd C:\Development\crm-udo-code\Tools\PowerShell
#.\AppServiceUpdateAppSettings.ps1 -resourceGroupName VEIS-DEVTEST-GOV-INTERNAL-INT-EAST-API-RG -webAppNames letterslob-dev-udo, appealslob-dev-udo

 param (
    [string]$resourceGroupName,
    [string[]]$webAppNames
 )

echo ("App Service update starting for resource group " + $resourceGroupName + "...")

Foreach ($webAppName in $webAppNames)
{
    echo (" ")
    # Retrieve site configuration for web app
    echo ("Retrieving site configuration for " + $webAppName)
    $webApp = Get-AzWebApp -ResourceGroupName $resourceGroupName -Name $webAppName
    $appSettings = $webApp.SiteConfig.AppSettings
    $newAppSettings = @{}
    #$appSettings | Format-Table -AutoSize

    Foreach ($appSetting in $appSettings)
    {
        echo("...processing application setting: " + $appSetting.Name)

        switch ($appSetting.Name)
        {
            'ClientId'
            {
                echo("......assigning ClientId")
                $appSetting.Value = "8fecdaff-0a69-42c1-9954-d604339824c1"
            }
            'ClientSecret'
            {
                echo("......assigning ClientSecret")
                $appSetting.Value = "CXxsZxLUPFoKvonB3IkZMmCg/4AkXKuABe0PPmhLjGA="
            }
            'Ocp-Apim-Subscription-Key'
            {
                echo("......assigning Ocp-Apim-Subscription-Key")
                $appSetting.Value = "88784cf9396e41dab087c90a5da0c2c1"
            }
            'OAuthClientId'
            {
                echo("......assigning OAuthClientId")
                $appSetting.Value = "58d50ca3-b921-4d9b-ac76-8965be2eb80b"
            }
            'OAuthResourceId'
            {
                echo("......assigning OAuthResourceId")
                $appSetting.Value = "4a77476c-ceed-45db-ad7e-ac2bbbc4f72a"
            }
        }

        # Update each name/value pair even if not in the switch statement so not to lose any of them in Azure
        $newAppSettings[$appSetting.Name] = $appSetting.Value
    }

    $newAppSettings | Format-Table -AutoSize


    # Updating site configuration for web app
    echo (".........updating application settings in Azure")
    Set-AzWebApp -Name $webAppName -ResourceGroupName $resourceGroupName -AppSettings $newAppSettings
    echo ("Site configuration updated for " + $webAppName)
}

echo ("App Service update complete for resource group " + $resourceGroupName + ".")