
param (
    [Parameter(Mandatory=$true)]
    [string]
    $subscriptionName
)

Connect-AzAccount -EnvironmentName AzureUSGovernment
Select-AzSubscription $subscriptionName

$webApps = Get-AzWebApp | Where-Object {$_.Name.IndexOf("prod-udo") -gt -1}

foreach ($webApp in $webApps) {
    $webApp = Get-AzWebApp -Name $webApp.Name -ResourceGroupName $webApp.ResourceGroup
    $appSettings = $webApp.SiteConfig.AppSettings
    $newAppSettings = @{}

    Write-Host "Removing App Settings from " $webApp.Name -ForegroundColor Green

    foreach ($appSetting in $appSettings) {
        switch ($appSetting.Name)
        {
            'ClientId'
            {
                Write-Host "    Removing Client Id" -ForegroundColor Green
            }
            'ClientSecret'
            {
                Write-Host "    Removing Client Secret" -ForegroundColor Green
            }
            'ECUri'
            {
                Write-Host "    Removing ECUri" -ForegroundColor Green
            }
            'LobApimUri'
            {
                Write-Host "    Removing LobApimUri" -ForegroundColor Green
            }
            'Ocp-Apim-Subscription-Key'
            {
                Write-Host "    Removing Ocp-Apim-Subscription-Key" -ForegroundColor Green
            }
            'Ocp-Apim-Subscription-Key-S'
            {
                Write-Host "    Removing Ocp-Apim-Subscription-Key-S" -ForegroundColor Green
            }
            'Ocp-Apim-Subscription-Key-E'
            {
                Write-Host "    Removing Ocp-Apim-Subscription-Key-E" -ForegroundColor Green
            }
            'OAuthClientId'
            {
                Write-Host "    Removing OAuthClientId" -ForegroundColor Green
            }
            'OAuthClientID'
            {
                Write-Host "    Removing OAuthClientID" -ForegroundColor Green
            }
            'OAuthClientSecret'
            {
                Write-Host "    Removing OAuthClientSecret" -ForegroundColor Green
            }
            'OAuthResourceId'
            {
                Write-Host "    Removing OAuthResourceId" -ForegroundColor Green
            }
            default
            {
                $newAppSettings[$appSetting.Name] = $appSetting.Value
            }
        }        
    }

    Write-Host "Updating App Settings back to Azure for web app " $webApp.Name -ForegroundColor Green

    Set-AzWebApp -Name $webApp.Name -ResourceGroupName $webApp.ResourceGroup -AppSettings $newAppSettings
}
