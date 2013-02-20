# The creation of this build script (and associated files) was only possible using the 
# work that was done on the BoxStarter Project on GitHub:
# http://boxstarter.codeplex.com/
# Big thanks to Matt Wrock (@mwrockx} for creating this project, thanks!

$psake.use_exit_on_error = $true
properties {
    $baseDir = (Split-Path -parent $psake.build_script_dir)
    $versionTag = git describe --abbrev=0 --tags
    $buildCounter = (git log $($versionTag + '..') --pretty=oneline | measure-object).Count
	
	$version = $versionTag + "." + $buildCounter	
	$preversion = $versionTag + "-pre" + $buildCounter.ToString("0000")
	
    $nugetExe = "$env:ChocolateyInstall\ChocolateyInstall\nuget"
	$assemblyVersionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
	$assemblyVersion = 'AssemblyVersion("' + $version + '")'
	$wixVersionPattern = 'VersionNumber="[0-9]+(\.([0-9]+|\*)){1,3}"'
	$wivVersion = 'VersionNumber="' + $version + '"'
}

Task default -depends Build
Task Build -depends Package -description 'Versions and packages'
Task Package -depends Version-Files, Build-Solution, Pack-Nuget -description 'Versions the CommonAssemblyInfo and WiX files, builds the solution, and packs the msi'

Task Version-Files -description 'Stamps the common file with the version' {
    (Get-Content "$baseDir\SharedSource\Common\CommonAssemblyVersion.cs") | % {$_ -replace $assemblyVersionPattern, $assemblyVersion } | Set-Content "$baseDir\SharedSource\Common\CommonAssemblyVersion.cs"
    (Get-Content "$baseDir\SharedSource\Common\CommonWixInfo.wxi") | % {$_ -replace $wixVersionPattern, $wivVersion } | Set-Content "$baseDir\SharedSource\Common\CommonWixInfo.wxi"    
}

Task Build-Solution -description 'Builds the main solution for the package' {
	exec { msbuild "$baseDir\Chocolatey Explorer\Chocolatey Explorer.sln" }
}


Task Pack-Nuget -description 'Packs the module and example package' {
    if (!(Test-Path "$baseDir\BuildArtifacts")) {
		mkdir "$baseDir\BuildArtifacts"
	}
	
	if (Test-Path "$baseDir\BuildArtifacts\*.nupkg") {
      Remove-Item "$baseDir\BuildArtifacts\*.nupkg" -Force
    }
    
    exec { .$nugetExe pack "$baseDir\ChocolateyPackage\ChocolateyGUI.nuspec" -OutputDirectory "$baseDir\BuildArtifacts" -NoPackageAnalysis -version $preversion }
}