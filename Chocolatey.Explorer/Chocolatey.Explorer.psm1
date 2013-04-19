$solutionPath = Get-Item dte:/solution |
					select -Expand fullname |
					Split-Path -Parent;

$psakeImportFile = Join-Path $solutionPath -Child "..\SharedBinaries\psake\psake.psm1";

Import-Module $psakeImportFile;

New-Item -Path dte:/commandbars/menubar/build/psake-build -Value {
    Invoke-ChocolateyGuiBuild -BuildTask "Build-Solution";
}

New-Item -Path "dte:/commandbars/Context Menus/Project and Solution Context Menus/Solution/psake-build" -Value {
	Invoke-ChocolateyGuiBuild -BuildTask "Build-Solution";
}

New-Item -Path dte:/commandbars/menubar/build/psake-clean -Value {
    Invoke-ChocolateyGuiBuild -BuildTask "Clean-Solution";
}

New-Item -Path "dte:/commandbars/Context Menus/Project and Solution Context Menus/Solution/psake-clean" -Value {
	Invoke-ChocolateyGuiBuild -BuildTask "Clean-Solution";
}

New-Item -Path dte:/commandbars/menubar/build/psake-rebuild -Value {
    Invoke-ChocolateyGuiBuild -BuildTask "Rebuild-Solution";
}

New-Item -Path "dte:/commandbars/Context Menus/Project and Solution Context Menus/Solution/psake-rebuild" -Value {
	Invoke-ChocolateyGuiBuild -BuildTask "Rebuild-Solution";
}

New-Item -Path dte:/commandbars/menubar/build/psake-package -Value {
	Invoke-ChocolateyGuiBuild -BuildTask "Package-Solution";
}

New-Item -Path "dte:/commandbars/Context Menus/Project and Solution Context Menus/Solution/psake-package" -Value {
	Invoke-ChocolateyGuiBuild -BuildTask "Package-Solution";
}

function Invoke-ChocolateyGuiBuild {
param(
	[string] $buildTask = "Package-Solution"
)
	$solution = Get-Item dte:\solution;
	
    $solutionPath = $solution | 
                        select -expand fullname | 
                        split-path -parent;
                        
	$activeConfiguration = $solution.solutionBuild.ActiveConfiguration.Name;
	
    $psakeBuildFile = Join-Path $solutionPath -Child "..\BuildScripts\default.ps1"
	
	Write-Host;
	Invoke-psake $psakeBuildFile -Task $buildTask -properties @{ 'config'=$activeConfiguration } | Out-Host;
	Write-Prompt;
}

function unregister-Chocolatey.Explorer {
	Remove-Module psake;
	Remove-Item dte:/commandbars/menubar/build/psake-build
	Remove-Item "dte:/commandbars/Context Menus/Project and Solution Context Menus/Solution/psake-build"
	Remove-Item dte:/commandbars/menubar/build/psake-clean
	Remove-Item "dte:/commandbars/Context Menus/Project and Solution Context Menus/Solution/psake-clean"
	Remove-Item dte:/commandbars/menubar/build/psake-rebuild
	Remove-Item "dte:/commandbars/Context Menus/Project and Solution Context Menus/Solution/psake-rebuild"
	Remove-Item dte:/commandbars/menubar/build/psake-package
	Remove-Item "dte:/commandbars/Context Menus/Project and Solution Context Menus/Solution/psake-package"
}