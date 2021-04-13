# Chocolatey GUI

Chocolatey GUI is a user interface for [Chocolatey](http://chocolatey.org) (the Machine Package Manager for Windows).

## Installation

You can install Chocolatey GUI via Chocolatey itself by executing:

```choco install ChocolateyGUI```

If you are interested in trying out the latest pre-release version of Chocolatey GUI then you can use the following installation command:

```choco install chocolateygui --source https://www.myget.org/F/chocolateygui/ --pre```

This uses the public Chocolatey GUI feed which is hosted on [MyGet.org](https://www.myget.org) as the source.

## Build Status

TeamCity  | Appveyor
------------- | -------------
[![TeamCity Build Status](http://img.shields.io/teamcity/codebetter/bt613.svg)](http://teamcity.codebetter.com/viewType.html?buildTypeId=bt613) | [![Appveyor Build status](https://ci.appveyor.com/api/projects/status/t7p3ywv3msu5ahl7/branch/develop?svg=true)](https://ci.appveyor.com/project/chocolatey/chocolateygui/branch/develop)


## Chat Room

Come join in the conversation about Chocolatey GUI in our Gitter Chat Room

[![Gitter chat](https://badges.gitter.im/chocolatey/ChocolateyGUI.png)](https://gitter.im/chocolatey/ChocolateyGUI)

Or, you can find us in IRC at #chocolatey.

## Information

* [Community Feed aka Chocolatey.org](https://chocolatey.org) (if this is down, try the backup at <http://chocolatey.apphb.com> )
* [Mailing List/Forum](http://groups.google.com/group/chocolateygui)
* [Twitter](https://twitter.com/chocolateynuget)
* [Build Status Email List](http://groups.google.com/group/chocolatey-build-status)

### Documentation

You can find information about Chocolatey GUI here: [https://docs.chocolatey.org/en-us/chocolatey-gui/](https://docs.chocolatey.org/en-us/chocolatey-gui/)

### Requirements

* .NET Framework 4.8
* Should work on all Windows Operating Systems from Windows 7 SP1 and above, and Windows Server 2008 R2 SP1 and above

### License / Credits

Apache 2.0 - see [LICENSE](https://github.com/chocolatey/chocolateygui/blob/develop/LICENSE.txt) and [NOTICE](https://github.com/chocolatey/chocolateygui/blob/develop/NOTICE) files.

## Submitting Issues

If you have found an issue with Chocolatey GUI, this is the place to submit.

Observe the following help for submitting an issue:

Prerequisites:

* The issue has to do with Chocolatey GUI itself and is not a package or website issue.
* Please check to see if your issue already exists with a quick search of the issues. Start with one relevant term and then add if you get too many results.
* You are not submitting an Enhancement. Enhancements should observe [CONTRIBUTING](https://github.com/chocolatey/chocolateygui/blob/develop/CONTRIBUTING.md) guidelines.

Submitting a ticket:

* We'll need debug and verbose output, so please run and capture the log with `-dv` or `--debug --verbose`. You can submit that with the issue or create a gist and link it.
* **Please note** that the debug/verbose output for some commands may have sensitive data (passwords or apiKeys) related to Chocolatey, so please remove those if they are there prior to submitting the issue.
* choco.exe logs to a file in `$env:ChocolateyInstall\log\`. You can grab the specific log output from there so you don't have to capture or redirect screen output. Please limit the amount included to just the command run (the log is appended to with every command).
* Please save the log output in a [gist](https://gist.github.com) (save the file as `log.sh`) and link to the gist from the issue. Feel free to create it as secret so it doesn't fill up against your public gists. Anyone with a direct link can still get to secret gists. If you accidentally include secret information in your gist, please delete it and create a new one (gist history can be seen by anyone) and update the link in the ticket (issue history is not retained except by email - deleting the gist ensures that no one can get to it). Using gists this way also keeps accidental secrets from being shared in the ticket in the first place as well.
* We'll need the entire log output from the run, so please don't limit it down to areas you feel are relevant. You may miss some important details we'll need to know. This will help expedite issue triage.
* It's helpful to include the version of choco, the version of the OS, and the version of PowerShell (Posh), but the debug script should capture all of those pieces of information.
* Include screenshots and/or animated gif's whenever possible, they help show us exactly what the problem is.

## Contributing

If you would like to contribute code or help squash a bug or two, that's awesome. Please familiarize yourself with [CONTRIBUTING](https://github.com/chocolatey/chocolateygui/blob/develop/CONTRIBUTING.md).

### Building

* It is assumed that a version of Visual Studio 2019 is already installed on the machine being used to complete the build.
* `choco install wixtoolset -y`
* **OPTIONAL:** Set `FXCOPDIR` environment variable, which can be set using [vswhere](https://chocolatey.org/packages/vswhere) and the following command:
   ```ps1
   $FXCOPDIR = vswhere -products * -latest -prerelease -find **/FxCopCmd.exe
   [Environment]::SetEnvironmentVariable("FXCOPDIR", $FXCOPDIR, 'User')
   refreshenv
   ```
* Install WiX toolset integration for your Visual Studio Integration from [here](https://marketplace.visualstudio.com/items?itemName=WixToolset.WixToolsetVisualStudio2019Extension)
* From and **Administrative** PowerShell Window, navigate to the folder where you have cloned the Chocolatey GUI repository and run `build.ps1`, this will run Cake and it will go through the build script.
  ```
  ./build.ps1
  ```

### Localization

If you are interested in helping with the effort in translating the various portions of the Chocolatey GUI UI into different languages, you can find out more about using the [transifex](https://www.transifex.com/) service in this [how to article](https://chocolatey.github.io/ChocolateyGUI/docs/localization).

## Committers

Committers, you should be very familiar with [COMMITTERS](https://github.com/chocolatey/chocolateygui/blob/develop/COMMITTERS.md).

## Features:

* View all **installed** and **available** packages
* **Update** installed but outdated packages
* **Install** and **uninstall** packages
* See detailed **package information**

![Chocolatey GUI](https://github.com/chocolatey/ChocolateyGUI/blob/10809890189206cece4b64ab038f33d11cf7b840/docs/Screenshots/Application_Loaded.png)

## Credits

Chocolatey GUI is brought to you by quite a few people and frameworks. See [CREDITS](https://github.com/chocolatey/chocolateygui/blob/develop/CREDITS.md) (just CREDITS.md in the zip folder)
