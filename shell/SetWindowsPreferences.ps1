$key = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer'
$advancedKey = "$key\Advanced"
Set-ItemProperty $advancedKey Hidden 1
Set-ItemProperty $advancedKey HideFileExt 0
Set-ItemProperty $advancedKey ShowSuperHidden 1

$identity = [System.Security.Principal.WindowsIdentity]::GetCurrent()
$parts = $identity.Name -split "\\"
$user = @{Domain=$parts[0];Name=$parts[1]}

try{
  try { $explorer = Get-Process -Name explorer -ErrorAction stop -IncludeUserName }
  catch {$global:error.RemoveAt(0)}

  if($explorer -ne $null) {
    $explorer | ? { $_.UserName -eq "$($user.Domain)\$($user.Name)"} | Stop-Process -Force -ErrorAction Stop | Out-Null
  }

  Start-Sleep 1

  if(!(Get-Process -Name explorer -ErrorAction SilentlyContinue)) {
    $global:error.RemoveAt(0)
    start-Process -FilePath explorer
  }
} catch {$global:error.RemoveAt(0)}
