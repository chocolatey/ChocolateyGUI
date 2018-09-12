---
Order: 10
Title: Feature Command (chocolateygui feature)
Description: Usage instructions on how to enable/disable Chocolatey GUI features
---

Chocolatey GUI will allow you to interact with features.

# Usage

```powershell
chocolateygui feature [list]|disable|enable [<options/switches]
```

# Examples

```powershell
chocolateygui feature
chocolateygui feature list
chocolateygui feature disable -n=bob
chocolateygui feature enable -n-bob
```

# Options and Switches

```powershell
-?, --help, -h
     Prints out the help menu.

-n, --name=VALUE
     Name - the name of the feature. Required with some actions. Defaults to empty.
```

# Resources

To view a short video about how this command is used, click on the image below:

[![Chocolatey GUI Feature Command](http://img.youtube.com/vi/_AkDNQFoCtc/0.jpg)](http://www.youtube.com/watch?v=_AkDNQFoCtc "Chocolatey GUI Feature Command")