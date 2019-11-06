---
Order: 10
Title: Config
Description: Usage instructions on how to list/get/set/unset Chocolatey GUI config settings.
---

Chocolatey GUI will allow you to interact with the config settings.

# Usage

```powershell
chocolateyguicli config [list]|get|set|unset [<options/switches]
```

# Examples

```powershell
chocolateyguicli config
chocolateyguicli config list
chocolateyguicli config get --name="'outdatedPackagesCacheDurationInMinutes'"
chocolateyguicli config set --name="'outdatedPackagesCacheDurationInMinutes'" --value="'60'"
chocolateyguicli config unset --name="'outdatedPackagesCacheDurationInMinutes'"
```

# Options and Switches

```powershell
-?, --help, -h
     Prints out the help menu.

    --name=VALUE
     Name - the name of the config setting. Required with some actions.

    --value=VALUE
     Value - the value of the config setting.  Required with some actions.
```

# Resources

Below is a short video which shows this in action:

