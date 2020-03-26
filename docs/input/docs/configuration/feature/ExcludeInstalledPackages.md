---
Order: 50
Title: Exclude Installed Packages
Description: Do not show packages that are already installed when viewing sources.
---

By default, when viewing remote sources, such as the Chocolatey Community Repository, Chocolatey GUI will show you all
availalbe packages, even if you already have them installed.  Installed packages will be marked with a green banner,
indicating that they are currently installed.  As a result, you may see something like the following:

![Exclude Installed Packages Disabled](/ChocolateyGUI/assets/img/Screenshots/feature_exclude_installed_packages_disabled.png "Exclude Installed Packages Disabled")

By enabling this feature, packages that are already installed will no longer be shown in the list of available packages,
as shown here:

![Exclude Installed Packages Enabled](/ChocolateyGUI/assets/img/Screenshots/feature_exclude_installed_packages_enabled.png "Exclude Installed Packages Enabled")

:::{.alert .alert-info}
**NOTE:**

When first enabling this feature, if the remote source was already open, it will be refresh to refresh the package list
before the packages will be excluded.
:::

## Resources

Below is a short video which shows this feature in action:

## Example

This feature can be enabled by running the following command:

```powershell
chocolateyguicli feature enable --name="'ExcludeInstalledPackages'"
```

This feaure can be disabled by running the following command:

```powershell
chocolateyguicli feature disable --name="'ExcludeInstalledPackages'"
```

## Default Value

The default value for this feature is disabled.

## Availability

The ability to control this feature from the Chocolatey GUI Settings screen has existed since Chocolatey GUI v0.17.0.

The ability to control this feature from the command line using `chocolateyguicli` has existed since Chocolatey GUI
v0.17.0.
