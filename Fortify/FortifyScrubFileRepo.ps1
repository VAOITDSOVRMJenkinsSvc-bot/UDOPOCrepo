# Run the following commented out lines directly on command line
## Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass

$rootDIR = Read-Host "Enter the root folder directory to scrub or 'Enter' for <default> c:\fortify\UDO\crm-udo-code\ "

if ($rootDIR -eq '') {
    $rootDIR = "c:\fortify\UDO\crm-udo-code\"
}

$subDIRToDelete = ".git", ".github", "CRM UDO User Guides", "docs", "Fortify", "Releases", "Tools", "UDO.LOB\UDO.LOB.Address", "UDO.LOB\UDO.Crm.LOB.AddressApi", "UDO.LOB\UDO.Crm.LOB.MVIApi", "UDO.LOB\UDO.LOB.SSRS", "UDO.LCS", "UDO.Plugin.AttachFileToEmail", "UDO.Reports", "UDO.SSIS", "UDO.USD", "UDO.VASS", "UDO_Migration", "UDOLOBBackUp", "UDOPurge", "UDOSolutions", "UDO.D365\UDO.D365.Plugins", "UDO.D365\UDOD365Customizations", "UDO.D365\CrmPackage\WebResources\Data", "*.Tests", "UDO.Plugins\VA.VRMUD", "UDO.LOB\UDO.Crm.LOB", "UDO.LOB\UDO.LOB.ReportData", "UDO.LOB\UDO.LOB.Reports", "UDO.LOB\VEIS.Messages.v2", "UDO.LOB\UDO.LOB.VirtualVA\MigrationBackup", "UDO.LOB\UDO.LOB.eGain", "UDO.LOB\VEIS.ReportServerReportExecution", "UDO.Plugins\CustomActions.LogtoAppInsightsLogic", "UDO.Plugins\Va.Udo.Crm.Interactions", "UDO.Plugins\Va.Udo.Crm.Notes", "UDO.Plugins\Va.Udo.Crm.PCT", "UDO.Plugins\Va.Udo.Crm.eBenefits", "UDO.LOB\UDO.LOB.PersonSearch\UDO.LOB.PersonSearchApi"

foreach ($subDIR in $subDIRToDelete) {
    $deleteDIR = $rootDIR + $subDIR
    Remove-Item –path $deleteDIR -Recurse -Force -Verbose
    echo ("Deleted directory: " + $deleteDIR)
}

get-childitem $rootDIR *Tests* -Recurse | 
ForEach-Object { 
    Remove-Item $_.fullname -Recurse -Force -Confirm:$false -verbose 
} 
echo ("Deleted LOB Tests directories")

$rootFilesToDelete = "*.gitignore", "*.js", "*.zip", "*.fpr", "*.cs", "*.dll", "*.htm", "*.html", "*.config"

foreach ($file in $rootFilesToDelete) {
    $deleteFile = $rootDIR + $file
    Remove-Item –path $deleteFile -Force -Verbose
    echo ("Deleted root matching file(s): " + $deleteFile)
}

$allFilesToDelete = "*.zip", "*.doc", "*.docx", "*.txt", "*.jazzignore", "Web.Debug.config", "Web.Release.config", "Web.INT.config", "Web.PERF.config", "Web.PREPROD.config", "Web.QA.config", "Web.PROD.config", "Web.TRAIN.config", "profile.arm.json", "ApiCatalog.json"

foreach ($wildCardFile in $allFilesToDelete) {
    $deleteFilePath = $rootDIR + "*"
    Remove-Item –path $deleteFilePath -Recurse -Include $wildCardFile -Verbose
    echo ("Deleted all matching file(s): " + $wildCardFile)
}