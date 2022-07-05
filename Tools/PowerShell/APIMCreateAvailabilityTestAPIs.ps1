# Run the following commented out lines directly on command line to log on to Azure with OITSAGAHEARM0@va.gov, not within script file
## Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
## Connect-AzAccount -EnvironmentName AzureUSGovernment
## Select-AzSubscription DEVTEST-GOV-INTERNAL, PREPROD-GOV-INTERNAL

#.\APIMCreateAvailabilityTestAPIs.ps1 -env "dev"

 param (
    [string]$env
 )

if ($env -eq "prod-east") 
{
    $resourceGroupName = "veis-prod-gov-internal-int-east-api-rg"
    $apimName = "va-veis-prod-apim-east"
    $baseURLSuffix = "prod-east-udo.prod.vaec.va.gov"
}
elseif ($env -eq "prod-south") 
{
    $resourceGroupName = "veis-prod-gov-internal-int-south-api-rg"
    $apimName = "va-veis-prod-apim-south"
    $baseURLSuffix = "prod-south-udo.prod.vaec.va.gov"
}
elseif ($env -eq "nprod")
{
    $resourceGroupName = "veis-preprod-gov-internal-int-east-api-rg"
    $apimName = "va-veis-nprod-apim"
    $baseURLSuffix = "np-udo.nprod.vaec.va.gov/PREPROD"
}
elseif ($env -eq "dev")
{
    $resourceGroupName = "veis-devtest-gov-internal-int-east-api-rg"
    $apimName = "va-veis-dev-apim"
    $baseURLSuffix = "dev-udo.devtest.vaec.va.gov"
}

$LOBs = "Appeals", "Awards", "BIRLS", "CADD", "ClaimDocuments", "ClaimEstablishment", "Claims", "Contact", "Denials", "DependentMaintenance",
        "eMIS", "ExamAppointments", "Flashes", "FNOD", "IDProofOrchestration", "IntentToFile", "LegacyPayments", "Letters", "MilitaryService", "MVI",
        "Notes", "Payments", "PeoplelistPayeeCode", "Ratings", "ScheduleJob", "ServiceRequest", "SSRS", "UserTool", "VBMS", "VBMSeFolder", "VeteranSnapShot",
        "VirtualVA"

echo ("APIM update starting for APIM " + $apimName + "...")

$apiMgmtContext = New-AzApiManagementContext -ResourceGroupName $resourceGroupName -ServiceName $apimName

Foreach ($LOB in $LOBs)
{
    echo (" ")

    $apiAVTest = "UDO.LOB." + $LOB + "AVTest"
    echo ("...Creating Availability Test API " + $apiAVTest)
    $apiPath = "/" + "udo.lob." + $LOB.ToLower() + "AVTest"
    $serviceURL = "https://" + $LOB.ToLower() + "lob-" + $baseURLSuffix
    $newAPI = New-AzApiManagementApi -Context $apiMgmtContext -Name $apiAVTest -ServiceUrl $serviceURL -Protocols @("https") -Path $apiPath
    echo ("...Availability Test API " + $apiAVTest + " created.")
    
    echo ("......setting 'Subscription Required' to false")
    $apiTemp = Get-AzApiManagementApi -Context $apiMgmtContext -ApiId $newAPI.ApiId
    $apiTemp.SubscriptionRequired = $false
    Set-AzApiManagementApi -InputObject $apiTemp -PassThru
    echo ("......setting updated")

    echo (".........creating API Operation")
    New-AzApiManagementOperation -Context $apiMgmtContext -ApiId $newapi.apiid -OperationId "Ping" -Name "Ping" -Method "GET" -UrlTemplate "/api/Ping"
    echo (".........API Operation created")
}

echo ("APIM updates complete.")