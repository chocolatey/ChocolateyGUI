---
Order: 10
Title: Outdated Packages Cache Duration in Minutes
---

In an attempt to be as efficient as possible, Chocolatey GUI caches the information about which of the installed
packages are currently outdated.  Instead of running the `choco outdated` command every time that information is
requested, the information is instead fetched from the local cache.  The length of time that the outdated packages
information is kept can be controlled via this configuration parameter.  The default is 60 minutes, which can be
increased as required.

:::{.alert .alert-info}
**NOTE:**

Any time a package operation is completed, i.e. install/uninstall/upgrade, the outdated package information cache will
be invalidated.
:::

## Example

To set this configuration parameter you can run the following:

```powershell
chocolateyguicli config set --name="'outdatedPackagesCacheDurationInMinutes'" --value="'120'"
```

## Default Value

The default value for this configuration is 60 minutes.
