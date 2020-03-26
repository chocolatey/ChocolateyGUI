@echo off
setlocal enableextensions enabledelayedexpansion
set psscript="./build.ps1"
echo ==================================================
echo ============= WRAP POWERSHELL SCRIPT =============
echo ==================================================

echo calling %psscript% with args %*
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& '%psscript%' -Configuration ReleaseOfficial --prepareLocalRelease=true%*"

echo ==================================================
endlocal
