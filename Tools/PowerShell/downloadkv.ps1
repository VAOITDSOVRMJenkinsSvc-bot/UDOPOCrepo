param (
  [Parameter(Mandatory=$true)]
  [string]$CSVLocation = ""
)

#Connect to AzureUSGovernment
Connect-AZAccount -EnvironmentName AzureUSGovernment

$KeyVault = ""
# Select Key Vault
    $KeyVault_DEV = New-Object System.Management.Automation.Host.ChoiceDescription '&1-DEV', 'keyvault-dev-udo'
    $KeyVault_NP = New-Object System.Management.Automation.Host.ChoiceDescription '&2-NP', 'keyvault-np-udo'
    $KeyVault_PROD = New-Object System.Management.Automation.Host.ChoiceDescription '&3-PROD', 'keyvault-prod-udo'
    $KeyVaultArray = [System.Management.Automation.Host.ChoiceDescription[]]($KeyVault_DEV, $KeyVault_NP, $KeyVault_PROD)
    $title = 'Key Vault'
    $message = 'What is the Key Vault you are targeting?'
    $KeyVault_result = $host.ui.PromptForChoice($title, $message, $KeyVaultArray, 0)
    #Update these values with the proper Key Vault names/environments
    switch ($KeyVault_result)
    {
        0 { 
            $KeyVault.value = "keyvault-dev-udo"
          }
        1 { 
            $KeyVault.value = "keyvault-np-udo"
          }
        2 { 
            $KeyVault.value = "keyvault-prod-udo"
          }
    }

$secrets = Get-AzKeyVaultSecret -VaultName $KeyVault

$csvArray = @()
foreach ($secret in $secrets)
{
    $secretValue = Get-AzKeyVaultSecret -VaultName $KeyVault -Name $secret.Name -AsPlainText
    $csvArray += @{"Name" = $secret.Name; "Value" = $secretValue }
}

$csvArray | Export-Csv $CSVLocation -NoTypeInformation
