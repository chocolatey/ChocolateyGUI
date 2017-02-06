Description: Localization In ChocolateyGUI
---

## Basics
ChocolateyGUI uses `.resx` files for all its strings. 
Basically it detects which [CultureInfo](https://msdn.microsoft.com/en-us/library/system.globalization.cultureinfo(v=vs.110).aspx) 
best fits your current system and chooses the correct `.resx` file automagically.

The most important thing to remember is: each language has it's own `Resources.lang.resx`, where `lang` is referring to the language culture name.
(as example `de` for German or `nl` for Dutch)
A list of culture names can be found [here](https://msdn.microsoft.com/en-us/library/cc233982.aspx).
If no fitting `lang` is present for the current system, the default/fallback `Resources.resx` - containing English language will be chosen.

## Adding A New Language
1. Make sure you're up-to-date with ChocolateyGUI/develop
2. Create a fork of ChocolateyGUI and a new branch named localization/your_language
3. In VisualStudio: create a copy of `Properties/Resources.resx` and rename it to `Properties/Resources.your_language.resx`
4. Use the resource-editor to change the name-value pairs of the new file

### Testing Other Languages
When testing another language (that does not match the current system locale), a little code-snipped has to be added.
Add the following to your App.xaml.cs (right before `application.Run();`) 
* **Don't forget** to remove this line of code after you're done testing!

```
System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("your_language");
application.Run();
```
## Adding Strings To ChocolateyGUI
* Always use static bindings (lang:Resources.xyz)
* Use context-sensitive names (`SomeHintAboutThePage_StringName`)
* Always add the new String to `Resources.resx` (English) first
* Be nice, also translate all other languages that you know immediately after adding a new string
* Open up issues for missing translations

