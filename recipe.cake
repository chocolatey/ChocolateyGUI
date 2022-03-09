#load nuget:?package=Chocolatey.Cake.Recipe&version=0.2.2

///////////////////////////////////////////////////////////////////////////////
// MODULES
///////////////////////////////////////////////////////////////////////////////
#module nuget:?package=Cake.Chocolatey.Module&version=0.3.0

///////////////////////////////////////////////////////////////////////////////
// TOOLS
///////////////////////////////////////////////////////////////////////////////
#tool choco:?package=transifex-client&version=0.12.4

if (BuildSystem.IsLocalBuild)
{
    Environment.SetVariableNames(
        gitHubTokenVariable: "CHOCOLATEYGUI_GITHUB_PAT",
        transifexApiTokenVariable: "CHOCOLATEYGUI_TRANSIFEX_API_TOKEN"
    );
}
else
{
    Environment.SetVariableNames();
}

Func<FilePathCollection> getScriptsToSign = () =>
{
    var scriptsToSign = GetFiles(BuildParameters.Paths.Directories.NuGetNuspecDirectory + "/**/*.ps1") +
                        GetFiles(BuildParameters.Paths.Directories.ChocolateyNuspecDirectory + "/**/*.ps1");

    Information("The following PowerShell scripts have been selected to be signed...");
    foreach (var scriptToSign in scriptsToSign)
    {
        Information(scriptToSign.FullPath);
    }

    return scriptsToSign;
};

Func<FilePathCollection> getFilesToSign = () =>
{
    var filesToSign = GetFiles(BuildParameters.Paths.Directories.PublishedApplications + "/^{ChocolateyGui|ChocolateyGuiCli}/{ChocolateyGui|ChocolateyGuiCli}*.{exe|dll}") +
                    GetFiles(BuildParameters.Paths.Directories.PublishedLibraries + "/ChocolateyGui*/ChocolateyGui*.dll");

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

    Information("The following assemblies have been selected to be signed...");
    foreach (var fileToSign in filesToSign)
    {
        Information(fileToSign.FullPath);
    }

    return filesToSign;
};

Func<FilePathCollection> getMsisToSign = () =>
{
    var msisToSign = GetFiles(BuildParameters.Paths.Directories.Build + "/ChocolateyGUI.msi");

    Information("The following msi's have been selected to be signed...");
    foreach (var msiToSign in msisToSign)
    {
        Information(msiToSign.FullPath);
    }

    return msisToSign;
};

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./Source",
                            solutionFilePath: "./Source/ChocolateyGui.sln",
                            solutionDirectoryPath: "./Source/ChocolateyGui",
                            resharperSettingsFileName: "ChocolateyGui.sln.DotSettings",
                            title: "Chocolatey GUI",
                            repositoryOwner: "chocolatey",
                            repositoryName: "ChocolateyGUI",
                            shouldDownloadMilestoneReleaseNotes: true,
                            productName: "Chocolatey GUI",
                            productDescription: "Chocolatey GUI is a product of Chocolatey Software, Inc. - All Rights Reserved",
                            productCopyright: "Copyright 2014 - Present Open Source maintainers of Chocolatey GUI, and Chocolatey Software, Inc. - All Rights Reserved.",
                            useChocolateyGuiStrongNameKey: true,
                            getScriptsToSign: getScriptsToSign,
                            getFilesToSign: getFilesToSign,
                            getMsisToSign: getMsisToSign,
                            shouldBuildMsi: true,
                            strongNameDependentAssembliesInputPath: string.Format("{0}{1}", ((FilePath)("./Source")).FullPath, "\\packages\\Splat*"));

ToolSettings.SetToolSettings(context: Context,
                            buildMSBuildToolVersion: MSBuildToolVersion.VS2019);

BuildParameters.PrintParameters(Context);

Build.Run();
