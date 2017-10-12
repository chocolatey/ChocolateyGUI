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
                            shouldPublishGitHub: false,
                            shouldExecuteGitLink: false);

ToolSettings.SetToolSettings(context: Context,
                             buildPlatformTarget: PlatformTarget.x86,
                             buildMSBuildToolVersion: MSBuildToolVersion.VS2015);

Build.Run();