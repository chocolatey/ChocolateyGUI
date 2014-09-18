# The creation of this build script (and associated files) was only possible using the 
# work that was done on the BoxStarter Project on GitHub:
# http://boxstarter.codeplex.com/
# Big thanks to Matt Wrock (@mwrockx} for creating this project, thanks!

param (
    [string]$Action="default",
	[string]$Config="release",
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

invoke-psake "$here/default.ps1" -task $Action -properties @{ 'config'=$Config; 'preversion'=$PackageVersion}
  
if ($psake.build_success -eq $false) { 
  exit 1 
} else { 
  exit 0 
}