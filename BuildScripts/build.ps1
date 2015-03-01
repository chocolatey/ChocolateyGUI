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
    invoke-psake "$here/default.ps1" -nologo -docs | Out-Host -paging
	} catch {}

	return
}

if(Test-Path -Path env:\APPVEYOR) {
		if($env:APPVEYOR_REPO_BRANCH -eq "develop" -And $env:APPVEYOR_PULL_REQUEST_NUMBER -eq $null) {
      Write-Host "Since we are on develop branch with no pull request number, we are ready to deploy to Develop MyGet Feed"
      invoke-psake "$here/default.ps1" -task DeployDevelopSolutionToMyGet -properties @{ 'config'='Release'; }
		} elseif($env:APPVEYOR_REPO_BRANCH -eq "develop" -And $env:APPVEYOR_PULL_REQUEST_NUMBER -ne $null) {
      Write-Host "Since we are on develop branch with a pull request number, we are just going to package the solution, with no deployment"
      invoke-psake "$here/default.ps1" -task InspectCodeForProblems -properties @{ 'config'='Release'; }
		} elseif($env:APPVEYOR_REPO_BRANCH -eq "master" -And $env:APPVEYOR_PULL_REQUEST_NUMBER -eq $null -And $env:APPVEYOR_REPO_TAG -eq $false) {
      Write-Host "Since we are on master branch with no pull request number, and no tag applied, we are ready to deploy to Master MyGet Feed"
      invoke-psake "$here/default.ps1" -task DeployMasterSolutionToMyGet -properties @{ 'config'='Release'; }
		} elseif($env:APPVEYOR_REPO_BRANCH -eq "master" -And $env:APPVEYOR_PULL_REQUEST_NUMBER -ne $null -And $env:APPVEYOR_REPO_TAG -eq $false) {
      Write-Host "Since we are on master branch with a pull request number, and no tag applied, we are just going to package the solution, with no deployment"
      invoke-psake "$here/default.ps1" -task InspectCodeForProblems -properties @{ 'config'='Release'; }
		} elseif($env:APPVEYOR_REPO_BRANCH -eq "master" -And $env:APPVEYOR_PULL_REQUEST_NUMBER -eq $null -And $env:APPVEYOR_REPO_TAG -eq $true) {
      Write-Host "Since we are on master branch with no pull request number, and a tag has been applied, we are ready to deploy Chocolatey.org"
      invoke-psake "$here/default.ps1" -task DeploySolutionToChocolatey -properties @{ 'config'='Release'; }
		}
} else {
		invoke-psake "$here/default.ps1" -task $Action -properties @{ 'config'=$Config; }
}

# If for some reason, the above if statement doesn't actually invoke-psake, the $psake.build_success will be false
# so it will exit with a code of 1 below, which will fail the build.
if ($psake.build_success -eq $false) { 
	exit 1 
} else { 
	exit 0 
}