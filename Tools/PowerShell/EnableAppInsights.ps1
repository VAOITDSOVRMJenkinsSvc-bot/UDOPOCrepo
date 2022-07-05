[CmdletBinding()]
param (
    [Parameter(Mandatory = $True)]
    [String]
    $csvLocation,
    [Parameter(Mandatory = $True)]
    [String]
    $resourceGroupName,
    [Parameter(Mandatory = $True)]
    [String]
    $appInsightsInstrumentationKey,
    [Parameter(Mandatory = $True)]
    [String]
    $subscriptionName
)

Connect-AzAccount -EnvironmentName AzureUSGovernment
Select-AzSubscription $subscriptionName

$csv = Import-Csv -Path $csvLocation

#Get instrumentation key from ENV application insights resource
#$appInsightsInstrumentationKey = (Get-AzApplicationInsights -Name $appInsightsName -ResourceGroupName $resourceGroupName).InstrumentationKey

foreach ($row in $csv) {
    $resourceNameString = $row.LobName + "/Microsoft.ApplicationInsights.AzureWebSites"
    New-AzResource -ResourceType "Microsoft.Web/sites/siteextensions" -ResourceGroupName $resourceGroupName -Name $resourceNameString -Force -ErrorAction Stop

    #Set the appseting to send telemetry to common applicaiton insights.
    $webApp = Get-AzWebApp -ResourceGroupName $resourceGroupName -Name $row.LobName
    $webAppSettings = $webApp.SiteConfig.AppSettings
    $hash = @{ }
    Write-Host "Clearing hash table" -ForegroundColor Green
    foreach ($setting in $webAppSettings) {
    $hash[$setting.Name] = $setting.Value
    }
    $hash['APPINSIGHTS_INSTRUMENTATIONKEY'] = "$($appInsightsInstrumentationKey)" #its important to include the syntax around the variable eg. "$($var)"" if not supplied like this it will change the hash table's object type.
    $hash['ApplicationInsightsAgent_EXTENSION_VERSION'] = "~2"
    $hash['XDT_MicrosoftApplicationInsights_Mode'] = "recommended"
    $hash['APPINSIGHTS_PROFILERFEATURE_VERSION'] = "disabled"
    $hash['DiagnosticServices_EXTENSION_VERSION'] = "disabled"
    $hash['APPINSIGHTS_SNAPSHOTFEATURE_VERSION'] = "disabled"
    $hash['SnapshotDebugger_EXTENSION_VERSION'] = "disabled"
    $hash['InstrumentationEngine_EXTENSION_VERSION'] = "disabled"
    $hash['XDT_MicrosoftApplicationInsights_BaseExtensions'] = "disabled"
    #Write back app settings into web app
    Write-Host "Writing back updated appsettings to app service" $row.LobName -ForegroundColor Green
    Set-AzWebApp -AppSettings $hash -Name $row.LobName -ResourceGroupName $resourceGroupName -verbose
    
    Restart-AzWebApp -ResourceGroupName $resourceGroupName -Name $row.LobName
}