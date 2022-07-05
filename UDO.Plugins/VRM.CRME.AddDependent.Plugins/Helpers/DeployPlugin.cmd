echo nothing
net stop MSCRMAsyncService
net stop MSCRMAsyncService$maintenance
net stop MSCRMSandboxService

iisreset

xcopy "C:\developmentRC\Amalgam\CRME\Add Dependent\Plugins\bin\Debug\VRM.CRME.Plugin.DependentMaintenance.dll" "C:\Program Files\Microsoft Dynamics CRM\Server\bin\assembly" /r/y
xcopy "C:\developmentRC\Amalgam\CRME\Add Dependent\Plugins\bin\Debug\VRM.CRME.Plugin.DependentMaintenance.pdb" "C:\Program Files\Microsoft Dynamics CRM\Server\bin\assembly" /r/y

net start MSCRMAsyncService
net start MSCRMAsyncService$maintenance
net start MSCRMSandboxService

pause