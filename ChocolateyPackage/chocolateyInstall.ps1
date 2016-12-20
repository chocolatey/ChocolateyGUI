$scriptPath =  $(Split-Path $MyInvocation.MyCommand.Path)

$packageArgs = @{
  packageName = 'ChocolateyGUI'
  fileType = 'msi'
  silentArgs = '/quiet'
  file = Join-Path $scriptPath 'ChocolateyGUI.msi'
}

Install-ChocolateyInstallPackage @packageArgs