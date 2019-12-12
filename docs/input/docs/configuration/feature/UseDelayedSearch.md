---
Order: 40
Title: Use Delayed Search
Description: Enables live search, which returns results after a short delay without clicking the search button.
---

By default, when viewing the `This PC` source in Chocolatey GUI, anything that you type into the search box at the top
of the screen will, after a short delay, filter the list of packages to what you have typed in.  Some people prefer that
the search isn't executed until they are finished typing, and actually press the enter key on the keyboard.  Enabling
this feature toggles it so that pressing the entry key is a requirement for the search to be performed.

![Use Delayed Search Enabled](/ChocolateyGUI/assets/img/Screenshots/feature_use_delayed_search_enabled.png "Use Delayed Search Enabled")

## Resources

Below is a short video which shows this feature in action:

## Example

This feature can be enabled by running the following command:

```powershell
chocolateyguicli feature enable --name="'UseDelayedSearch'"
```

This feaure can be disabled by running the following command:

```powershell
chocolateyguicli feature disable --name="'UseDelayedSearch'"
```

## Default Value

The default value for this feature is disabled.
