---
Order: 10
Title: Allow non-admin access to Settings
Description: Describe how access to the Chocolatey GUI Settings screen can be restricted to non-admin users.
---

When using Chocolatey GUI for Business a new feature is available in the Settings screen:

![Allow non-admin access to Settings](/ChocolateyGUI/assets/img/Screenshots/allow_non_admin_access_to_settings.png "Allow non-admin access to Settings")

Which controls whether or not a non-admin user has access to the Settings screen or not.

By default, this setting is enabled, so that we don't introduce a breaking change in the way that the application has worked to date.  However, at some point, this will likely be flipped to not allowing non-admin users to access the settings.

When this setting is turned off, a non-admin user will no longer be able to access the Settings screen, as shown below:

![Access to Settings screen removed](/ChocolateyGUI/assets/img/Screenshots/access_to_settings_removed.png "Access to Settings screen removed")

**NOTE:** Regardless of this setting, an administrator user will always be able to access the Settings screen.

# Resources

Below is a short video which shows this feature in action:

<iframe width="900" height="506" src="https://www.youtube.com/embed/VCTHWo7cgW0" frameborder="0" allow="autoplay; encrypted-media" allowfullscreen></iframe>