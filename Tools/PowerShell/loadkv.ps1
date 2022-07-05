param (
  [Parameter(Mandatory=$true)]
  [string]$CSVLocation,
  [Parameter(Mandatory=$true)]
  [string]$subscriptionName
)

#Connect to AzureUSGovernment
Connect-AZAccount -EnvironmentName AzureUSGovernment
Select-AzSubscription $subscriptionName

$KeyVault = ""
# Select Key Vault
    $KeyVault_DEV = New-Object System.Management.Automation.Host.ChoiceDescription '&1-DEV', 'DEV'
    $KeyVault_NP = New-Object System.Management.Automation.Host.ChoiceDescription '&2-NP', 'NP'
    $KeyVault_PROD_E = New-Object System.Management.Automation.Host.ChoiceDescription '&3-PROD-E', 'PROD-EAST'
    $KeyVault_PROD_S = New-Object System.Management.Automation.Host.ChoiceDescription '&4-PROD-S', 'PROD-SOUTH'
    $KeyVaultArray = [System.Management.Automation.Host.ChoiceDescription[]]($KeyVault_DEV, $KeyVault_NP, $KeyVault_PROD_E, $KeyVault_PROD_S)
    $title = 'Key Vault'
    $message = 'What is the Key Vault you are targeting?'
    $KeyVault_result = $host.ui.PromptForChoice($title, $message, $KeyVaultArray, 0)
    #Update these values with the proper Key Vault names/environments
    switch ($KeyVault_result)
    {
        0 { 
            $KeyVault = "keyvault-dev-udo"
          }
        1 { 
            $KeyVault = "VANPRODEASTVEISKV1"
          }
        2 { 
            $KeyVault = "VAPRODEASTVEISKV1"
          }
        3 {
            $KeyVault = "VAPRODSOUTHVEISKV1"
          }
    }

# Import the contents of the csv file and store it in the $keyvault_list variable.
$keyvault_list = Import-Csv $CSVLocation
# Loop through all the records in the CSV

foreach ($secrets in $keyvault_list) {
	$Secret = ConvertTo-SecureString -String $secrets.value -AsPlainText -Force
	Set-AzKeyVaultSecret -VaultName $KeyVault -Name $secrets.Name -SecretValue $Secret

  Write-Host "Updated Secret" $secrets.Name "to" $secrets.Value -ForegroundColor Green
}
