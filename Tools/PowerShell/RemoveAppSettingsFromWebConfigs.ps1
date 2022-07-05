function GetTarget([ref] $TargetEnvironment, [ref] $TargetRegion, [ref] $TargetFolder)
{
    # TARGET

    # TARGET ENVIRONMENT
    $TargetEnv_DEV = New-Object System.Management.Automation.Host.ChoiceDescription '&1-DEV', 'DEV'
    $TargetEnv_NP = New-Object System.Management.Automation.Host.ChoiceDescription '&2-NP', 'NP'
    $TargetEnv_PROD = New-Object System.Management.Automation.Host.ChoiceDescription '&3-PROD', 'PROD'
    $TargetEnv = [System.Management.Automation.Host.ChoiceDescription[]]($TargetEnv_DEV, $TargetEnv_NP, $TargetEnv_PROD)
    $title = 'Target Environment'
    $message = 'What is the Target environment?'
    $TargetEnv_result = $host.ui.PromptForChoice($title, $message, $TargetEnv, 0)
    switch ($TargetEnv_result)
    {
        0 {
            $TargetEnvironment.value = "DEV"
          }
        1 { 
            $TargetEnvironment.value = "NP"
          }
        2 { 
            $TargetEnvironment.value = "PROD"
          }
    }

    # REGION
    $TargetRegion_EAST = New-Object System.Management.Automation.Host.ChoiceDescription '&1-EAST', 'EAST'
    $TargetRegion_SOUTH = New-Object System.Management.Automation.Host.ChoiceDescription '&2-SOUTH', 'SOUTH'
    $TargetReg = [System.Management.Automation.Host.ChoiceDescription[]]($TargetRegion_EAST, $TargetRegion_SOUTH)
    $title = 'Target Region'
    $message = 'What is the Target region?'
    $TargetRegion_result = $host.ui.PromptForChoice($title, $message, $TargetReg, 0)
    switch ($TargetRegion_result)
    {
        0 { 
            $TargetRegion.value = "EAST"
          }
        1 { 
            $TargetRegion.value = "SOUTH"
          }
    }

    # Target FOLDER
    $TargetFolder_wwwrooot = New-Object System.Management.Automation.Host.ChoiceDescription '&1-wwwroot', 'wwwroot'
    $TargetFolder_wwwrooot_1 = New-Object System.Management.Automation.Host.ChoiceDescription '&2-wwwroot_1', 'wwwroot_1'
    $TargetFolder_wwwrooot_2 = New-Object System.Management.Automation.Host.ChoiceDescription '&3-wwwroot_2', 'wwwroot_2'
    $TargetFolder_wwwrooot_int = New-Object System.Management.Automation.Host.ChoiceDescription '&4-wwwroot_int', 'wwwroot_int'
    $TargetFolder_wwwrooot_intalt = New-Object System.Management.Automation.Host.ChoiceDescription '&5-wwwroot_intalt', 'wwwroot_intalt'
    $TargetFolder_wwwrooot_qa = New-Object System.Management.Automation.Host.ChoiceDescription '&6-wwwroot_qa', 'wwwroot_qa'
    $TargetFolder_wwwrooot_qauci = New-Object System.Management.Automation.Host.ChoiceDescription '&7-wwwroot_qauci', 'wwwroot_qauci'
    $TargetFolder_wwwrooot_preprod = New-Object System.Management.Automation.Host.ChoiceDescription '&8-wwwroot_preprod', 'wwwroot_preprod'
    $TargetFolder_wwwrooot_preprodalt = New-Object System.Management.Automation.Host.ChoiceDescription '&9-wwwroot_preprodalt', 'wwwroot_preprodalt'

    $TargetFold = [System.Management.Automation.Host.ChoiceDescription[]]($TargetFolder_wwwrooot, $TargetFolder_wwwrooot_1, $TargetFolder_wwwrooot_2, $TargetFolder_wwwrooot_int, 
                    $TargetFolder_wwwrooot_intalt, $TargetFolder_wwwrooot_qa, $TargetFolder_wwwrooot_qauci, $TargetFolder_wwwrooot_preprod, $TargetFolder_wwwrooot_preprodalt)

    $title = 'Target Folder'
    $message = 'What is the Target folder?'
    $TargetFolder_result = $host.ui.PromptForChoice($title, $message, $TargetFold, 0)
    switch ($TargetFolder_result)
    {
        0 { 
            $TargetFolder.value = "wwwroot"
          }
        1 { 
            $TargetFolder.value = "wwwroot_1"
          }
        2 { 
            $TargetFolder.value = "wwwroot_2"
          }
        3 { 
            $TargetFolder.value = "wwwroot_int"
          }
        4 { 
            $TargetFolder.value = "wwwroot_intalt"
          }
        5 { 
            $TargetFolder.value = "wwwroot_qa"
          }
        6 { 
            $TargetFolder.value = "wwwroot_qauci"
          }
        7 { 
            $TargetFolder.value = "wwwroot_preprod"
          }
        8 { 
          $TargetFolder.value = "wwwroot_preprodalt"
        }
    }
}

$TargetEnvironment = ""
$TargetRegion = ""
$TargetFolder = ""

GetTarget ([ref] $TargetEnvironment) ([ref] $TargetRegion) ([ref] $TargetFolder)
        
$currentDir = Get-Location
$targetRootString = [string]::Join('\', $currentDir, "\DeployAppServices\WebConfig", $TargetEnvironment, $TargetRegion, $TargetFolder)
$targetRootDir = dir targetRootString -Directory

foreach ($dir in $targetRootDir)
{
    $webConfigLocation = [string]::Join('\', $dir.fullname, "web.config")
    $doc = [xml] (get-content $webConfigLocation)
    $appSettings = $doc.configuration.appSettings

    $doc.configuration.RemoveChildNode($appSettings)

    $doc.Save($webConfig)
    
    Write-Host $dir.Name " removed app settings node in web.config"
}