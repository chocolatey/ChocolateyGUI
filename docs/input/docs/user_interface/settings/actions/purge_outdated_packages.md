---
Order: 10
Title: Purge Outdated Packages
Description: Information about purging outdated packages
---

In order to prevent requesting the same information again and again from Chocolatey, Chocolatey GUI caches information
about the outdated packages which are currently installed on the machine.  It uses this information to display an
outdated banner on the package within the Chocolatey GUI application.  By default, this cache is kept for 60 minutes,
and you can change this configuration value if required.  If, for whatever reason, you need to purge this cache, you
can do so using the `Purge Outdated Packages` button.  When a Chocolatey operation such as `install`, `upgrade`, or
`uninstall` is performed, the Outdated Packages cache will be invalidated.

![Settings Actions Purge Outdated Packages](/ChocolateyGUI/assets/img/Screenshots/user_interface_settings_actions_purge_outdated_packages.png "Settings Actions Purge Outdated Packages")

In addition to purging the cache from within the Chocolatey GUI application, it is also possible to do this using the
`chocolateyguicli` executable.  More information on doing this can be found [here](../../../commands/purge).
