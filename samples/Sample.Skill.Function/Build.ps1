# Store the current location
$originalLocation = Get-Location

# Get the directory of the script
$scriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent

# Change to the script's directory
Set-Location -Path $scriptDir

# Execute the commands
dotnet restore
dotnet lambda package --configuration release --framework net9.0 --output-package bin/release/net9.0/basic-skill.zip

# Reset to the original location
Set-Location -Path $originalLocation