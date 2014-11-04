# The creation of this build script (and associated files) was only possible using the 
# work that was done on the BoxStarter Project on GitHub:
# http://boxstarter.codeplex.com/
# Big thanks to Matt Wrock (@mwrockx} for creating this project, thanks!

param (
	[string]$Action="default",
	[string]$Config="Debug",
	[switch]$Help
)

$here = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$psakePath = Join-Path $here -Child "..\Tools\psake\psake.psm1";
Import-Module $psakePath;

if($Help){ 
	try {
    Get-Help "$($MyInvocation.MyCommand.Definition)" -full | Out-Host -paging
    Write-Host "Available build tasks:"
    psake "$here/default.ps1" -nologo -docs | Out-Host -paging
	} catch {}

	return
}

if(Test-Path -Path env:\APPVEYOR) {
		if($env:APPVEYOR_REPO_BRANCH -eq "develop" -And $env:APPVEYOR_PULL_REQUEST_NUMBER -eq "") {
      Write-Host "Since we are on develop branch with no pull request number, we are ready to deploy to MyGet"
      invoke-psake "$here/default.ps1" -task DeploySolutionToMyGet -properties @{ 'config'='Release'; }
		} elseif($env:APPVEYOR_REPO_BRANCH -eq "develop" -And $env:APPVEYOR_PULL_REQUEST_NUMBER -ne "") {
      Write-Host "Since we are on develop branch with a pull request number, we are just going to package the solution, with no deployment"
      invoke-psake "$here/default.ps1" -task InspectCodeForProblems -properties @{ 'config'='Release'; }
		} elseif($env:APPVEYOR_REPO_BRANCH -eq "master" -And $env:APPVEYOR_PULL_REQUEST_NUMBER -ne "") {
      Write-Host "Since we are on develop branch with no pull request number, we are ready to deploy to Chocolatey"
      invoke-psake "$here/default.ps1" -task DeploySolutionToChocolatey -properties @{ 'config'='Release'; }
		} elseif($env:APPVEYOR_REPO_BRANCH -eq "master" -And $env:APPVEYOR_PULL_REQUEST_NUMBER -ne "") {
      Write-Host "Since we are on develop branch with a pull request number, we are just going to package the solution, with no deployment"
      invoke-psake "$here/default.ps1" -task InspectCodeForProblems -properties @{ 'config'='Release'; }
		}
} else {
		invoke-psake "$here/default.ps1" -task $Action -properties @{ 'config'=$Config; }
}

if ($psake.build_success -eq $false) { 
	exit 1 
} else { 
	exit 0 
}