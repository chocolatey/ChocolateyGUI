---
Order: 20
Title: Purge Icons
Description: Information about purging icons
---

To prevent downloading the same application icon multiple times, Chocolatey GUI caches the icon that is found for each
package, both those that are installed locally, as well as those from remote sources.  These are stored locally on disk
and will remain cached indefinitely.  If/when required, you can click the `Purge Icons` button to completely clear the
icon cache, and it will begin to be built up again.

![Settings Actions Purge Icons](/ChocolateyGUI/assets/img/Screenshots/user_interface_settings_actions_purge_icons.png "Settings Actions Purge Icons")

In addition to purging the cache from within the Chocolatey GUI application, it is also possible to do this using the
`chocolateyguicli` executable.  More information on doing this can be found [here](../../../commands/purge).
