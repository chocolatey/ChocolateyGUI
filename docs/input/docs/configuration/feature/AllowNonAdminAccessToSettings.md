---
Order: 80
Title: Allow Non Admin Access to Settings
Description: Controls whether or not a non-administrator user can access the Settings Screen.  NOTE - This feature will only work when using the licensed extension for Chocolatey and Chocolatey GUI.
---

When using Chocolatey GUI for Business a new feature is available in the Settings screen:

![Allow non-admin access to Settings](/ChocolateyGUI/assets/img/Screenshots/allow_non_admin_access_to_settings.png "Allow non-admin access to Settings")

Which controls whether or not a non-admin user has access to the Settings screen or not.

By default, this setting is enabled, so that we don't introduce a breaking change in the way that the application has worked to date.  However, at some point, this will likely be flipped to not allowing non-admin users to access the settings.

When this setting is turned off, a non-admin user will no longer be able to access the Settings screen, as shown below:

![Access to Settings screen removed](/ChocolateyGUI/assets/img/Screenshots/access_to_settings_removed.png "Access to Settings screen removed")

:::{.alert .alert-info}
**NOTE:**

Regardless of this setting, an administrator user will always be able to access the Settings screen.
:::

## Resources

Below is a short video which shows this feature in action:

<iframe width="700" height="506" src="https://www.youtube.com/embed/VCTHWo7cgW0" frameborder="0" allow="autoplay; encrypted-media" allowfullscreen></iframe>

## Example

This feature can be enabled by running the following command:

```powershell
chocolateyguicli feature enable --name="'AllowNonAdminAccessToSettings'"
```

This feaure can be disabled by running the following command:

```powershell
chocolateyguicli feature disable --name="'AllowNonAdminAccessToSettings'"
```

## Default Value

The default value for this feature is disabled.

## Availability

The ability to control this feature from the Chocolatey GUI Settings screen has existed since Chocolatey GUI v0.17.0.

The ability to control this feature from the command line using `chocolateyguicli` has existed since Chocolatey GUI
v0.17.0.
