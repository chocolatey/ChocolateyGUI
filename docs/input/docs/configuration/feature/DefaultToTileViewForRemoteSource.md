---
Order: 30
Title: Default to Tile View for Remote Source
Description: Default to tile instead of list view. It is still possible to switch during use.
---

By default, Chocolatey GUI uses a list view to show all of the packages that are currently available on the remote
sources that have been configured.  As a result, you will see a screen similar to the following when first
starting the application and clicking on a remote source:

![Default to Tile View for Remote Source Disabled](/ChocolateyGUI/assets/img/Screenshots/feature_default_tile_view_remote_disabled.png "Default to Tile View for Remote Source Disabled")

It is possible to toggle between the list view and a tile view for the packages that are availalbe on a remote source
using the buttons at the top right hand corner of the application, but it you prefer to see the tile view by default,
then you can enable it with this feature.  As a result, you will see something like the following when first starting
the application and clicking on a remote source:

![Default to Tile View for Remote Source Enabled](/ChocolateyGUI/assets/img/Screenshots/feature_default_tile_view_remote_enabled.png "Default to Tile View for Remote Source Enabled")

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

## Availability

The ability to control this feature from the Chocolatey GUI Settings screen has existed since Chocolatey GUI v0.16.0.

The ability to control this feature from the command line using `chocolateyguicli` has existed since Chocolatey GUI
v0.17.0.
