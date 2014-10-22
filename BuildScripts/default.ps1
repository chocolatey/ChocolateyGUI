# The creation of this build script (and associated files) was only possible using the 
# work that was done on the BoxStarter Project on GitHub:
# http://boxstarter.codeplex.com/
# Big thanks to Matt Wrock (@mwrockx} for creating this project, thanks!

$psake.use_exit_on_error = $true
properties {
  $config = 'Debug';
  $nugetExe = "..\Tools\NuGet\NuGet.exe";
  $projectName = "ChocolateyGUI";
}

$private = "This is a private task not meant for external use!";

function get-buildArtifactsDirectory {
  return "." | Resolve-Path | Join-Path -ChildPath "../BuildArtifacts";
}

function get-buildScriptsDirectory {
  return "." | Resolve-Path;
}

function get-sourceDirectory {
  return "." | Resolve-Path | Join-Path -ChildPath "../Source";
}

function get-rootDirectory {
  return "." | Resolve-Path | Join-Path -ChildPath "../";
}

function create-PackageDirectory( [Parameter(ValueFromPipeline=$true)]$packageDirectory ) {
  process {
    Write-Verbose "checking for package path $packageDirectory...";
    if( !(Test-Path $packageDirectory ) ) {
      Write-Verbose "creating package directory at $packageDirectory...";
      mkdir $packageDirectory | Out-Null;
    }
  }    
}

function remove-PackageDirectory( [Parameter(ValueFromPipeline=$true)]$packageDirectory ) {
  process {
    Write-Verbose "Checking directory at $packageDirectory...";
    if(Test-Path $packageDirectory) {
      Write-Verbose "Removing directory at $packageDirectory...";
      Remove-Item $packageDirectory -recurse -force;
    }
  }
}

function isAppVeyor() {
  Test-Path -Path env:\APPVEYOR
}

function isChocolateyInstalled() {
  $script:chocolateyDir = $null
  if ($env:ChocolateyInstall -ne $null) {
    $script:chocolateyDir = $env:ChocolateyInstall;
  } elseif (Test-Path (Join-Path $env:SYSTEMDRIVE Chocolatey)) {
    $script:chocolateyDir = Join-Path $env:SYSTEMDRIVE Chocolatey;
  } elseif (Test-Path (Join-Path ([Environment]::GetFolderPath("CommonApplicationData")) Chocolatey)) {
    $script:chocolateyDir = Join-Path ([Environment]::GetFolderPath("CommonApplicationData")) Chocolatey;
  }

  Test-Path -Path $script:chocolateyDir;
}

function analyseStyleCopResults( [Parameter(ValueFromPipeline=$true)]$styleCopResultsFile ) {
  $styleCopViolations = [xml](Get-Content $styleCopResultsFile);
    
  foreach ($styleCopViolation in $styleCopViolations.StyleCopViolations.Violation) {        
    Write-Host "Violation of Rule $($styleCopViolation.RuleId): $($styleCopViolation.Rule) Line Number: $($styleCopViolation.LineNumber) FileName: $($styleCopViolation.Source) ErrorMessage: $($styleCopViolation.InnerXml)";       

    if(isAppVeyor) {
      Add-AppveyorTest "Violation of Rule $($styleCopViolation.RuleId): $($styleCopViolation.Rule) Line Number: $($styleCopViolation.LineNumber)" -Outcome Failed -FileName $styleCopViolation.Source -ErrorMessage $styleCopViolation.InnerXml;
    }
  }

  if(isAppVeyor) {
    Push-AppveyorArtifact $styleCopResultsFile;
  }
}

function analyseCodeAnalysisResults( [Parameter(ValueFromPipeline=$true)]$codeAnalysisResultsFile ) {
  $codeAnalysisErrors = [xml](Get-Content $codeAnalysisResultsFile);

  foreach ($codeAnalysisError in $codeAnalysisErrors.SelectNodes("//Message")) {
    $issueNode = $codeAnalysisError.SelectSingleNode("Issue");
    Write-Host "Violation of Rule $($codeAnalysisError.CheckId): $($codeAnalysisError.TypeName) Line Number: $($issueNode.Line) FileName: $($issueNode.Path)\$($codeAnalysisError.Issue.File) ErrorMessage: $($issueNode.InnerXml)";

    if(isAppVeyor) {
      Add-AppveyorTest "Violation of Rule $($codeAnalysisError.CheckId): $($codeAnalysisError.TypeName) Line Number: $($issueNode.Line)" -Outcome Failed -FileName "$($issueNode.Path)\$($codeAnalysisError.Issue.File)" -ErrorMessage $($issueNode.InnerXml);
    }
  }

  if(isAppVeyor) {
    Push-AppveyorArtifact $codeAnalysisResultsFile;
  }
}

function analyseDupFinderResults( [Parameter(ValueFromPipeline=$true)]$dupFinderResultsFile ) {
  if(isAppVeyor) {
	  Push-AppveyorArtifact $dupFinderResultsFile;
  }
}

function analyseInspectCodeResults( [Parameter(ValueFromPipeline=$true)]$inspectCodeResultsFile ) {
  if(isAppVeyor) {
	  Push-AppveyorArtifact $inspectCodeResultsFile;
  }
}

function testEnvironmentVariable($envVariableName, $envVariableValue) {
  if($envVariableValue -ne "") {
    Write-Host "$envVariableName : $envVariableValue";
  } else {
    Write-Host "$envVariableName : Not Defined";
  }
}

function applyXslTransform($xmlFile, $xslFile, $outputFile) {
  Write-Host "XML File: $xmlFile"
  Write-Host "XSL File: $xslFile"
  Write-Host "Output File: $outputFile"

  $xslt = New-Object System.Xml.Xsl.XslCompiledTransform;
  $xslt.Load($xslFile);
  $xslt.Transform($xmlFile, $outputFile);
  
  Write-Host "XSL Transform completed."
  
  if(isAppVeyor) {
      Push-AppveyorArtifact $outputFile;
  }
}

Task -Name Default -Depends BuildSolution

# private tasks

Task -Name __EchoAppVeyorEnvironmentVariables -Description $private -Action {
  if(isAppVeyor) {
    testEnvironmentVariable "CI" $env:CI;
    testEnvironmentVariable "APPVEYOR_API_URL" $env:APPVEYOR_API_URL;
    testEnvironmentVariable "APPVEYOR_PROJECT_ID" $env:APPVEYOR_PROJECT_ID;
    testEnvironmentVariable "APPVEYOR_PROJECT_NAME" $env:APPVEYOR_PROJECT_NAME;
    testEnvironmentVariable "APPVEYOR_PROJECT_SLUG" $env:APPVEYOR_PROJECT_SLUG;
    testEnvironmentVariable "APPVEYOR_BUILD_FOLDER" $env:APPVEYOR_BUILD_FOLDER;
    testEnvironmentVariable "APPVEYOR_BUILD_ID" $env:APPVEYOR_BUILD_ID;
    testEnvironmentVariable "APPVEYOR_BUILD_NUMBER" $env:APPVEYOR_BUILD_NUMBER;
    testEnvironmentVariable "APPVEYOR_BUILD_VERSION" $env:APPVEYOR_BUILD_VERSION;
    testEnvironmentVariable "APPVEYOR_PULL_REQUEST_NUMBER" $env:APPVEYOR_PULL_REQUEST_NUMBER;
    testEnvironmentVariable "APPVEYOR_PULL_REQUEST_TITLE" $env:APPVEYOR_PULL_REQUEST_TITLE;
    testEnvironmentVariable "APPVEYOR_JOB_ID" $env:APPVEYOR_JOB_ID;
    testEnvironmentVariable "APPVEYOR_REPO_PROVIDER" $env:APPVEYOR_REPO_PROVIDER;
    testEnvironmentVariable "APPVEYOR_REPO_SCM" $env:APPVEYOR_REPO_SCM;
    testEnvironmentVariable "APPVEYOR_REPO_NAME" $env:APPVEYOR_REPO_NAME;
    testEnvironmentVariable "APPVEYOR_REPO_BRANCH" $env:APPVEYOR_REPO_BRANCH;
    testEnvironmentVariable "APPVEYOR_REPO_COMMIT" $env:APPVEYOR_REPO_COMMIT;
    testEnvironmentVariable "APPVEYOR_REPO_COMMIT_AUTHOR" $env:APPVEYOR_REPO_COMMIT_AUTHOR;
    testEnvironmentVariable "APPVEYOR_REPO_COMMIT_TIMESTAMP" $env:APPVEYOR_REPO_COMMIT_TIMESTAMP;
    testEnvironmentVariable "APPVEYOR_SCHEDULED_BUILD" $env:APPVEYOR_SCHEDULED_BUILD;
    testEnvironmentVariable "PLATFORM" $env:PLATFORM;
    testEnvironmentVariable "CONFIGURATION" $env:CONFIGURATION;
  }
}

Task -Name __VerifyConfiguration -Description $private -Action {
  Assert ( @('Debug', 'Release') -contains $config ) "Unknown configuration, $config; expecting 'Debug' or 'Release'";
}

Task -Name __CreateBuildArtifactsDirectory -Description $private -Action {
  get-buildArtifactsDirectory | create-packageDirectory;
}

Task -Name __RemoveBuildArtifactsDirectory -Description $private -Action {
  get-buildArtifactsDirectory | remove-packageDirectory;
}

Task -Name __InstallChocolatey -Description $private -Action {
  if(isChocolateyInstalled) {
    Write-Host "Chocolatey already installed";
  }	else {
    try {
      Write-Host "Chocolatey is not installed, installing Chocolatey...";
                        
      exec {
        Invoke-Expression ((new-object net.webclient).DownloadString('https://chocolatey.org/install.ps1'));
      }
                        
      $script:chocolateyDir = Join-Path ([Environment]::GetFolderPath("CommonApplicationData")) Chocolatey
      if (-not (Test-Path $script:chocolateyDir)) {
        throw "Error installing Chocolatey"
      }

      Write-Host ("************ Install PSBuild Successful ************")
    }
    catch {
      Write-Error $_
      Write-Host ("************ Install PSBuild Failed ************")
    }
  }
}

Task -Name __InstallReSharperCommandLineTools -Depends __InstallChocolatey -Description $private -Action {
  $chocolateyBinDir = Join-Path $script:chocolateyDir -ChildPath "bin";
  $inspectCodeExe = Join-Path $chocolateyBinDir -ChildPath "inspectcode.exe";
  $choco = Join-Path (Join-Path $script:chocolateyDir "chocolateyInstall") -ChildPath "chocolatey.cmd";

  try {
    Write-Output "Running Install Command Line Tools..."

    if (-not (Test-Path $inspectCodeExe)) {
      exec {
        Invoke-Expression "$choco install resharper-clt";
      }
    } else {
      Write-Host "resharper-clt already installed";
    }

    Write-Host ("************ Install Command Line Tools Successful ************")
  }
  catch {
    Write-Error $_
    Write-Host ("************ Install Command Line Tools Failed ************")
  }	
}

Task -Name __UpdateReSharperCommandLineTools -Description $private -Action {
  $choco = Join-Path (Join-Path $script:chocolateyDir "chocolateyInstall") -ChildPath "chocolatey.cmd";

  try {
    Write-Output "Running Update Command Line Tools..."

    exec {
      Invoke-Expression "$choco update resharper-clt";
    }

    Write-Host ("************ Update Command Line Tools Successful ************")
  }
  catch {
    Write-Error $_
    Write-Host ("************ Update Command Line Tools Failed ************")
  }	
}

Task -Name __InstallPSBuild -Description $private -Action {
  try {
    Write-Output "Running Install PSBuild..."

    exec {
      # Need a test here to see if this is actually required
      (new-object Net.WebClient).DownloadString("https://raw.github.com/ligershark/psbuild/master/src/GetPSBuild.ps1") | Invoke-Expression;
    }

    Write-Host ("************ Install PSBuild Successful ************")
  }
  catch {
    Write-Error $_
    Write-Host ("************ Install PSBuild Failed ************")
  }	
}	

Task -Name __InstallGitVersion -Depends __InstallChocolatey -Description $private -Action {
  $chocolateyBinDir = Join-Path $script:chocolateyDir -ChildPath "bin";
  $gitVersionExe = Join-Path $chocolateyBinDir -ChildPath "GitVersion.exe";
  $choco = Join-Path (Join-Path $script:chocolateyDir "chocolateyInstall") -ChildPath "chocolatey.cmd";

  try {
    Write-Output "Running Install GitVersion.Portable..."

    if (-not (Test-Path $gitVersionExe)) {
      exec {
              Invoke-Expression "$choco install GitVersion.Portable";
      }
    } else {
      Write-Host "GitVersion.Portable already installed";
    }

    Write-Host ("************ Install GitVersion.Portable Successful ************")
  }
  catch {
    Write-Error $_
    Write-Host ("************ Install GitVersion.Portable Failed ************")
  }	
}

Task -Name __UpdateGitVersion -Description $private -Action {
  $choco = Join-Path (Join-Path $script:chocolateyDir "chocolateyInstall") -ChildPath "chocolatey.cmd";

  try {
    Write-Output "Running Update GitVersion.Portable..."

    exec {
      Invoke-Expression "$choco update GitVersion.Portable";
    }

    Write-Host ("************ Update GitVersion.Portable Successful ************")
  }
  catch {
    Write-Error $_
    Write-Host ("************ Update GitVersion.Portable Failed ************")
  }	
}

# primary targets

Task -Name PackageSolution -Depends RebuildSolution, PackageChocolatey -Description "Complete build, including creation of Chocolatey Package."

Task -Name InspectCodeForProblems -Depends PackageSolution, RunDupFinder, RunInspectCode -Description "Complete build, including running dupfinder, and inspectcode."

Task -Name DeploySolutionToMyGet -Depends InspectCodeForProblems, DeployPacakgeToMyGet -Description "Complete build, including creation of Chocolatey Package and Deployment to MyGet.org"

Task -Name DeploySolutionToChocolatey -Depends InspectCodeForProblems, DeployPackageToChocolatey -Description "Complete build, including creation of Chocolatey Package and Deployment to Chocolatey.org."

# build tasks

Task -Name RunGitVersion -Depends __InstallGitVersion -Description "Execute the GitVersion Command Line Tool, to figure out what the current semantic version of the repository is" -Action {
  $rootDirectory = get-rootDirectory;
  $chocolateyBinDir = Join-Path $script:chocolateyDir -ChildPath "bin";
  $gitVersionExe = Join-Path $chocolateyBinDir -ChildPath "GitVersion.exe";
        
  try {
    Write-Output "Running RunGitVersion..."

    exec {
      if(isAppVeyor) {
        Write-Host "Running on AppVeyor, so UpdateAssemblyInfo will be called."
        & $gitVersionExe /output buildserver /UpdateAssemblyInfo true
        $script:version = $env:GitVersion_LegacySemVerPadded
      } else {
        $output = & $gitVersionExe
        $joined = $output -join "`n"
        $versionInfo = $joined | ConvertFrom-Json
        $script:version = $versionInfo.LegacySemVerPadded
      }

      Write-Host "Calculated Legacy SemVer Padded Version Number: $script:version"
    }

    Write-Host ("************ RunGitVersion Successful ************")
  }
  catch {
    Write-Error $_
    Write-Host ("************ RunGitVersion Failed ************")
  }	
}

Task -Name RunInspectCode -Depends __InstallReSharperCommandLineTools -Description "Execute the InspectCode Command Line Tool" -Action {
  $buildArtifactsDirectory = get-buildArtifactsDirectory;     
  $buildScriptsDirectory = get-buildScriptsDirectory;
  $chocolateyBinDir = Join-Path $script:chocolateyDir -ChildPath "bin";
  
  $inspectCodeExe = Join-Path $chocolateyBinDir -ChildPath "inspectcode.exe";
  $inspectCodeConfigFile = Join-Path $buildScriptsDirectory -ChildPath "inspectcode.config";
  $inspectCodeXmlFile = Join-Path -Path $buildArtifactsDirectory -ChildPath "inspectcode.xml";
  
  exec {
    Invoke-Expression "$inspectCodeExe /config=$inspectCodeConfigFile";
	
	if(Test-Path $inspectCodeXmlFile) {
	  $inspectCodeXmlFile | analyseInspectCodeResults;
	}
  }
}

Task -Name RunDupFinder -Depends __InstallReSharperCommandLineTools -Description "Execute the DupFinder Command Line Tool" -Action {
  $buildArtifactsDirectory = get-buildArtifactsDirectory;     
  $buildScriptsDirectory = get-buildScriptsDirectory;
  $chocolateyBinDir = Join-Path $script:chocolateyDir -ChildPath "bin";
    
  $dupFinderExe = Join-Path $chocolateyBinDir -ChildPath "dupfinder.exe";
  $dupFinderConfigFile = Join-Path $buildScriptsDirectory -ChildPath "dupfinder.config";
  $dupFinderXmlFile = Join-Path -Path $buildArtifactsDirectory -ChildPath "dupfinder.xml";
  $dupFinderXslFile = Join-Path -Path $buildScriptsDirectory -ChildPath "dupfinder.xsl";
  $dupFinderHtmlFile = $dupFinderXmlFile -replace ".xml", ".html";
  
  exec {
    Invoke-Expression "$dupFinderExe /config=$dupFinderConfigFile";
	
	if(Test-Path $dupFinderXmlFile) {
	  $dupFinderXmlFile | analyseDupFinderResults;
	  applyXslTransform $dupFinderXmlFile $dupFinderXslFile $dupFinderHtmlFile;
	}
  }
}

Task -Name OutputNugetVersion -Description "So that we are clear which version of NuGet is being used, call NuGet" -Action {
  try {
    Write-Output "Running NuGet..."

    exec {
      & $nugetExe;
    }

    Write-Host ("************ NuGet Successful ************")
  }
  catch {
    Write-Error $_
    Write-Host ("************ NuGet Failed ************")
  }
}

Task -Name NugetPackageRestore -Depends OutputNugetVersion -Description "Restores all the required NuGet packages for this solution, before running the build" -Action {
  $sourceDirectory = get-sourceDirectory;
        
  try {
    Write-Output "Running NugetPackageRestore..."

    exec {
      & $nugetExe restore "$sourceDirectory\ChocolateyGui.sln"
    }

    Write-Host ("************ NugetPackageRestore Successful ************")
  }
  catch {
    Write-Error $_
    Write-Host ("************ NugetPackageRestore Failed ************")
  }
}

Task -Name BuildSolution -Depends __RemoveBuildArtifactsDirectory, __VerifyConfiguration, __InstallPSBuild, __EchoAppVeyorEnvironmentVariables, RunGitVersion, NugetPackageRestore -Description "Builds the main solution for the package" -Action {
  $sourceDirectory = get-sourceDirectory;
  $buildArtifactsDirectory = get-buildArtifactsDirectory;
  $buildScriptsDirectory = get-buildScriptsDirectory;
        
  $styleCopXslFile = Join-Path -Path $buildScriptsDirectory -ChildPath "StyleCopReport.xsl";
  $codeAnalysisXslFile = Join-Path -Path $buildScriptsDirectory -ChildPath "CodeAnalysisReport.xsl";

  try {
    Write-Output "Running BuildSolution..."

    exec { 
      Invoke-MSBuild "$sourceDirectory\ChocolateyGui.sln" -NoLogo -Configuration $config -Targets Build -DetailedSummary -VisualStudioVersion 12.0 -Properties (@{'Platform'='Mixed Platforms'})
            
      $styleCopResultsFiles = Get-ChildItem $buildArtifactsDirectory -Filter "StyleCop*.xml"
      foreach ($styleCopResultsFile in $styleCopResultsFiles) {
        $reportXmlFile = Join-Path -Path $buildArtifactsDirectory -ChildPath $styleCopResultsFile | Resolve-Path;
        $reportHtmlFile = $reportXmlFile -replace ".xml", ".html";
        Join-Path -Path $buildArtifactsDirectory -ChildPath $styleCopResultsFile | analyseStyleCopResults;
        applyXslTransform $reportXmlFile $styleCopXslFile $reportHtmlFile;
      }
            
      $codeAnalysisFiles = Get-ChildItem $buildArtifactsDirectory -Filter "CodeAnalysis*.xml"
      foreach ($codeAnalysisFile in $codeAnalysisFiles) {
        $reportXmlFile = Join-Path -Path $buildArtifactsDirectory -ChildPath $codeAnalysisFile | Resolve-Path;
        $reportHtmlFile = $reportXmlFile -replace ".xml", ".html";
        Join-Path -Path $buildArtifactsDirectory -ChildPath $codeAnalysisFile | analyseCodeAnalysisResults;
        applyXslTransform $reportXmlFile $codeAnalysisXslFile $reportHtmlFile;
      }

      if(isAppVeyor) {
        $expectedMsiFile = Join-Path -Path $buildArtifactsDirectory -ChildPath "ChocolateyGUI.msi"
        if(Test-Path $expectedMsiFile) {
          Push-AppveyorArtifact $expectedMsiFile;
        }
      }
    }

    Write-Host ("************ BuildSolution Successful ************")
  }
  catch {
    Write-Error $_
    Write-Host ("************ BuildSolution Failed ************")
  }
}

Task -Name RebuildSolution -Depends CleanSolution, __CreateBuildArtifactsDirectory, BuildSolution -Description "Rebuilds the main solution for the package"

# clean tasks

Task -Name CleanSolution -Depends __InstallPSBuild, __RemoveBuildArtifactsDirectory, __VerifyConfiguration -Description "Deletes all build artifacts" -Action {
  $sourceDirectory = get-sourceDirectory;
        
  try {
    Write-Output "Running CleanSolution..."

    exec {
      Invoke-MSBuild "$sourceDirectory\ChocolateyGui.sln" -NoLogo -Configuration $config -Targets Clean -DetailedSummary -VisualStudioVersion 12.0 -Properties (@{'Platform'='Mixed Platforms'})
    }

    Write-Host ("************ CleanSolution Successful ************")
  }
  catch {
    Write-Error $_
    Write-Host ("************ CleanSolution Failed ************")
  }
}

# package tasks

Task -Name PackageChocolatey -Description "Packs the module and example package" -Action { 
  $sourceDirectory = get-sourceDirectory;
  $buildArtifactsDirectory = get-buildArtifactsDirectory;
        
  try {
    Write-Output "Running PackageChocolatey..."

    exec { 
      .$nugetExe pack "$sourceDirectory\..\ChocolateyPackage\ChocolateyGUI.nuspec" -OutputDirectory "$buildArtifactsDirectory" -NoPackageAnalysis -version $script:version 

      if(isAppVeyor) {
        $expectedNupkgFile = Join-Path -Path $buildArtifactsDirectory -ChildPath "ChocolateyGUI*.nupkg"
        if(Test-Path $expectedNupkgFile) {
          Push-AppveyorArtifact ($expectedNupkgFile | Resolve-Path).Path;
        }
      }
    }

    Write-Host ("************ PackageChocolatey Successful ************")
  }
  catch {
    Write-Error $_
    Write-Host ("************ PackageChocolatey Failed ************")
  }	
}

Task -Name DeployPacakgeToMyGet -Description "Takes the packaged Chocolatey package and deploys to MyGet.org" -Action {
  $buildArtifactsDirectory = get-buildArtifactsDirectory;
        
  try {
    Write-Output "Deploying to MyGet..."

    exec {
      & $nugetExe push "$buildArtifactsDirectory\*.nupkg" $env:MyGetApiKey -source $env:MyGetFeedUrl
    }

    Write-Host ("************ MyGet Deployment Successful ************")
  }
  catch {
    Write-Error $_
    Write-Host ("************ MyGet Deployment Failed ************")
  }
}

Task -Name DeployPackageToChocolatey -Description "Takes the packaged Chocolatey package and deploys to Chocolatey.org" -Action {
  $buildArtifactsDirectory = get-buildArtifactsDirectory;
        
  try {
    Write-Output "Deploying to Chocolatey..."

    exec {
      & $nugetExe push "$buildArtifactsDirectory\*.nupkg" $env:ChocolateyApiKey -source $env:ChocolateyFeedUrl
    }

    Write-Host ("************ Chocolatey Deployment Successful ************")
  }
  catch {
    Write-Error $_
    Write-Host ("************ Chocolatey Deployment Failed ************")
  }
}
