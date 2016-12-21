#load nuget:https://www.myget.org/F/cake-contrib/api/v2?package=Cake.Recipe&prerelease

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context, 
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./Source",
                            title: "ChocolateyGUI",
                            repositoryOwner: "chocolatey",
                            repositoryName: "ChocolateyGUI",
                            appVeyorAccountName: "chocolatey");

ToolSettings.SetToolSettings(context: Context,
                            dupFinderExcludePattern: new string[] { 
                                BuildParameters.RootDirectoryPath + "/Source/ChocolateyGui/Utilities/Extensions/LinqExtensions.cs" },
                            buildPlatformTarget: PlatformTarget.x86);

Build.Run();