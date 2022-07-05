# =====================================================================
#  This file is part of the Microsoft Dynamics CRM SDK code samples.
#
#  Copyright (C) Microsoft Corporation.  All rights reserved.
#
#  This source code is intended only as a supplement to Microsoft
#  Development Tools and/or on-line documentation.  See these other
#  materials for detailed information regarding Microsoft code samples.
#
#  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
#  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
#  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
#  PARTICULAR PURPOSE.
# =====================================================================

#======================================================================
#Registers or Unregisters the Microsoft.Xrm.Tooling.CrmConnector.Powershell.dll and the Microsoft.Xrm.Tooling.PackageDeployment.Powershell.dll if they are present
#======================================================================

param
(
[switch]$uninstall
)

If (-NOT ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator"))
{   
    Write-Error("This script must be run in admin mode, please restart powershell and run it as an administrator");
break
}


#setup the alias I need. 
Set-Alias installUtil $env:windir\Microsoft.NET\Framework64\v4.0.30319\installutil.exe

if ( $uninstall ) 
{
    #unregister components:
    if ( (Get-Item("Microsoft.Xrm.Tooling.CrmConnector.Powershell.Dll") -ErrorAction SilentlyContinue) -ne $null ) 
    {
        installUtil /u Microsoft.Xrm.Tooling.CrmConnector.Powershell.Dll
    }
    else
    {
        Write-Host "Did Not find Microsoft.Xrm.Tooling.CrmConnector.Powershell.Dll" -ForegroundColor Red
    }
    if ( (Get-Item("Microsoft.Xrm.Tooling.PackageDeployment.Powershell.Dll") -ErrorAction SilentlyContinue) -ne $null ) 
    {
        installUtil /u Microsoft.Xrm.Tooling.PackageDeployment.Powershell.Dll
    }    
    else
    {
       Write-Host "Did Not Find Microsoft.Xrm.Tooling.PackageDeployment.Powershell.Dll" -ForegroundColor Red
    }    
}
else
{
    #register components:
    if ( (Get-Item("Microsoft.Xrm.Tooling.CrmConnector.Powershell.Dll") -ErrorAction SilentlyContinue ) -ne $null ) 
    {
        Write-Host "Found Microsoft.Xrm.Tooling.CrmConnector.Powershell.Dll" -ForegroundColor Yellow
        installUtil Microsoft.Xrm.Tooling.CrmConnector.Powershell.Dll
    }
    else
    {
        Write-Host "Did Not find Microsoft.Xrm.Tooling.CrmConnector.Powershell.Dll" -ForegroundColor Red
    }
    if ( (Get-Item("Microsoft.Xrm.Tooling.PackageDeployment.Powershell.Dll") -ErrorAction SilentlyContinue ) -ne $null ) 
    {
        Write-Host "Found Microsoft.Xrm.Tooling.PackageDeployment.Powershell.Dll" -ForegroundColor Yellow
        installUtil Microsoft.Xrm.Tooling.PackageDeployment.Powershell.Dll
    }
    else
    {
       Write-Host "Did Not Find Microsoft.Xrm.Tooling.PackageDeployment.Powershell.Dll" -ForegroundColor Red
    }    
}
