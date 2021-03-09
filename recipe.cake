#load nuget:https://pkgs.dev.azure.com/cake-contrib/Home/_packaging/addins/nuget/v3/index.json?package=Cake.Recipe&version=2.0.0-alpha0276&prerelease

///////////////////////////////////////////////////////////////////////////////
// MODULES
///////////////////////////////////////////////////////////////////////////////
#module nuget:?package=Cake.Chocolatey.Module&version=0.3.0
#module nuget:?package=Cake.BuildSystems.Module&version=0.3.1

///////////////////////////////////////////////////////////////////////////////
// TOOLS
///////////////////////////////////////////////////////////////////////////////
#tool choco:?package=transifex-client&version=0.12.4
#tool nuget:?package=Brutal.Dev.StrongNameSigner&version=2.4.0

///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////
#addin nuget:?package=Cake.StrongNameSigner&version=0.1.0
#addin nuget:?package=Cake.StrongNameTool&version=0.0.4

if(BuildSystem.IsLocalBuild)
{
    Environment.SetVariableNames(
        githubTokenVariable: "CHOCOLATEYGUI_GITHUB_TOKEN",
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
                            solutionFilePath: "./Source/ChocolateyGui.sln",
                            solutionDirectoryPath: "./Source/ChocolateyGui",
                            resharperSettingsFileName: "ChocolateyGui.sln.DotSettings",
                            title: "Chocolatey GUI",
                            repositoryOwner: "chocolatey",
                            repositoryName: "ChocolateyGUI",
                            appVeyorAccountName: "chocolatey",
                            appVeyorProjectSlug: "chocolateygui",
                            shouldDownloadFullReleaseNotes: true,
                            shouldDownloadMilestoneReleaseNotes: true,
                            shouldPublishChocolatey: false,
                            shouldPublishNuGet: false,
                            shouldPublishGitHub: false,
                            shouldRunGitVersion: true,
                            webLinkRoot: "ChocolateyGUI",
                            webBaseEditUrl: "https://github.com/chocolatey/ChocolateyGUI/tree/develop/docs/input/");

ToolSettings.SetToolSettings(context: Context,
                             dupFinderExcludePattern: new string[] {
                                BuildParameters.RootDirectoryPath + "/Source/ChocolateyGui.Common.Windows/Utilities/Converters/BooleanToVisibilityInverted.cs",
                                BuildParameters.RootDirectoryPath + "/Source/ChocolateyGui.Common.Windows/Startup/ChocolateyGuiModule.cs",
                                BuildParameters.RootDirectoryPath + "/Source/ChocoalteyGuiCli/Startup/ChocolateyGuiCliModule.cs"
                            },
                            buildMSBuildToolVersion: MSBuildToolVersion.VS2019
                            );

BuildParameters.PrintParameters(Context);

//var SIGN_TOOL = EnvironmentVariable("SIGN_TOOL") ?? @"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\signtool.exe";
var CERT_PATH = EnvironmentVariable("CHOCOLATEY_OFFICIAL_CERT") ?? "";
var CERT_PASSWORD = EnvironmentVariable("CHOCOLATEY_OFFICIAL_CERT_PASSWORD") ?? "";
var CERT_TIMESTAMP_URL = EnvironmentVariable("CERT_TIMESTAMP_URL") ?? "http://timestamp.digicert.com";
var CERT_ALGORITHM = EnvironmentVariable("CERT_ALGORITHM") ?? "Sha256";

var STRONG_KEY_PATH = "";

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
            BuildParameters.Paths.Directories.PublishedApplications + "/ChocolateyGui/ChocolateyGui.exe",
            BuildParameters.Paths.Directories.PublishedApplications + "/ChocolateyGui/ChocolateyGui.Common.dll",
            BuildParameters.Paths.Directories.PublishedApplications + "/ChocolateyGui/ChocolateyGui.Common.Windows.dll",
            BuildParameters.Paths.Directories.PublishedApplications + "/ChocolateyGuiCli/ChocolateyGuiCli.exe",
            BuildParameters.Paths.Directories.PublishedApplications + "/ChocolateyGuiCli/ChocolateyGui.Common.dll",
            BuildParameters.Paths.Directories.PublishedLibraries + "/ChocolateyGui.Common/ChocolateyGui.Common.dll",
            BuildParameters.Paths.Directories.PublishedLibraries + "/ChocolateyGui.Common.Windows/ChocolateyGui.Common.Windows.dll"
        };

        var platformTarget = ToolSettings.BuildPlatformTarget == PlatformTarget.MSIL ? "AnyCPU" : ToolSettings.BuildPlatformTarget.ToString();
        foreach(var project in ParseSolution(BuildParameters.SolutionFilePath).GetProjects())
        {
            var parsedProject = ParseProject(project.Path, BuildParameters.Configuration, platformTarget);
            if (parsedProject.RootNameSpace == "ChocolateyGui")
            {
                filesToSign.Add(parsedProject.OutputPaths.First().FullPath + "/ChocolateyGui.exe");
                continue;
            }

            if (parsedProject.RootNameSpace == "ChocolateyGuiCli")
            {
                filesToSign.Add(parsedProject.OutputPaths.First().FullPath + "/ChocolateyGuiCli.exe");
                continue;
            }

            if (parsedProject.RootNameSpace == "ChocolateyGui.Common")
            {
                filesToSign.Add(parsedProject.OutputPaths.First().FullPath + "/ChocolateyGui.Common.dll");
                continue;
            }

            if (parsedProject.RootNameSpace == "ChocolateyGui.Common.Windows")
            {
                filesToSign.Add(parsedProject.OutputPaths.First().FullPath + "/ChocolateyGui.Common.Windows.dll");
                continue;
            }
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
    .Does(() => RequireTool(ToolSettings.MSBuildExtensionPackTool, () => {
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

Task("Create-Solution-Info-File")
    .IsDependeeOf("Clean")
    .Does<BuildVersion>((context, buildVersion) =>
    {
        var officialStrongNameKey = EnvironmentVariable("CHOCOLATEYGUI_OFFICIAL_KEY");
        var localUnofficialStrongNameKey = BuildParameters.RootDirectoryPath.CombineWithFilePath("chocolateygui.snk").FullPath;

        if (BuildParameters.Configuration == "ReleaseOfficial" && !string.IsNullOrWhiteSpace(officialStrongNameKey) && FileExists(officialStrongNameKey))
        {
            Information("Using Official Strong Name Key...");
            STRONG_KEY_PATH = officialStrongNameKey;
        }
        else if (FileExists(localUnofficialStrongNameKey))
        {
            Information("Using local Unofficial Strong Name Key...");
            STRONG_KEY_PATH = localUnofficialStrongNameKey;
        }
        else
        {
            Information("Creating new unofficial Strong Name Key...");

            var newChocolateyUnofficialKey = MakeAbsolute(new FilePath(string.Format("{0}.snk", BuildParameters.Title)));

            // If the file already exists, don't re-create it
            if (!FileExists(newChocolateyUnofficialKey))
            {
                // The Cake.StrongNameTool Addin doesn't have an alias for creating a new key, so here I am really
                // abusing an existing alias, and making it run the -k argument.  I plan of raising a PR to the addin
                // to add an actual alias for doing this, for now this, this works, just not pretty.
                var settings = new StrongNameToolSettings();
                settings.ArgumentCustomization = arts => new ProcessArgumentBuilder().Append(string.Format("-k {0}", newChocolateyUnofficialKey.FullPath));
                StrongNameVerify(BuildParameters.SolutionFilePath, settings);
            }

            STRONG_KEY_PATH = newChocolateyUnofficialKey.FullPath;
        }

        // create SolutionVersion.cs file...
        var assemblyInfoSettings = new AssemblyInfoSettings {
            Company = "Chocolatey",
            Version = buildVersion.AssemblySemVer,
            FileVersion = string.Format("{0}.0", buildVersion.Version),
            InformationalVersion = buildVersion.InformationalVersion,
            Product = "Chocolatey GUI",
            Copyright = "Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC"
        };

        var assemblyKeyFileAttribute = new AssemblyInfoCustomAttribute
        {
            Name = "AssemblyKeyFile",
            Value = STRONG_KEY_PATH.Replace("\\", "\\\\").Replace("/", "\\\\"),
            NameSpace = "System.Reflection"
        };

        assemblyInfoSettings.CustomAttributes = new List<AssemblyInfoCustomAttribute>();
        assemblyInfoSettings.CustomAttributes.Add(assemblyKeyFileAttribute);

        CreateAssemblyInfo(BuildParameters.Paths.Files.SolutionInfoFilePath, assemblyInfoSettings);
    });

Task("Strong-Name-Signer")
    .IsDependentOn("Create-Solution-Info-File")
    .IsDependeeOf("Build")
    .Does(() => {
        var settings = new StrongNameSignerSettings();
        settings.KeyFile = STRONG_KEY_PATH;
        var inputDirectoryString = string.Format("{0}{1}", BuildParameters.SourceDirectoryPath.FullPath, "\\packages\\Splat*");
        Information("InputDirectoryString: {0}", inputDirectoryString);
        settings.InputDirectory = inputDirectoryString;
        settings.LogLevel = StrongNameSignerVerbosity.Summary;

        StrongNameSigner(settings);
    });

BuildParameters.Tasks.CreateChocolateyPackagesTask
    .IsDependentOn("SignMSI");

BuildParameters.Tasks.CreateNuGetPackagesTask
    .IsDependentOn("SignExecutable");

Build.Run();
