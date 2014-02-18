$package = "ChocolateyGUI"

try {
  	$packageGuid = Get-ChildItem HKLM:\SOFTWARE\Classes\Installer\Products |
    	Get-ItemProperty -Name 'ProductName' |
    	? { $_.ProductName -like $package + "*"} |
    	Select -ExpandProperty PSChildName -First 1
	
	$properties = Get-ItemProperty HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Products\$packageGuid\InstallProperties
	
	$pkg = $properties.LocalPackage
	
	msiexec.exe /x $pkg /qr /norestart
	
	Write-ChocolateySuccess $package
}
catch {
	Write-ChocolateyFailure $package "$($_.Exception.Message)"
	throw
}