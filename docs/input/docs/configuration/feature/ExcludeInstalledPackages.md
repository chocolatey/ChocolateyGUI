---
Order: 50
Title: Exclude Installed Packages
Description: Do not show packages that are already installed when viewing sources.
---

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
