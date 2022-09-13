$ChocolateyPackages = choco list -lo -r | ConvertFrom-Csv -Delimiter '|' -Header Package,Version
$NugetPackage = $ChocolateyPackages | Where-Object Package -EQ nuget.commandline
$ChocolateyVersion = $ChocolateyPackages | Where-Object Package -EQ 'chocolatey'

If ($null -eq $NugetPackage) {
    throw "You must have nuget.commandline installed in order to automatically update Chocolatey.lib. Alternatively you can update Chocolatey.lib in Visual Studio to Version '$($ChocolateyVersion.Version)'"
}

nuget restore $PSScriptRoot/Source/ChocolateyGui.sln
nuget update $PSScriptRoot/Source/ChocolateyGui.sln -Id Chocolatey.lib -Version $ChocolateyVersion.Version
