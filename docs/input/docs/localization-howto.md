---
Description: Localization In Chocolatey GUI
---

## Basics

To enable using localized strings in the Chocolatey GUI UI, the UI loads all its strings from a set of resource files called `resx` files.
These `resx` files allows defining language and culture specific strings and resources, while having english fallbacks when they aren't present.
This allows Chocolatey GUI to, in general, be localization neutral.
For more information about localizaton using `resx` check the CultureInfo section on [MSDN](https://msdn.microsoft.com/en-us/library/system.globalization.cultureinfo(v=vs.110).aspx).

The most important thing to remember is: each language has it's own `Resources.<lang>.resx`, where `<lang>` is referring to the language culture name (as an example `de` for German or `nl` for Dutch).
A list of available cultures and their corresponding codes tags can be found [here](https://msdn.microsoft.com/en-us/library/cc233982.aspx).
If no fitting `<lang>` is present for the current system, the default/fallback `Resources.resx` - containing the English language will be chosen.

## Adding A New Language

1. Make sure you're up-to-date with ChocolateyGUI/develop
2. Create a fork of Chocolatey GUI and a new branch named `localization/<lang>`
3. In VisualStudio: create a copy of `Resources.resx` in the Properties folder and rename it to `Resources.<lang>.resx`
4. Use the resource-editor to change the name-value pairs of the new file

### Testing Other Languages

When testing another language (that does not match the current system locale), a little code-snippet has to be added.
Add the following to your App.xaml.cs (right before `application.Run();`)

* **Don't forget** to remove this line of code after you're done testing!

```cs
System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("<lang>");
application.Run();
```

## Adding New Strings To Chocolatey GUI

The following rules of thumb apply for adding entirely new strings to Chocolatey GUI:

* Always use static bindings (lang:Resources.xyz)
* Use context-sensitive names (`PageOrControlName_StringUsageHintOrName`)
* Always add the new String to `Resources.resx` (English) first
* Be nice, also translate all other languages that you know - immediately after adding a new string
* Open up issues for missing translations
