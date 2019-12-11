---
Order: 40
Title: Use Delayed Search
Description: Enables live search, which returns results after a short delay without clicking the search button.
---

## Resources

Below is a short video which shows this feature in action:

## Example

This feature can be enabled by running the following command:

```powershell
chocolateyguicli feature enable --name="'UseDelayedSearch'"
```

This feaure can be disabled by running the following command:

```powershell
chocolateyguicli feature disable --name="'UseDelayedSearch'"
```

## Default Value

The default value for this feature is disabled.
