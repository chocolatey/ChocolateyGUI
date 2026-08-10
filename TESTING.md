## Tests in Chocolatey GUI

Like Chocolatey CLI, Chocolatey GUI also has tests. A good place to start to understand Chocolatey testing is the [Chocolatey CLI TESTING.md document](https://github.com/chocolatey/choco/blob/develop/TESTING.md). In addition to the Unit and Integration tests, Chocolatey GUI has a UITests project.

## Running UITests in Chocolatey GUI

When run from Visual Studio, Chocolatey GUI can operate in one of three setups:

1. **(Default for `DEBUG` builds) Isolated local install.** A `DEBUG` build of Chocolatey GUI points `ChocolateyInstall` at the directory the GUI is running from (its `bin\Debug\net48` output), so Chocolatey reads/writes its own `lib`, `config`, `.chocolatey` store, etc. there instead of touching the machine-wide install. This keeps debugging fully isolated from the Chocolatey installed on your system, mirroring how Chocolatey CLI behaves when debugged out of Visual Studio. A fresh isolated config starts with only the default `chocolatey` community source. To use a folder of your choosing instead of the `bin` output, set the `ChocolateyGuiDebugInstall` environment variable to that path.
2. **The Chocolatey installed to the system** (generally `C:\ProgramData\chocolatey`). To use this from a `DEBUG` build (the old default behaviour), set the environment variable `ChocolateyGuiUseSystemInstall=true`. `Release`/`ReleaseOfficial` builds always use the system install, so this is also the behaviour in CI.
3. **A debug build of Chocolatey** "installed" into the same directory that Chocolatey GUI is running from. This occurs when you run `.\Get-ChocoUpdatedDebugVersion.ps1`, which swaps a `DEBUG`-compiled `chocolatey.dll` in for the referenced (official) one - use this when you need to step into / change Chocolatey.lib itself.

Currently for the UITests to consistently pass, they require a source named `hermes`. In addition, this source requires the `mixed-package` package that is currently only available on a NuGet repository internal to Chocolatey.

How you get the `hermes` source into the configuration used by UITests is up to you. Note that with setup 1 the UITests use the GUI's isolated `bin` config, so add `hermes` there (run the debug GUI and add it through the UI, or edit that `config\chocolatey.config`). Alternatively, set `ChocolateyGuiUseSystemInstall=true` to fall back to setup 2 and add the source to your system installed configuration.

Once you've gotten the source added, you can run the UITests by opening the Test Explorer (View -> Test Explorer), right clicking the ChocolateyGui.UITests collection, and select Run. **IMPORTANT**: Once you select to run the tests, DO NOT source your mouse or keyboard. The tests rely on interacting with the Chocolatey GUI window, and using the mouse or keyboard could impact that and cause tests to fail.