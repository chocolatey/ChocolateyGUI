$localVersion='0.10.11'
$installLatestBeta = $false
# OR install a version directly
#$env:chocolateyVersion="0.9.10-beta-20160402"
$installLocalFile = $false
$localChocolateyPackageFilePath = "c:\packages\chocolatey.$localVersion.nupkg"
$ChocoInstallPath = "$($env:SystemDrive)\ProgramData\Chocolatey\bin"
$env:ChocolateyInstall = "$($env:SystemDrive)\ProgramData\Chocolatey"
$env:Path += ";$ChocoInstallPath"
$DebugPreference = "Continue";
$env:ChocolateyEnvironmentDebug = 'false'

# Ensure we can run everything
Set-ExecutionPolicy Bypass -Scope Process -Force;

# Reroute TEMP to a local location
New-Item $env:ALLUSERSPROFILE\choco-cache -ItemType Directory -Force
$env:TEMP = "$env:ALLUSERSPROFILE\choco-cache"
# Ignore proxies since we are using internal locations
$env:chocolateyIgnoreProxy = 'true'
# Set proxy settings if necessary
#$env:chocolateyProxyLocation = 'https://local/proxy/server'
#$env:chocolateyProxyUser = 'username'
#$env:chocolateyProxyPassword = 'password'

function Install-LocalChocolateyPackage {
param (
  [string]$chocolateyPackageFilePath = ''
)

  if ($chocolateyPackageFilePath -eq $null -or $chocolateyPackageFilePath -eq '') {
    throw "You must specify a local package to run the local install."
  }

  if (!(Test-Path($chocolateyPackageFilePath))) {
    throw "No file exists at $chocolateyPackageFilePath"
  }

  if ($env:TEMP -eq $null) {
    $env:TEMP = Join-Path $env:SystemDrive 'temp'
  }
  $chocTempDir = Join-Path $env:TEMP "chocolatey"
  $tempDir = Join-Path $chocTempDir "chocInstall"
  if (![System.IO.Directory]::Exists($tempDir)) {[System.IO.Directory]::CreateDirectory($tempDir)}
  $file = Join-Path $tempDir "chocolatey.zip"
  Copy-Item $chocolateyPackageFilePath $file -Force

  # unzip the package
  Write-Output "Extracting $file to $tempDir..."
  $shellApplication = new-object -com shell.application
  $zipPackage = $shellApplication.NameSpace($file)
  $destinationFolder = $shellApplication.NameSpace($tempDir)
  $destinationFolder.CopyHere($zipPackage.Items(),0x10)

  # Call chocolatey install
  Write-Output "Installing chocolatey on this machine"
  $toolsFolder = Join-Path $tempDir "tools"
  $chocInstallPS1 = Join-Path $toolsFolder "chocolateyInstall.ps1"

  & $chocInstallPS1

  Write-Output 'Ensuring chocolatey commands are on the path'
  $chocInstallVariableName = "ChocolateyInstall"
  $chocoPath = [Environment]::GetEnvironmentVariable($chocInstallVariableName)
  if ($chocoPath -eq $null -or $chocoPath -eq '') {
    $chocoPath = 'C:\ProgramData\Chocolatey'
  }

  $chocoExePath = Join-Path $chocoPath 'bin'

  if ($($env:Path).ToLower().Contains($($chocoExePath).ToLower()) -eq $false) {
    $env:Path = [Environment]::GetEnvironmentVariable('Path',[System.EnvironmentVariableTarget]::Machine);
  }
}

if (!(Test-Path $ChocoInstallPath)) {
  # Install Chocolatey
  if ($installLocalFile) {
    Install-LocalChocolateyPackage $localChocolateyPackageFilePath
  } else {
    if ($installLatestBeta) {
      iex ((new-object net.webclient).DownloadString('https://chocolatey.org/installabsolutelatest.ps1'))
    } else {
      iex ((new-object net.webclient).DownloadString('https://chocolatey.org/install.ps1'))
    }
  }
}

#Update-SessionEnvironment
choco feature enable --name="'autouninstaller'"
# - not recommended for production systems:
choco feature enable --name="'allowGlobalConfirmation'"
# - not recommended for production systems:
choco feature enable --name="'logEnvironmentValues'"

# Set Configuration
choco config set cacheLocation $env:ALLUSERSPROFILE\choco-cache
choco config set commandExecutionTimeoutSeconds 14400

# Sources - Add internal repositories
choco source add --name="'local'" --source="'c:\packages'" --priority="'2'" --bypass-proxy --allow-self-service
