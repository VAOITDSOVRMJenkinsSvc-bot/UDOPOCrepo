net stop MSCRMAsyncService
net stop MSCRMAsyncService$maintenance
net stop MSCRMSandboxService

iisreset

xcopy ".\bin\Debug\VRM.CRME.Plugin.DependentMaintenance.dll" "D:\Program Files\Microsoft Dynamics CRM\Server\bin\assembly" /r/y
xcopy ".\bin\Debug\VRM.CRME.Plugin.DependentMaintenance.pdb" "D:\Program Files\Microsoft Dynamics CRM\Server\bin\assembly" /r/y

net start MSCRMAsyncService
net start MSCRMAsyncService$maintenance
net start MSCRMSandboxService


