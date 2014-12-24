$packageName = "ChocolateyGUI";
$fileType = 'msi';
$silentArgs = '/qr /norestart'
$validExitCodes = @(0)

try {
  	$packageGuid = Get-ChildItem HKLM:\SOFTWARE\Classes\Installer\Products |
    	Get-ItemProperty -Name 'ProductName' |
    	? { $_.ProductName -like $packageName + "*"} |
    	Select -ExpandProperty PSChildName -First 1
	
	$properties = Get-ItemProperty HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Products\$packageGuid\InstallProperties
	
	$file = $properties.LocalPackage
	
	Uninstall-ChocolateyPackage $packageName $fileType $silentArgs $file -validExitCodes $validExitCodes
	
	Write-ChocolateySuccess $package
}
catch {
	Write-ChocolateyFailure $package "$($_.Exception.Message)"
	throw
}
