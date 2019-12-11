---
Order: 20
Title: Default to Tile View for Local Source
Description: Default to tile instead of list view. It is still possible to switch during use.
---

By default, Chocolatey GUI uses a list view to show all of the packages that are currently installed locally on the
machine that is executing Chocolatey GUI.  As a result, you will see a screen similar to the following when first
starting the application:

![Default to Tile View for Local Source Disabled](/ChocolateyGUI/assets/img/Screenshots/feature_default_tile_view_local_disabled.png "Default to Tile View for Local Source Disabled")

It is possible to toggle between the list view and a tile view for the packages that are installed locally using the
buttons at the top right hand corner of the application, but it you prefer to see the tile view by default, then you
can enable it with this feature.  As a result, you will see something like the following when first starting the
application:

![Default to Tile View for Local Source Enabled](/ChocolateyGUI/assets/img/Screenshots/feature_default_tile_view_local_enabled.png "Default to Tile View for Local Source Enabled")

## Resources

Below is a short video which shows this feature in action:

## Example

This feature can be enabled by running the following command:

```powershell
chocolateyguicli feature enable --name="'DefaultToTileViewForLocalSource'"
```

This feaure can be disabled by running the following command:

```powershell
chocolateyguicli feature disable --name="'DefaultToTileViewForLocalSource'"
```

## Default Value

The default value for this feature is disabled.
