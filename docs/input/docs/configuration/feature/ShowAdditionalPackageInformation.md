---
Order: 70
Title: Show Additional Package Information
Description: Show additional package information on Local and Remote views.
---

By default, when using the list view for local and remote sources, a limited set of information is provided.  This
information is the minimum amount of information that is required to make informed decisions about interacting with the
packages that are shown.  As a result, when using Chocolatey GUI, you will see the following when using the local
source:

![Show Additional Package Information Disabled 1](/ChocolateyGUI/assets/img/Screenshots/feature_show_additional_package_information_disabled_1.png "Show Additional Package Information Disabled 1")

And the following when using a remote source:

![Show Additional Package Information Disabled 2](/ChocolateyGUI/assets/img/Screenshots/feature_show_additional_package_information_disabled_2.png "Show Additional Package Information Disabled 2")

By enabling this feature, you allow the addition of extra package information to be displayed.  For example, in the
local source view, you will see additional columns for package id and whether or not it is a pre-release package:

![Show Additional Package Information Enabled 1](/ChocolateyGUI/assets/img/Screenshots/feature_show_additional_package_information_enabled_1.png "Show Additional Package Information Enabled 1")

And in the remote source view, it will be rendered as follows:

![Show Additional Package Information Enabled 2](/ChocolateyGUI/assets/img/Screenshots/feature_show_additional_package_information_enabled_2.png "Show Additional Package Information Enabled 2")

## Resources

Below is a short video which shows this feature in action:

## Example

This feature can be enabled by running the following command:

```powershell
chocolateyguicli feature enable --name="'ShowAdditionalPackageInformation'"
```

This feaure can be disabled by running the following command:

```powershell
chocolateyguicli feature disable --name="'ShowAdditionalPackageInformation'"
```

## Default Value

The default value for this feature is disabled.
