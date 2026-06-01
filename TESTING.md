## Tests in Chocolatey GUI

Like Chocolatey CLI, Chocolatey GUI also has tests. A good place to start to understand Chocolatey testing is the [Chocolatey CLI TESTING.md document](https://github.com/chocolatey/choco/blob/develop/TESTING.md). In addition to the Unit and Integration tests, Chocolatey GUI has a UITests project.

## Running UITests in Chocolatey GUI

When run from Visual Studio, Chocolatey GUI operates in one of two setups: 

1. Chocolatey GUI utilizes the Chocolatey installed to the system (generally `C:\ProgramData\chocolatey`)
2. Chocolatey GUI utilizes a debug build of Chocolatey "installed" into the same directory that Chocolatey GUI is running from. This occurs when you run `.\Get-ChocoUpdatedDebugVersion.ps1`.

Currently for the UITests to consistently pass, they require a source named `hermes`. In addition, this source requires the `mixed-package` package that is currently only available on a NuGet repository internal to Chocolatey.

How you get the `hermes` source into the configuration used by UITests is up to you, if there's no need for an updated Chocolatey.lib, you could just use it in scenario 1 above, and ensure you have the source in your system installed configuration. Alternatively, you could run the debug version of Chocolatey GUI and add it through there.

Once you've gotten the source added, you can run the UITests by opening the Test Explorer (View -> Test Explorer), right clicking the ChocolateyGui.UITests collection, and select Run. **IMPORTANT**: Once you select to run the tests, DO NOT source your mouse or keyboard. The tests rely on interacting with the Chocolatey GUI window, and using the mouse or keyboard could impact that and cause tests to fail.