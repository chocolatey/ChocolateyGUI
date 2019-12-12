---
Order: 90
Title: Use Keyboard Bindings
Description: Allows keyboard bindings to be used to interact with different areas of the Chocolatey GUI User Interface
---

By default, Chocolatey GUI ships with some keyboard bindings that make navigating sources easier when using the
keyboard.  In the screenshot below, it is possible to navigate to the `chocolatey.licensed` feed using `CTRL + 3` and
then back to the `This PC` feed using `CTRL + 1`.  It is also possible to navigate to the `chocolatey` feed using
`CTRL + 2`.

![Use Keyyboard Bindings](/ChocolateyGUI/assets/img/Screenshots/feature_use_keyboard_bindings.png "Use Keyboard Bindings")

Disabling this feature will stop these navigations from being possible using these keyboard shortcuts, but they can
still be navigated to using the mouse in the normal way.

:::{.alert .alert-info}
**NOTE:**

Keyboard bindings are only supported for the first 9 sources.  Trying to press `CTRL + 1 + 0` for navigating to a tenth
source will not work.
:::

## Resources

Below is a short video which shows this feature in action:

## Example

This feature can be enabled by running the following command:

```powershell
chocolateyguicli feature enable --name="'UseKeyboardBindings'"
```

This feaure can be disabled by running the following command:

```powershell
chocolateyguicli feature disable --name="'UseKeyboardBindings'"
```

## Default Value

The default value for this feature is enabled.
