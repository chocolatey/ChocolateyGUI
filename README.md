# ChocolateyGUI
ChocolateyGUI is a user interface for Chocolatey NuGet (a Machine Package Manager for Windows - see http://chocolatey.org).

##Installation
You can install ChocolateyGUI via Chocolatey by executing:
 
```cinst ChocolateyGUI```

If you are interested in trying out the latest pre-release version of ChocolateyGUI then you can use the following installation command:

```cinst ChocolateyGUI -source https://www.myget.org/F/chocolateygui -pre```

This uses the public ChocolateyGUI feed which is hosed on [MyGet.org](https://www.myget.org) as the source.

Further, if you are interested in trying out the new WPF Refresh of ChocolateyGUI which is currently being worked on, you can install it from here:

```cinst ChocolateyGUI -source https://www.myget.org/F/chocolateygui-wpf_refres -pre```

**NOTE:** The above wpf-refresh branch of ChocolateyGUI is VERY new, and as such, it likely to contain issues and missing features.  Please bear with us while we work on this, and understand that this is not even at alpha stage yet.  Having said that, all feedback is welcome, and gratefully received.

##Build Status
###ChocolateyGUI Master Branch
[![ChocolateyGUI Build Status](https://www.myget.org/BuildSource/Badge/chocolateygui?identifier=124190bb-ec73-4776-bfb3-e07bc6658e35)](https://www.myget.org/F/chocolateygui)

###ChocolateyGUI WPF Refresh Branch
[![ChocolateyGUI WPF Refresh Build Status](https://www.myget.org/BuildSource/Badge/chocolateygui-wpf_refresh?identifier=3251a865-0412-44fe-a0c6-3fe479afaa42)](https://www.myget.org/F/chocolateygui-wpf_refresh)

## Support
If you find any problems with ChocolateyGUI, please raise an issue [here](https://github.com/chocolatey/ChocolateyGUI/issues/new).

If you want to ask any questions about ChocolateyGUI, please issue the forum [here](https://groups.google.com/forum/#!forum/chocolateygui)

## Features:
* View all **installed** and **available** packages
* **Update** installed but outdated packages
* **Install** and **uninstall** packages
* See detailed **package information**

![ChocolateyGUI screenshot](http://blogs.lessthandot.com/wp-content/uploads/users/chrissie1/chocolatey/ChocoGUI1.png?mtime=1360428609)

## Blog Posts and external articles

* [All new ChocolateyGUI](http://blogs.lessthandot.com/index.php/SysAdmins/OS/Windows/all-new-chocolateygui) by chrissie1
* [Making a chocolatey package for ChocolateyGUI.](http://blogs.lessthandot.com/index.php/DesktopDev/MSTech/making-a-chocolatey-package) by chrissie1
* [ChocolateyGUI](http://blogs.lessthandot.com/index.php/DesktopDev/MSTech/chocolatey-gui) by chrissie1

