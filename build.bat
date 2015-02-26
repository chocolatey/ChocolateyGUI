@ECHO off

REM The creation of this build script (and associated files) was only possible using the 
REM work that was done on the BoxStarter Project on GitHub:
REM http://boxstarter.codeplex.com/
REM Big thanks to Matt Wrock (@mwrockx} for creating this project, thanks!

IF '%1'=='/?' GOTO help
IF '%1'=='-help' GOTO help
IF '%1'=='-h' GOTO help

powershell -NoProfile -ExecutionPolicy bypass -Command "%~dp0BuildScripts\build.ps1 %*"
GOTO :eof

:help
powershell -NoProfile -ExecutionPolicy Bypass -Command "& '%~dp0BuildScripts\build.ps1' -help"