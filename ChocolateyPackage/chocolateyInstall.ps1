$packageName = 'ChocolateyGUI'
$fileType = 'msi'
$silentArgs = '/quiet'
$filePath =  $(Split-Path $MyInvocation.MyCommand.Path)
$fileFullPath = Join-Path $scriptPath 'ChocolateyGUI.msi'

try { 
  Install-ChocolateyInstallPackage $packageName $fileType $silentArgs $fileFullPath
} catch {
  Write-ChocolateyFailure $packageName $($_.Exception.Message)
  throw 
}
