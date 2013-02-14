try { 
  $scriptPath = $(Split-Path $MyInvocation.MyCommand.Path)
  $nodePath = Join-Path $scriptPath 'Setup_ChocolateyGUI.msi'
  Install-ChocolateyInstallPackage 'ChocolateyGUI' 'msi' '/quiet' $nodepath
  Write-ChocolateySuccess 'ChocolateyGUI'
} catch {
  Write-ChocolateyFailure 'ChocolateyGUI' "$($_.Exception.Message)"
  throw 
}