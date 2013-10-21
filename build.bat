@echo off

REM The creation of this build script (and associated files) was only possible using the 
REM work that was done on the BoxStarter Project on GitHub:
REM http://boxstarter.codeplex.com/
REM Big thanks to Matt Wrock (@mwrockx} for creating this project, thanks!

if '%1'=='/?' goto help
if '%1'=='-help' goto help
if '%1'=='-h' goto help

set config=%2
set version=%3

if [%1]==[] (
	set action='PackageSolution'
) else (
	set action=%1
)

if [%2]==[] (
	set config='Release'
) else (
	set config=%2
)

if not '%PackageVersion%'=='' (
   set version=%PackageVersion%
)

if '%version%'=='' (
	set version='12.13.14-pre15'
)

powershell -NoProfile -ExecutionPolicy bypass -Command "%~dp0/BuildScripts/trigger-build.ps1 -Action %action% -Config %config% -PackageVersion %version%"
goto :eof

:help
powershell -NoProfile -ExecutionPolicy Bypass -Command "& '%~dp0buildscripts\trigger-build.ps1' -help"