---
Order: 30
Title: Default to Tile View for Remote Source
Description: Default to tile instead of list view. It is still possible to switch during use.
---

## Resources

Below is a short video which shows this feature in action:

## Example

This feature can be enabled by running the following command:

```powershell
chocolateyguicli feature enable --name="'DefaultToTileViewForRemoteSource'"
```

This feaure can be disabled by running the following command:

```powershell
chocolateyguicli feature disable --name="'DefaultToTileViewForRemoteSource'"
```

## Default Value

The default value for this feature is disabled.
