---
Order: 20
Title: Feature
Description: Usage instructions on how to enable/disable Chocolatey GUI features
---

Chocolatey GUI will allow you to interact with features.

## Usage

```powershell
chocolateyguicli feature [list]|disable|enable [<options/switches]
```

## Examples

```powershell
chocolateyguicli feature
chocolateyguicli feature list
chocolateyguicli feature disable --name="'bob'"
chocolateyguicli feature enable --name="'bob'"
```

## Exit Codes

Exit codes that normally result from running this command.

Normal:

- 0: operation was successful, no issues detected
- -1 or 1: an error has occurred

## Options and Switches

```powershell
-?, --help, -h
     Prints out the help menu.

-n, --name=VALUE
     Name - the name of the feature. Required with some actions. Defaults to empty.
```

## Resources

Below is a short video which shows this in action:

<iframe width="700" height="506" src="https://www.youtube.com/embed/_AkDNQFoCtc" frameborder="0" allow="autoplay; encrypted-media" allowfullscreen></iframe>

## Feature Options

The available feautres that can be configured are:

* [Allow Non Admin Access to Settings](../configuration/feature/AllowNonAdminAccessToSettings)
* [Default to Tile View for Local Source](../configuration/feature/DefaultToTileViewForLocalSource)
* [Default to Tile VIew for Remote Source](../configuration/feature/DefaultToTileViewForRemoteSource)
* [Exclude Installed Packages](../configuration/feature/ExcludeInstalledPackages)
* [Hide Package Download Count](../configuration/feature/HidePackageDownloadCount)
* [Show Additional Package Information](../configuration/feature/ShowAdditionalPackageInformation)
* [Show Aggregated Source](../configuration/feature/ShowAggregatedSourceView)
* [Show Console Output](../configuration/feature/ShowConsoleOutput)
* [Use Delayed Search](../configuration/feature/UseDelayedSearch)
* [Use Keyboard Bindings](../configuration/feature/UseKeyboardBindings)