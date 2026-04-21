#load nuget:?package=Chocolatey.Cake.Recipe&version=0.32.0

///////////////////////////////////////////////////////////////////////////////
// MODULES
///////////////////////////////////////////////////////////////////////////////
#module nuget:?package=Cake.Chocolatey.Module&version=0.3.0

///////////////////////////////////////////////////////////////////////////////
// TOOLS
///////////////////////////////////////////////////////////////////////////////
#tool choco:?package=transifex-cli&version=1.6.5

if (BuildSystem.IsLocalBuild)
{
    Environment.SetVariableNames(
        gitReleaseManagerTokenVariable: "CHOCOLATEYGUI_GITHUB_PAT",
        transifexApiTokenVariable: "CHOCOLATEYGUI_TRANSIFEX_API_TOKEN"
    );
}
else
{
    Environment.SetVariableNames();
}

Func<FilePathCollection> getScriptsToVerify = () =>
{
    var scriptsToVerify = GetFiles(BuildParameters.Paths.Directories.ChocolateyNuspecDirectory + "/**/*.{ps1|psm1|psd1}");

    Information("The following PowerShell scripts have been selected to be verified...");
    foreach (var scriptToVerify in scriptsToVerify)
    {
        Information(scriptToVerify.FullPath);
    }

    var numberOfScriptsToVerify = scriptsToVerify.Count();

    if (numberOfScriptsToVerify != 3)
    {
        throw new Exception(string.Format("Expected to find 3 scripts to verify, but found {0}", numberOfScriptsToVerify));
    }

    return scriptsToVerify;
};

Func<FilePathCollection> getScriptsToSign = () =>
{
    var scriptsToSign = GetFiles(BuildParameters.Paths.Directories.ChocolateyNuspecDirectory + "/**/*.{ps1|psm1|psd1}");

    Information("The following PowerShell scripts have been selected to be signed...");
    foreach (var scriptToSign in scriptsToSign)
    {
        Information(scriptToSign.FullPath);
    }

    var numberOfScriptsToSign = scriptsToSign.Count();

    if (numberOfScriptsToSign != 3)
    {
        throw new Exception(string.Format("Expected to find 3 scripts to verify, but found {0}", numberOfScriptsToSign));
    }

    return scriptsToSign;
};

Func<FilePathCollection> getFilesToSign = () =>
{
    var filesToSign = GetFiles(BuildParameters.Paths.Directories.PublishedApplications + "/^{ChocolateyGui|ChocolateyGuiCli}/net48/{ChocolateyGui|ChocolateyGuiCli}*.{exe|dll}") +
                    GetFiles(BuildParameters.Paths.Directories.PublishedLibraries + "/ChocolateyGui*/net48/ChocolateyGui*.dll");

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

    var numberOfFilesToSign = filesToSign.Count();

    if (numberOfFilesToSign != 13)
    {
        throw new Exception(string.Format("Expected to find 13 files to sign, but found {0}", numberOfFilesToSign));
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

    var numberOfMsisToSign = msisToSign.Count();

    if (numberOfMsisToSign != 1)
    {
        throw new Exception(string.Format("Expected to find 1 msis to sign, but found {0}", numberOfMsisToSign));
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
                            treatWarningsAsErrors: false,
                            productName: "Chocolatey GUI",
                            productDescription: "Chocolatey GUI is a product of Chocolatey Software, Inc. - All Rights Reserved",
                            productCopyright: "Copyright 2014 - Present Open Source maintainers of Chocolatey GUI, and Chocolatey Software, Inc. - All Rights Reserved.",
                            useChocolateyGuiStrongNameKey: true,
                            getScriptsToVerify: getScriptsToVerify,
                            getScriptsToSign: getScriptsToSign,
                            getFilesToSign: getFilesToSign,
                            getMsisToSign: getMsisToSign,
                            shouldBuildMsi: true,
                            strongNameDependentAssembliesInputPath: string.Format("{0}{1}", ((FilePath)("./Source")).FullPath, "\\packages\\Splat*"),
                            shouldRunInspectCode: false);

ToolSettings.SetToolSettings(context: Context);

BuildParameters.PrintParameters(Context);

Build.RunDotNet();
