@echo off

REM The creation of this build script (and associated files) was only possible using the 
REM work that was done on the BoxStarter Project on GitHub:
REM http://boxstarter.codeplex.com/
REM Big thanks to Matt Wrock (@mwrockx} for creating this project, thanks!

if '%1'=='/?' goto help
if '%1'=='-help' goto help
if '%1'=='-h' goto help

powershell -NoProfile -ExecutionPolicy bypass -Command "%~dp0BuildScripts\build.ps1 %*"
goto :eof

:help
powershell -NoProfile -ExecutionPolicy Bypass -Command "& '%~dp0BuildScripts\build.ps1' -help"