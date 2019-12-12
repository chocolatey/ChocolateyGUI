---
Order: 100
Title: Hide Package Download Count
Description: Allows control over whether package download count is displayed on remote source views.
---

By default, Chocolatey GUI will attempt to show the download statistics for a package on a remote feed.  This can be
useful when making a decision about whether to install a package or not.  For example, when viewing the Chocolatey
Community Repository feed, you will see the following:

![Hide Package Download Count Disabled 1](/ChocolateyGUI/assets/img/Screenshots/feature_hide_package_download_count_disabled_1.png "Hide Package Download Count Disabled 1")

However, when using a feed that doesn't support package download statisics, you can be shown the following which isn't
as useful:

![Hide Package Download Count Disabled 2](/ChocolateyGUI/assets/img/Screenshots/feature_hide_package_download_count_disabled_2.png "Hide Package Download Count Disabled 2")

By enabling this feature, you can turn off package download count for all sources, and as a result, you will see the
following:

![Hide Package Download Count Enabled 1](/ChocolateyGUI/assets/img/Screenshots/feature_hide_package_download_count_enabled_1.png "Hide Package Download Count Enabled 1")

![Hide Package Download Count Enabled 2](/ChocolateyGUI/assets/img/Screenshots/feature_hide_package_download_count_enabled_2.png "Hide Package Download Count Enabled 2")

:::{.alert .alert-info}
**NOTE:**

It is currently not possible to configure showing/hiding the package download count for individual feeds
:::

## Resources

Below is a short video which shows this feature in action:

## Example

This feature can be enabled by running the following command:

```powershell
chocolateyguicli feature enable --name="'HidePackageDownloadCount'"
```

This feaure can be disabled by running the following command:

```powershell
chocolateyguicli feature disable --name="'HidePackageDownloadCount'"
```

## Default Value

The default value for this feature is disabled.
