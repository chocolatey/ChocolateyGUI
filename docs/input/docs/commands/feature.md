---
Order: 10
Title: Feature
Description: Usage instructions on how to enable/disable Chocolatey GUI features
---

Chocolatey GUI will allow you to interact with features.

## Usage

```powershell
chocolateyguicli feature [list]|disable|enable [<options/switches]
```

## Examples

```powershell
chocolateyguicli feature
chocolateyguicli feature list
chocolateyguicli feature disable --name="'bob'"
chocolateyguicli feature enable --name="'bob'"
```

## Options and Switches

```powershell
-?, --help, -h
     Prints out the help menu.

-n, --name=VALUE
     Name - the name of the feature. Required with some actions. Defaults to empty.
```

## Resources

Below is a short video which shows this in action:

<iframe width="700" height="506" src="https://www.youtube.com/embed/_AkDNQFoCtc" frameborder="0" allow="autoplay; encrypted-media" allowfullscreen></iframe>
