#load nuget:https://www.myget.org/F/cake-contrib/api/v2?package=Cake.Recipe&prerelease

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context, 
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./src",
                            title: "ChocolateyGUI",
                            repositoryOwner: "chocolatey",
                            repositoryName: "ChocolateyGUI",
                            appVeyorAccountName: "chocolatey");

ToolSettings.SetToolSettings(Context);

Build.Run();