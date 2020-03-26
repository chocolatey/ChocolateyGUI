$ErrorActionPreference = 'Stop';
$toolsDir     = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$fileLocation = Join-Path $toolsDir 'ChocolateyGUI.msi'

$packageArgs = @{
  packageName   = $env:ChocolateyPackageName
  softwareName  = 'Chocolatey GUI'
  file          = $fileLocation
  fileType      = 'msi'
  silentArgs    = "/qn /norestart /l*v `"$env:TEMP\$env:ChocolateyPackageName.$env:ChocolateyPackageVersion.log`""
  validExitCodes= @(0,1641,3010)
}

Install-ChocolateyInstallPackage @packageArgs

Remove-Item -Force $packageArgs.file

$installDirectory = Get-AppInstallLocation $packageArgs.softwareName

if ($installDirectory) {
  Install-BinFile -Name "chocolateygui" -Path "$installDirectory\ChocolateyGui.exe" -UseStart
  Install-BinFile -Name "chocolateyguicli" -Path "$installDirectory\ChocolateyGuiCli.exe"
}

$pp = Get-PackageParameters

function Set-FeatureState {
  [CmdletBinding()]
  param(
    [Parameter(Mandatory=$true)]
    [string] $FeatureName,

    [Parameter(Mandatory=$true)]
    [bool] $EnableFeature
  )

  if ($EnableFeature) {
    Write-Output "Enabling $FeatureName..."
    Start-ChocolateyProcessAsAdmin -Statements "feature enable --name=$FeatureName" -ExeToRun "chocolateyguicli"
  } else {
    Write-Output "Disabling $FeatureName..."
    Start-ChocolateyProcessAsAdmin -Statements "feature disable --name=$FeatureName" -ExeToRun "chocolateyguicli"
  }
}

# Features
if($pp.ContainsKey("ShowConsoleOutput")) {
  Set-FeatureState "ShowConsoleOutput" ($pp.ShowConsoleOutput -eq $true)
}

if($pp.ContainsKey("DefaultToTileViewForLocalSource")) {
  Set-FeatureState "DefaultToTileViewForLocalSource" ($pp.DefaultToTileViewForLocalSource -eq $true)
}

if($pp.ContainsKey("DefaultToTileViewForRemoteSource")) {
  Set-FeatureState "DefaultToTileViewForRemoteSource" ($pp.DefaultToTileViewForRemoteSource -eq $true)
}

if($pp.ContainsKey("UseDelayedSearch")) {
  Set-FeatureState "UseDelayedSearch" ($pp.DefaultToTileViewForRemoteSource -eq $true)
}

if($pp.ContainsKey("ExcludeInstalledPackages")) {
  Set-FeatureState "ExcludeInstalledPackages" ($pp.ExcludeInstalledPackages -eq $true)
}

if($pp.ContainsKey("ShowAggregatedSourceView")) {
  Set-FeatureState "ShowAggregatedSourceView" ($pp.ShowAggregatedSourceView -eq $true)
}

if($pp.ContainsKey("OutdatedPackagesCacheDurationInMinutes")) {
  Start-ChocolateyProcessAsAdmin -Statements "config set --name=OutdatedPackagesCacheDurationInMinutes --value=$($pp.OutdatedPackagesCacheDurationInMinutes)" -ExeToRun "chocolateyguicli"
}