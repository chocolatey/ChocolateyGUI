---
Order: 100
Title: Localization - How To
Description: Localization In Chocolatey GUI
---

## Basics

To enable using localized strings in the Chocolatey GUI UI, the UI loads all its strings from a set of resource files called `resx` files.
**NOTE:** Only the default `Resources.resx` file is committed into source control.  All other translations are downloaded from transifex at the time of compilation (more information on this below).
These `resx` files allows defining language and culture specific strings and resources, while having English fallbacks when they aren't present.
This allows Chocolatey GUI to, in general, be localization neutral.
For more information about localization using `resx` check the CultureInfo section on [MSDN](https://msdn.microsoft.com/en-us/library/system.globalization.cultureinfo(v=vs.110).aspx).

The most important thing to remember is: each language has it's own `Resources.<lang>.resx`, where `<lang>` is referring to the language culture name (as an example `de` for German or `nl` for Dutch).
A list of available cultures and their corresponding codes tags can be found [here](https://msdn.microsoft.com/en-us/library/cc233982.aspx).
If no fitting `<lang>` is present for the current system, the default/fallback `Resources.resx` - containing the English language will be chosen.

When a new release of Chocolatey GUI is created, the resx files at the time of compilation are embedded within the generated assembly.
As a result, any modifications/additions to any files will not be available until the next release of Chocolatey GUI.
**NOTE:** It is possible, assuming you have a transifex token, to download the resx files locally onto your environment. and compile Chocolatey GUI, will would allow you to test any changes locally.  More information on this below.

## transifex

The Chocolatey GUI project makes use of the localization platform known as [transifex](https://www.transifex.com/).
**NOTE:** This is normally a paid for service, however, we are using the Open Source offering that they provide.

### Helping with translations

In order to help with the localization effort for Chocolatey GUI, you will need to create an account on [transifex](https://www.transifex.com/).
Once you have an account, you will be able to request to join the [Chocolatey GUI](https://www.transifex.com/chocolatey/chocolatey-gui/dashboard/) project in transifex.
**NOTE:** This request will need to be approved.
If a request goes unapproved for a period of time, reach out on the Chocolatey GUI [Gitter](https://gitter.im/chocolatey/ChocolateyGUI) room for help.

### Adding a new language

If a language hasn't yet been created for the Chocolatey GUI project that you would like to help with, you will need to make a request to have it added.
This can be done via the [Chocolatey GUI dashboard](https://www.transifex.com/chocolatey/chocolatey-gui/dashboard/).
**NOTE:** Adding a new language to the project will require approval.
If a request goes unapproved for a period of time, reach out on the Chocolatey GUI [Gitter](https://gitter.im/chocolatey/ChocolateyGUI) room for help.
**NOTE:** If you are requesting a new language that uses a full language code, for example `zh_CN` rather than a language code similar to `de`, please get in touch with one of the project maintainers, as a Pull Request similar to [this](https://github.com/chocolatey/ChocolateyGUI/pull/634) will be required.

## Testing Other Languages

### Download transifex files

In order to test other languages within Chocolatey GUI, prior to a release, you will need to do the following:

* Open an Administrative PowerShell Window
* Navigate to the folder where you have cloned the Chocolatey GUI repository
* Run the following command:
  * `.\build.ps1 --target="Transifex-Pull-Translations"`
* You will be asked whether you have a transifex token
  * If so, answer yes, and enter it
  * If not, answer no, and you will be prompted for your transifex username/password
* Once all information is provided, hit enter
* All current resx files should be downloaded into the `Source\ChocolateyGui\Properties` folder
* Build the application by running the command:
  * `.\build.ps1`
* Assuming that completes successfully, you should be able to install the generated MSI, which will contain the most recent translations available

### Reset transifex authentication

If for some reason you need to reset the authentication used by transifex, you will need to delete the persisted information.
This can be found in the root of your Users folder, in a file called `.transifexrc`.
Simply delete this file.
If you do this, you will need to run the `Transifex-Pull-Translations` task again, in order to provide the required information.

### Temporary code change

When testing another language (that does not match the current system locale), a little code-snippet has to be added.
Add the following to your App.xaml.cs (located at `Source/ChocolateyGui/App.xaml.cs`) (right before `application.Run();`)

* **Don't forget** to remove this line of code after you're done testing!

```cs
System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("<lang>");
application.Run();
```

**NOTE:** If you are unsure what you should replace `<lang>` with, a list of available cultures and their corresponding codes tags can be found [here](https://msdn.microsoft.com/en-us/library/cc233982.aspx).

**NOTE:** There is an [open issue](https://github.com/chocolatey/ChocolateyGUI/issues/533) that would allow runtime modification of the current locale, but this hasn't been implemented yet.
Once this has been implemented, this code change will no longer be required.

## Adding New Strings To Chocolatey GUI

The following rules of thumb apply for adding entirely new strings to Chocolatey GUI:

* Always use static bindings (lang:Resources.xyz)
* Use context-sensitive names (`PageOrControlName_StringUsageHintOrName`)
* Always add the new String to `Resources.resx` (English)
* Only changes to this one file should be committed into source control
* Once your changes have been merged into the develop branch, the resx file will be uploaded to Transifex, which will then make the strings available for translation into other languages
