$psake.use_exit_on_error = $true
properties {
    $baseDir = (Split-Path -parent $psake.build_script_dir)
    #$versionTag = git describe --abbrev=0 --tags
    #$version = $versionTag + "."
    #$version += (git log $($version + '..') --pretty=oneline | measure-object).Count
    $version = "0.1.5"
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
    if (Test-Path "$baseDir\BuildArtifacts") {
      Remove-Item "$baseDir\BuildArtifacts" -Recurse -Force
    }
    if (Test-Path "$baseDir\buildPackages\*.nupkg") {
      Remove-Item "$baseDir\buildPackages\*.nupkg" -Force
    }
    
    mkdir "$baseDir\BuildArtifacts"
    exec { .$nugetExe pack "$baseDir\ChocolateyPackage\ChocolateyGUI\ChocolateyGUI.nuspec" -OutputDirectory "$baseDir\BuildArtifacts" -NoPackageAnalysis -version $version }
}