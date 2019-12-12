---
Order: 60
Title: Show Aggregated Source
Description: Show additional source combining all sources in one place.
---

By default, Chocolatey GUI shows a source view for every feed that is configured for Chocolatey.  Clicking on each tab
will show all of the packages for that selected feed.  As a result, the normal Chocolatey GUI view looks something like
the following:

![Show Aggregated Source Disabled](/ChocolateyGUI/assets/img/Screenshots/feature_show_aggregated_source_disabled.png "Show Aggregated Source Disabled")

By enabling this feature, it is possible to add an additional source view, which aggregates all the packages from all
configured sources, into a single view.  Once enabled, you will see a new `All Sources` tab available for selection,
similar to the following:

![Show Aggregated Source Enabled](/ChocolateyGUI/assets/img/Screenshots/feature_show_aggregated_source_enabled.png "Show Aggregated Source Enabled")

:::{.alert .alert-info}
**NOTE:**

When first enabling this feature, it will be necessary to close and re-open Chocolatey GUI in order to see the new source
view.
:::

## Resources

Below is a short video which shows this feature in action:

## Example

This feature can be enabled by running the following command:

```powershell
chocolateyguicli feature enable --name="'ShowAggregatedSourceView'"
```

This feaure can be disabled by running the following command:

```powershell
chocolateyguicli feature disable --name="'ShowAggregatedSourceView'"
```

## Default Value

The default value for this feature is disabled.
