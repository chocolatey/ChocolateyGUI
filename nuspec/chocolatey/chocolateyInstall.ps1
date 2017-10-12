$scriptPath =  $(Split-Path $MyInvocation.MyCommand.Path)

$packageArgs = @{
	packageName = 'Chocolatey GUI'
	softwareName   = 'Chocolatey GUI'
	fileType = 'msi'
	silentArgs = '/quiet'
	file = Join-Path $scriptPath 'ChocolateyGUI.msi'
}

Install-ChocolateyInstallPackage @packageArgs

Remove-Item -Force $packageArgs.file