# because sometimes explorer.exe just doesn't get the message that things were updated.
if (-not ("win32.nativemethods" -as [type])) {
  # import sendmessagetimeout from win32
  add-type -Namespace Win32 -Name NativeMethods -MemberDefinition @"
[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
public static extern IntPtr SendMessageTimeout(
  IntPtr hWnd, uint Msg, UIntPtr wParam, string lParam,
  uint fuFlags, uint uTimeout, out UIntPtr lpdwResult);
"@
}

$HWND_BROADCAST = [intptr]0xffff;
$WM_SETTINGCHANGE = 0x1a;
$result = [uintptr]::zero

# notify all windows of environment block change
[win32.nativemethods]::SendMessageTimeout($HWND_BROADCAST, $WM_SETTINGCHANGE, [uintptr]::Zero, "Environment", 2, 5000, [ref]$result);

# Attempting to make the script recapture environment changes
setx.exe trigger 1

#Write-Output "For good measure, we are going to take out explorer"
#tskill.exe explorer /a /v
