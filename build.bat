@ECHO off

REM The creation of this build script (and associated files) was only possible using the 
REM work that was done on the BoxStarter Project on GitHub:
REM http://boxstarter.codeplex.com/
REM Big thanks to Matt Wrock (@mwrockx} for creating this project, thanks!

IF '%1'=='/?' GOTO help
IF '%1'=='-help' GOTO help
IF '%1'=='-h' GOTO help

ECHO Inspect Environment Variables
IF "%APPVEYOR%"=="" (
	ECHO APPVEYOR: Not Defined
	
	powershell -NoProfile -ExecutionPolicy bypass -Command "%~dp0BuildScripts\build.ps1 %*"
	GOTO :eof
) ELSE (
	ECHO APPVEYOR: %APPVEYOR%
	IF "%CI%"=="" (ECHO CI: Not Defined) ELSE (ECHO CI: %CI%)
	IF "%APPVEYOR_API_URL%"=="" (ECHO APPVEYOR_API_URL: Not Defined) ELSE (ECHO APPVEYOR_API_URL: %APPVEYOR_API_URL%)
	IF "%APPVEYOR_PROJECT_ID%"=="" (ECHO APPVEYOR_PROJECT_ID: Not Defined) ELSE (ECHO APPVEYOR_PROJECT_ID: %APPVEYOR_PROJECT_ID%)
	IF "%APPVEYOR_PROJECT_NAME%"=="" (ECHO APPVEYOR_PROJECT_NAME: Not Defined) ELSE (ECHO APPVEYOR_PROJECT_NAME: %APPVEYOR_PROJECT_NAME%)
	IF "%APPVEYOR_PROJECT_SLUG%"=="" (ECHO APPVEYOR_PROJECT_SLUG: Not Defined) ELSE (ECHO APPVEYOR_PROJECT_SLUG: %APPVEYOR_PROJECT_SLUG%)
	IF "%APPVEYOR_BUILD_FOLDER%"=="" (ECHO APPVEYOR_BUILD_FOLDER: Not Defined) ELSE (ECHO APPVEYOR_BUILD_FOLDER: %APPVEYOR_BUILD_FOLDER%)
	IF "%APPVEYOR_BUILD_ID%"=="" (ECHO APPVEYOR_BUILD_ID: Not Defined) ELSE (ECHO APPVEYOR_BUILD_ID: %APPVEYOR_BUILD_ID%)
	IF "%APPVEYOR_BUILD_NUMBER%"=="" (ECHO APPVEYOR_BUILD_NUMBER: Not Defined) ELSE (ECHO APPVEYOR_BUILD_NUMBER: %APPVEYOR_BUILD_NUMBER%)
	IF "%APPVEYOR_BUILD_VERSION%"=="" (ECHO APPVEYOR_BUILD_VERSION: Not Defined) ELSE (ECHO APPVEYOR_BUILD_VERSION: %APPVEYOR_BUILD_VERSION%)
	IF "%APPVEYOR_PULL_REQUEST_NUMBER%"=="" (ECHO APPVEYOR_PULL_REQUEST_NUMBER: Not Defined) ELSE (ECHO APPVEYOR_PULL_REQUEST_NUMBER: %APPVEYOR_PULL_REQUEST_NUMBER%)
	IF "%APPVEYOR_PULL_REQUEST_TITLE%"=="" (ECHO APPVEYOR_PULL_REQUEST_TITLE: Not Defined) ELSE (ECHO APPVEYOR_PULL_REQUEST_TITLE: %APPVEYOR_PULL_REQUEST_TITLE%)
	IF "%APPVEYOR_JOB_ID%"=="" (ECHO APPVEYOR_JOB_ID: Not Defined) ELSE (ECHO APPVEYOR_JOB_ID: %APPVEYOR_JOB_ID%)
	IF "%APPVEYOR_REPO_PROVIDER%"=="" (ECHO APPVEYOR_REPO_PROVIDER: Not Defined) ELSE (ECHO APPVEYOR_REPO_PROVIDER: %APPVEYOR_REPO_PROVIDER%)
	IF "%APPVEYOR_REPO_SCM%"=="" (ECHO APPVEYOR_REPO_SCM: Not Defined) ELSE (ECHO APPVEYOR_REPO_SCM: %APPVEYOR_REPO_SCM%)
	IF "%APPVEYOR_REPO_NAME%"=="" (ECHO APPVEYOR_REPO_NAME: Not Defined) ELSE (ECHO APPVEYOR_REPO_NAME: %APPVEYOR_REPO_NAME%)
	IF "%APPVEYOR_REPO_BRANCH%"=="" (ECHO APPVEYOR_REPO_BRANCH: Not Defined) ELSE (ECHO APPVEYOR_REPO_BRANCH: %APPVEYOR_REPO_BRANCH%)
	IF "%APPVEYOR_REPO_COMMIT%"=="" (ECHO APPVEYOR_REPO_COMMIT: Not Defined) ELSE (ECHO APPVEYOR_REPO_COMMIT: %APPVEYOR_REPO_COMMIT%)
	IF "%APPVEYOR_REPO_COMMIT_AUTHOR%"=="" (ECHO APPVEYOR_REPO_COMMIT_AUTHOR: Not Defined) ELSE (ECHO APPVEYOR_REPO_COMMIT_AUTHOR: %APPVEYOR_REPO_COMMIT_AUTHOR%)
	IF "%APPVEYOR_REPO_COMMIT_TIMESTAMP%"=="" (ECHO APPVEYOR_REPO_COMMIT_TIMESTAMP: Not Defined) ELSE (ECHO APPVEYOR_REPO_COMMIT_TIMESTAMP: %APPVEYOR_REPO_COMMIT_TIMESTAMP%)
	IF "%APPVEYOR_SCHEDULED_BUILD%"=="" (ECHO APPVEYOR_SCHEDULED_BUILD: Not Defined) ELSE (ECHO APPVEYOR_SCHEDULED_BUILD: %APPVEYOR_SCHEDULED_BUILD%)
	IF "%PLATFORM%"=="" (ECHO PLATFORM: Not Defined) ELSE (ECHO PLATFORM: %PLATFORM%)
	IF "%CONFIGURATION%"=="" (ECHO CONFIGURATION: Not Defined) ELSE (ECHO CONFIGURATION: %CONFIGURATION%)
	
	IF "%APPVEYOR_REPO_BRANCH%"=="develop" IF "%APPVEYOR_PULL_REQUEST_NUMBER%"=="" (
		ECHO Since we are on develop branch with no pull request number, we are ready to deploy to MyGet
		powershell -NoProfile -ExecutionPolicy bypass -Command "%~dp0BuildScripts\build.ps1 DeploySolutionToMyGet Release"
		GOTO :eof
	)

	IF "%APPVEYOR_REPO_BRANCH%"=="develop" IF "%APPVEYOR_PULL_REQUEST_NUMBER%" NEQ "" (
		ECHO Since we are on develop branch with a pull request number, we are just going to package the solution, with no deployment
		powershell -NoProfile -ExecutionPolicy bypass -Command "%~dp0BuildScripts\build.ps1 InspectCodeForProblems Release"
		GOTO :eof
	)

	IF "%APPVEYOR_REPO_BRANCH%"=="master" IF "%APPVEYOR_PULL_REQUEST_NUMBER%"=="" (
		ECHO Since we are on develop branch with no pull request number, we are ready to deploy to Chocolatey
		powershell -NoProfile -ExecutionPolicy bypass -Command "%~dp0BuildScripts\build.ps1 DeploySolutionToChocolatey Release"
		GOTO :eof
	)

	IF "%APPVEYOR_REPO_BRANCH%"=="master" IF "%APPVEYOR_PULL_REQUEST_NUMBER%" NEQ "" (
		ECHO Since we are on develop branch with a pull request number, we are just going to package the solution, with no deployment
		powershell -NoProfile -ExecutionPolicy bypass -Command "%~dp0BuildScripts\build.ps1 InspectCodeForProblems Release"
		GOTO :eof
	)
)

:help
powershell -NoProfile -ExecutionPolicy Bypass -Command "& '%~dp0BuildScripts\build.ps1' -help"