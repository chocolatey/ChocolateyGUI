$packageName = 'ChocolateyGUI'
$fileType = 'msi'
$silentArgs = '/quiet'
$scriptPath =  $(Split-Path $MyInvocation.MyCommand.Path)
$fileFullPath = Join-Path $scriptPath 'ChocolateyGUI.msi'

Install-ChocolateyInstallPackage $packageName $fileType $silentArgs $fileFullPath