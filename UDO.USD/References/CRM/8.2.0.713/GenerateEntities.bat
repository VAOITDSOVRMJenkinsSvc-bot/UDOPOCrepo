@echo off

echo Enter CRM Username (domain\username)
set /p Username=

echo Enter CRM Password for your account (WARNING! Password is displayed in clear text!)
set /p Password=

::cd ".\8.2.0.713\"

CrmSvcUtil.exe  /connectionString:"Url=https://internalcrm.crmo15.dev.crm.vrm.vba.va.gov/UDODEV;UserName=%Username%;Password=%Password%;AuthenticationType=AD" /out:"..\..\..\UDO.Model\UDODataModel.cs" /namespace:UDO.Model /servicecontextname:UDOContext /generateActions /codewriterfilter:"MCSModelFilter.CodeWriterFilter,MCSModelFilter"

pause
