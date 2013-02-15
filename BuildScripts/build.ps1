# The creation of this build script (and associated files) was only possible using the 
# work that was done on the BoxStarter Project on GitHub:
# http://boxstarter.codeplex.com/
# Big thanks to Matt Wrock (@mwrockx} for creating this project, thanks!

param (
    [string]$Action="default",
    [switch]$Help
)
$here = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

if($Help){ 
  try {
    Get-Help "$($MyInvocation.MyCommand.Definition)" -full | Out-Host -paging
    Write-Host "Available build tasks:"
    psake -nologo -docs | Out-Host -paging
  } catch {}
  return
}

psake "$here/default.ps1" $Action