---
Order: 10
Title: Show Console Output
Description: Shows output from the commands being executed when a job is running.
---

By default, when Chocolatey GUI begins what is known to be an operation that can take a while, it will show a loading
modal window.  When this modal is shown, there is the option to also show the console output for the operation that is
currently being executed.

This is what you will see when an operation like this happens, and this feature is disabled:

![Show Console Output Disabled](/ChocolateyGUI/assets/img/Screenshots/feature_show_console_output_disabled.png "Show Console Output Disabled")

When this feature becomes enabled, you will be default be able to see the contents of the console output, without having
to first expand it:

![Show Console Output Enabled](/ChocolateyGUI/assets/img/Screenshots/feature_show_console_output_enabled.png "Show Console Output Enabled")

## Resources

Below is a short video which shows this feature in action:

## Example

This feature can be enabled by running the following command:

```powershell
chocolateyguicli feature enable --name="'ShowConsoleOutput'"
```

This feaure can be disabled by running the following command:

```powershell
chocolateyguicli feature disable --name="'ShowConsoleOutput'"
```

## Default Value

The default value for this feature is disabled.

## Availability

The ability to control this feature from the Chocolatey GUI Settings screen has existed since Chocolatey GUI v0.15.0.

The ability to control this feature from the command line using `chocolateyguicli` has existed since Chocolatey GUI
v0.17.0.
