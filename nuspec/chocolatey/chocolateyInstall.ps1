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
}

$pp = Get-PackageParameters

# Features
if($pp.ContainsKey("ShowConsoleOutput")) {
  if($pp.ShowConsoleOutput -eq $true) {
    Write-Output "Enabling ShowConsoleOutput..."
    Start-ChocolateyProcessAsAdmin -Statements "feature enable --name=ShowConsoleOutput" -ExeToRun "chocolateygui"
  } else {
    Write-Output "Disabling ShowConsoleOutput..."
    Start-ChocolateyProcessAsAdmin -Statements "feature disable --name=ShowConsoleOutput" -ExeToRun "chocolateygui"

  }
}

if($pp.ContainsKey("DefaultToTileViewForLocalSource")) {
  if($pp.DefaultToTileViewForLocalSource -eq $true) {
    Write-Output "Enabling DefaultToTileViewForLocalSource..."
    Start-ChocolateyProcessAsAdmin -Statements "feature enable --name=DefaultToTileViewForLocalSource" -ExeToRun "chocolateygui"
  } else {
    Write-Output "Disabling DefaultToTileViewForLocalSource..."
    Start-ChocolateyProcessAsAdmin -Statements "feature disable --name=DefaultToTileViewForLocalSource" -ExeToRun "chocolateygui"
  }
}

if($pp.ContainsKey("DefaultToTileViewForRemoteSource")) {
  if($pp.DefaultToTileViewForRemoteSource -eq $true) {
    Write-Output "Enabling DefaultToTileViewForRemoteSource..."
    Start-ChocolateyProcessAsAdmin -Statements "feature enable --name=DefaultToTileViewForRemoteSource" -ExeToRun "chocolateygui"
  } else {
    Write-Output "Disabling DefaultToTileViewForRemoteSource..."
    Start-ChocolateyProcessAsAdmin -Statements "feature disable --name=DefaultToTileViewForRemoteSource" -ExeToRun "chocolateygui"
  }
}

if($pp.ContainsKey("UseDelayedSearch")) {
  if($pp.UseDelayedSearch -eq $true) {
    Write-Output "Enabling UseDelayedSearch..."
    Start-ChocolateyProcessAsAdmin -Statements "feature enable --name=UseDelayedSearch" -ExeToRun "chocolateygui"
  } else {
    Write-Output "Disabling UseDelayedSearch..."
    Start-ChocolateyProcessAsAdmin -Statements "feature disable --name=UseDelayedSearch" -ExeToRun "chocolateygui"
  }
}

if($pp.ContainsKey("ExcludeInstalledPackages")) {
  if($pp.ExcludeInstalledPackages -eq $true) {
    Write-Output "Enabling ExcludeInstalledPackages..."
    Start-ChocolateyProcessAsAdmin -Statements "feature enable --name=ExcludeInstalledPackages" -ExeToRun "chocolateygui"
  } else {
    Write-Output "Disabling ExcludeInstalledPackages..."
    Start-ChocolateyProcessAsAdmin -Statements "feature disable --name=ExcludeInstalledPackages" -ExeToRun "chocolateygui"
  }
}

if($pp.ContainsKey("ShowAggregatedSourceView")) {
  if($pp.ShowAggregatedSourceView -eq $true) {
    Write-Output "Enabling ShowAggregatedSourceView..."
    Start-ChocolateyProcessAsAdmin -Statements "feature enable --name=ShowAggregatedSourceView" -ExeToRun "chocolateygui"
  } else {
    Write-Output "Disabling ShowAggregatedSourceView..."
    Start-ChocolateyProcessAsAdmin -Statements "feature disable --name=ShowAggregatedSourceView" -ExeToRun "chocolateygui"
  }
}

if($pp.ContainsKey("OutputPackagesCacheDurationInMinutes")) {
  Start-ChocolateyProcessAsAdmin -Statements "config set --name=OutputPackagesCacheDurationInMinutes --value=$($pp.OutputPackagesCacheDurationInMinutes)" -ExeToRun "chocolateygui"
}