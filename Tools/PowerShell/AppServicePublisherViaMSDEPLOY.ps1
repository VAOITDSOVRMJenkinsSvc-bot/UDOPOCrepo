# Run the following commented out lines directly on command line to log on to Azure with OITSAGAHEARM0@va.gov, not within script file
## Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
## Connect-AzAccount -EnvironmentName AzureUSGovernment
## Select-AzSubscription DEVTEST-GOV-INTERNAL

$msdeploy = "C:\Program Files (x86)\IIS\Microsoft Web Deploy V3\msdeploy.exe"
$resourceGroupName = “VEIS-DEVTEST-GOV-INTERNAL-INT-EAST-API-RG”
$webAppNames = “letterslob-dev-udo", "appealslob-dev-udo"
$rootDevFolder = "C:\Development\crm-udo-code\UDO.LOB"

echo ("App Service Publisher starting for resource group " + $resourceGroupName + "...")

# Retrieve publish profiles for each web app
Foreach ($webAppName in $webAppNames)
{
    echo (" ")

    $appDevFolder = ""

    switch ($webAppName)
    {
        'appealslob-dev-udo'
        {
            $appDevFolder = "\UDO.LOB.Appeals\UDO.LOB.AppealsApi"
        }
        'letterslob-dev-udo'
        {
            $appDevFolder = "\UDO.LOB.Letters\UDO.LOB.LettersApi"
        }
    }

    if ($appDevFolder -ne "")
    {
        # Get web application publish profile from Azure
        echo (".retrieving publish profile for " + $webAppName)
        $outputPublishProfile = $rootDevFolder + $appDevFolder + "\bin\" + $webAppName + ".publishsettings"
        $xml = Get-AzWebAppPublishingProfile -ResourceGroupName $resourceGroupName -Name $webAppName -Format "WebDeploy" -OutputFile $outputPublishProfile

        # Zip up web package for deployment
        echo ("..zipping content for " + $webAppName)
        $source = $rootDevFolder + $appDevFolder + "/bin/app.publish"
        $zipDestination = $rootDevFolder + $appDevFolder + "/bin/webapppublish.zip"
        del $zipDestination # Remove a previous copy
        Add-Type -assembly "system.io.compression.filesystem"
        [io.compression.zipfile]::CreateFromDirectory($source, $zipDestination)

        # Parse publish profile of web app from Azure
        echo ("...parsing publish profile for " + $webAppName)
        $username = ([xml]$xml).SelectNodes("//publishProfile[@publishMethod=`"MSDeploy`"]/@userName").value
        $password = ([xml]$xml).SelectNodes("//publishProfile[@publishMethod=`"MSDeploy`"]/@userPWD").value
        $url = ([xml]$xml).SelectNodes("//publishProfile[@publishMethod=`"MSDeploy`"]/@publishUrl").value
        $siteName = ([xml]$xml).SelectNodes("//publishProfile[@publishMethod=`"MSDeploy`"]/@msdeploySite").value

        $msdeployArguments = 
            '-verb:sync ' +
            "-source:package='$zipDestination' " + 
            "-dest:auto,ComputerName=https://$url/msdeploy.axd?site=$siteName,UserName=$username,Password=$password,AuthType='Basic',includeAcls='False' " +
            "-setParam:name=$webAppName,value=$siteName"
        $commandLine = '&"C:\Program Files (x86)\IIS\Microsoft Web Deploy V3\msdeploy.exe" --% ' + $msdeployArguments
        echo ($commandLine)

       
#$source = "-source:contentPath=$WebAppPath"
#$dest = "-dest:contentPath=d:\home\site\wwwroot\,publishSettings=$publishProfilePath"
#& $MSDeployPath @('-verb:sync', $source, $dest)

        # Push web application to Azure
        echo ("....deploying " + $webAppName)
 #       Invoke-Expression $commandLine

        echo (".....published " + $webAppName)
    }
    else 
    {
        echo ".skipping publish for " + $webAppName
    }
}

echo "App Service Publisher completed."
