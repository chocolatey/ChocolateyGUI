#module nuget:?package=Cake.Chocolatey.Module&version=0.3.0
#load nuget:https://www.myget.org/F/cake-contrib/api/v2?package=Cake.Recipe&version=0.3.0-unstable0457
#tool choco:?package=transifex-client&version=0.12.4

if(BuildSystem.IsLocalBuild)
{
    Environment.SetVariableNames(
        githubUserNameVariable: "CHOCOLATEYGUI_GITHUB_USERNAME",
        githubPasswordVariable: "CHOCOLATEYGUI_GITHUB_PASSWORD",
        appVeyorApiTokenVariable: "CHOCOLATEYGUI_APPVEYOR_API_TOKEN",
        wyamAccessTokenVariable: "CHOCOLATEYGUI_WYAM_ACCESS_TOKEN",
        wyamDeployRemoteVariable: "CHOCOLATEYGUI_WYAM_DEPLOY_REMOTE",
        wyamDeployBranchVariable: "CHOCOLATEYGUI_WYAM_DEPLOY_BRANCH",
        transifexApiTokenVariable: "CHOCOLATEYGUI_TRANSIFEX_API_TOKEN"
    );
}
else
{
    Environment.SetVariableNames();
}

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./Source",
                            title: "ChocolateyGUI",
                            repositoryOwner: "chocolatey",
                            repositoryName: "ChocolateyGUI",
                            appVeyorAccountName: "chocolatey",
                            shouldDownloadFullReleaseNotes: true,
                            shouldDownloadMilestoneReleaseNotes: true,
                            shouldPublishChocolatey: false,
                            shouldPublishNuGet: false,
                            shouldPublishGitHub: false,
                            shouldExecuteGitLink: false);

ToolSettings.SetToolSettings(context: Context,
                             dupFinderExcludePattern: new string[] {
                                BuildParameters.RootDirectoryPath + "/Source/ChocolateyGui/Utilities/Converters/BooleanToVisibilityInverted.cs"
                            });

//var SIGN_TOOL = EnvironmentVariable("SIGN_TOOL") ?? @"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\signtool.exe";
var CERT_PATH = EnvironmentVariable("CHOCOLATEY_OFFICIAL_CERT") ?? "";
var CERT_PASSWORD = EnvironmentVariable("CHOCOLATEY_OFFICIAL_CERT_PASSWORD") ?? "";
var CERT_TIMESTAMP_URL = EnvironmentVariable("CERT_TIMESTAMP_URL") ?? "http://timestamp.digicert.com";
var CERT_ALGORITHM = EnvironmentVariable("CERT_ALGORITHM") ?? "Sha256";

// todo: we need a hook in AFTER compiling before the copy, but it currently
// involves a lot of work to reimplement the Build task to split CopyOutputFiles
// away as a separate task
// So for now, we will just look to sign the executables prior to building MSI
Task("SignExecutable")
    .WithCriteria(() => {
       return !string.IsNullOrWhiteSpace(CERT_PATH) && FileExists(CERT_PATH);
    })
    .Does(() =>
    {
        // needs to bake into VS or separate out to two build steps (two solutions)
        // compilation as a Cake step
        // from maintenance it is better to separate out to two solution files

      //C:\codelocal\chocolateygui\BuildArtifacts\temp\_PublishedApplications\ChocolateyGui\ChocolateyGui.exe

        var filesToSign = new List<string>() {
            BuildParameters.Paths.Directories.PublishedApplications + "/ChocolateyGui/ChocolateyGui.exe"
        };

        var platformTarget = ToolSettings.BuildPlatformTarget == PlatformTarget.MSIL ? "AnyCPU" : ToolSettings.BuildPlatformTarget.ToString();
        foreach(var project in ParseSolution(BuildParameters.SolutionFilePath).GetProjects())
        {

        var parsedProject = ParseProject(project.Path, BuildParameters.Configuration, platformTarget);
        if (parsedProject.RootNameSpace != "ChocolateyGui") continue;

        filesToSign.Add(parsedProject.OutputPath.FullPath + "/ChocolateyGui.exe");
        }

        var password = System.IO.File.ReadAllText(CERT_PASSWORD);

        foreach(var fileToSign in filesToSign)
        {
            Information("Signing '{0}' with {1}", fileToSign, CERT_PATH);

            // Sign(fileToSign, new SignToolSignSettings {
            //     TimeStampUri = new Uri(CERT_TIMESTAMP_URL),
            //     CertPath = CERT_PATH,
            //     Password = password,
            //     DigestAlgorithm = SignToolDigestAlgorithm.Sha1,
            //     TimeStampDigestAlgorithm = SignToolDigestAlgorithm.Sha1
            // });

            // takes the last signed settings

            Sign(fileToSign, new SignToolSignSettings {
                TimeStampUri = new Uri(CERT_TIMESTAMP_URL),
                CertPath = CERT_PATH,
                Password = password,
                DigestAlgorithm = (SignToolDigestAlgorithm)Enum.Parse(typeof(SignToolDigestAlgorithm), CERT_ALGORITHM, true),
                TimeStampDigestAlgorithm = (SignToolDigestAlgorithm)Enum.Parse(typeof(SignToolDigestAlgorithm), CERT_ALGORITHM, true)
            });
        }
});

Task("BuildMSI")
    .IsDependentOn("SignExecutable")
    .Does(() => RequireTool(MSBuildExtensionPackTool, () => {
        Information("Building MSI", BuildParameters.SolutionFilePath);

        var msbuildSettings = new MSBuildSettings()
                .SetPlatformTarget(PlatformTarget.x86)
                .UseToolVersion(ToolSettings.BuildMSBuildToolVersion)
                .WithProperty("TreatWarningsAsErrors","true")
                .WithProperty("MSBuildExtensionsPath32", "C:/Program Files (x86)/MSBuild")
                .WithTarget("Build")
                .SetMaxCpuCount(ToolSettings.MaxCpuCount)
                .SetConfiguration("WIX")
                .WithLogger(
                    Context.Tools.Resolve("MSBuild.ExtensionPack.Loggers.dll").FullPath,
                    "XmlFileLogger",
                    string.Format(
                        "logfile=\"{0}\";invalidCharReplacement=_;verbosity=Detailed;encoding=UTF-8",
                         BuildParameters.Paths.Directories.Build + "/MSBuild.msi.log")
                );

        MSBuild(BuildParameters.SolutionFilePath, msbuildSettings);;
}));

Task("SignMSI")
    .IsDependentOn("BuildMSI")
    .WithCriteria(() => {
       return !string.IsNullOrWhiteSpace(CERT_PATH) && FileExists(CERT_PATH);
    })
    .Does(() =>
    {
        var msiPath = BuildParameters.Paths.Directories.Build + "/ChocolateyGUI.msi";
        var password = System.IO.File.ReadAllText(CERT_PASSWORD);

        Information("Signing '{0}' with {1}", msiPath, CERT_PATH);

        Sign(msiPath, new SignToolSignSettings {
            TimeStampUri = new Uri(CERT_TIMESTAMP_URL),
            CertPath = CERT_PATH,
            Password = password,
            DigestAlgorithm = (SignToolDigestAlgorithm)Enum.Parse(typeof(SignToolDigestAlgorithm), CERT_ALGORITHM, true),
            TimeStampDigestAlgorithm = (SignToolDigestAlgorithm)Enum.Parse(typeof(SignToolDigestAlgorithm), CERT_ALGORITHM, true)
        });

        // dual signing Sha1 and Sha256 for an MSI would require https://github.com/puppetlabs/packaging/blob/8f5c5ff19fa1c495cc82b608464b3bd7e23a2e27/lib/packaging/msi.rb#L14-L63

        // Sign(msiPath, new SignToolSignSettings {
        //     TimeStampUri = new Uri(CERT_TIMESTAMP_URL),
        //     CertPath = CERT_PATH,
        //     Password = password,
        //     TimeStampDigestAlgorithm = SignToolDigestAlgorithm.Sha1
        // });
});

BuildParameters.Tasks.CreateChocolateyPackagesTask
    .IsDependentOn("SignMSI");

Build.Run();
