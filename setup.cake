#load nuget:https://www.myget.org/F/cake-contrib/api/v2?package=Cake.Recipe&prerelease

Environment.SetVariableNames();

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
                            shouldPublishGitHub: false);

ToolSettings.SetToolSettings(context: Context,
                            dupFinderExcludePattern: new string[] { 
                                BuildParameters.RootDirectoryPath + "/Source/ChocolateyGui/Utilities/Extensions/LinqExtensions.cs",
                                BuildParameters.RootDirectoryPath + "/Source/ChocolateyGui.Subprocess/Hacks.cs",
                                BuildParameters.RootDirectoryPath + "/Source/ChocolateyGui.Subprocess/ChocolateyExtensions.cs",
                                BuildParameters.RootDirectoryPath + "/Source/ChocolateyGui/Utilities/Hacks.cs" },
                            buildPlatformTarget: PlatformTarget.x86);

Build.Run();