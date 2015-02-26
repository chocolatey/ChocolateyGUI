## 0.12.0

As part of this release we had [74 issues](https://github.com/chocolatey/ChocolateyGUI/issues?milestone=2&state=closed) closed.

This release is a complete re-architecture of ChocolateyGUI, switching from a Windows Forms Application to a WPF Application.  All credit goes to @RichiCoder1 for the amazing amount of work put into this.  In addition, we have added a more stringent build process, making use of tools like FxCop, StyleCop, DupFinder and InspectCode, as well as Continuous Integration services like AppVeyor and TeamCity CodeBetter.

__Features__

- [__#189__](https://github.com/chocolatey/ChocolateyGUI/issues/189) Administrator Issues
- [__#185__](https://github.com/chocolatey/ChocolateyGUI/issues/185) Right click context menu for packages should show "Details"
- [__#177__](https://github.com/chocolatey/ChocolateyGUI/issues/177) A feature under the menu strip to be able to refresh package lists.
- [__#163__](https://github.com/chocolatey/ChocolateyGUI/issues/163) Add EditorConfig File
- [__#125__](https://github.com/chocolatey/ChocolateyGUI/issues/125) [Feature] Add Raygun.io to application
- [__#122__](https://github.com/chocolatey/ChocolateyGUI/issues/122) Should be able to cancel Update All
- [__#121__](https://github.com/chocolatey/ChocolateyGUI/issues/121) Change "Updating all packages."
- [__#118__](https://github.com/chocolatey/ChocolateyGUI/issues/118) Add Maintainers to Package View
- [__#114__](https://github.com/chocolatey/ChocolateyGUI/issues/114) [Suggestion] Provide context menu for common actions
- [__#113__](https://github.com/chocolatey/ChocolateyGUI/issues/113) [Suggestion] Ability to do update all
- [__#112__](https://github.com/chocolatey/ChocolateyGUI/issues/112) [Suggestion] Provide additional information in Local Grid
- [__#111__](https://github.com/chocolatey/ChocolateyGUI/issues/111) [Suggestion] Rename "Local" to "This PC"
- [__#110__](https://github.com/chocolatey/ChocolateyGUI/issues/110) [Suggestion] Change the way the application starts up
- [__#109__](https://github.com/chocolatey/ChocolateyGUI/issues/109) Fill out support for alernate package sources
- [__#106__](https://github.com/chocolatey/ChocolateyGUI/issues/106) Add a way to show all updates or maybe sort by last updated
- [__#101__](https://github.com/chocolatey/ChocolateyGUI/issues/101) Navigate back to Local Source List after uninstalling or updating a package.
- [__#96__](https://github.com/chocolatey/ChocolateyGUI/issues/96) Implement Package Service
- [__#94__](https://github.com/chocolatey/ChocolateyGUI/issues/94) Add Package View Page
- [__#92__](https://github.com/chocolatey/ChocolateyGUI/issues/92) ChocolateyGUI should show license information for packages
- [__#91__](https://github.com/chocolatey/ChocolateyGUI/issues/91) Update Readme with information about contributing
- [__#90__](https://github.com/chocolatey/ChocolateyGUI/issues/90) Update Wix package to include output from WPF Project
- [__#81__](https://github.com/chocolatey/ChocolateyGUI/issues/81) Update information in Help | About Page
- [__#40__](https://github.com/chocolatey/ChocolateyGUI/issues/40) .Net Framework 4.0 Dependency
- [__#32__](https://github.com/chocolatey/ChocolateyGUI/issues/32) chocolatey allows you to do a force update
- [__#9__](https://github.com/chocolatey/ChocolateyGUI/issues/9) Add better search via the odata feed
- [__#8__](https://github.com/chocolatey/ChocolateyGUI/issues/8) Add paging to the available packages list

__Improvements__

- [__#200__](https://github.com/chocolatey/ChocolateyGUI/issues/200) Correct Line Endings Configuration for repository
- [__#199__](https://github.com/chocolatey/ChocolateyGUI/issues/199) Add a title in the taskbar
- [__#196__](https://github.com/chocolatey/ChocolateyGUI/issues/196) Fix AppVeyor Build
- [__#195__](https://github.com/chocolatey/ChocolateyGUI/issues/195) Add AppVeyor notifications
- [__#191__](https://github.com/chocolatey/ChocolateyGUI/pull/191) Checkbox replaced with tick/cross chars
- [__#180__](https://github.com/chocolatey/ChocolateyGUI/issues/180) Adding Build Caching in AppVeyor
- [__#172__](https://github.com/chocolatey/ChocolateyGUI/issues/172) Update the Build and Testing Process
- [__#168__](https://github.com/chocolatey/ChocolateyGUI/issues/168) Fix Code Analysis Issues
- [__#160__](https://github.com/chocolatey/ChocolateyGUI/issues/160) Setup ReSharper Duplicate Checker
- [__#159__](https://github.com/chocolatey/ChocolateyGUI/issues/159) Setup ReSharper Code Inspections
- [__#158__](https://github.com/chocolatey/ChocolateyGUI/issues/158) Setup Code Analysis
- [__#157__](https://github.com/chocolatey/ChocolateyGUI/issues/157) Setup StyleCop
- [__#152__](https://github.com/chocolatey/ChocolateyGUI/issues/152) Add Deployment Task to Chocolatey
- [__#151__](https://github.com/chocolatey/ChocolateyGUI/issues/151) Add Deployment Task to MyGet
- [__#149__](https://github.com/chocolatey/ChocolateyGUI/issues/149) Inspect AppVeyor Environment Variables to decide what action should be taken
- [__#148__](https://github.com/chocolatey/ChocolateyGUI/issues/148) Add Solution Folders for BuildScripts and appveyor.yml file
- [__#146__](https://github.com/chocolatey/ChocolateyGUI/issues/146) Only run GitVersion UpdateAssemblyInfo if on AppVeyor
- [__#144__](https://github.com/chocolatey/ChocolateyGUI/issues/144) Setup Build Caching in AppVeyor to speed up build
- [__#142__](https://github.com/chocolatey/ChocolateyGUI/issues/142) Add appveyor.yml to control build process
- [__#140__](https://github.com/chocolatey/ChocolateyGUI/issues/140) Add GitVersion to source code and execute from psake
- [__#135__](https://github.com/chocolatey/ChocolateyGUI/issues/135) Update MahApps.Metro version.
- [__#98__](https://github.com/chocolatey/ChocolateyGUI/issues/98) Implement Logging
- [__#89__](https://github.com/chocolatey/ChocolateyGUI/issues/89) Create MyGet Build Service for the wpf-refresh branch

__Bugs__

- [__#204__](https://github.com/chocolatey/ChocolateyGUI/issues/204) Unable to add source
- [__#201__](https://github.com/chocolatey/ChocolateyGUI/issues/201) Uninstall script doesn't wait for the uninstaller to finish
- [__#198__](https://github.com/chocolatey/ChocolateyGUI/issues/198) Installation broken in latest release
- [__#193__](https://github.com/chocolatey/ChocolateyGUI/issues/193) Not all installed packages get listed
- [__#174__](https://github.com/chocolatey/ChocolateyGUI/issues/174) Fix Broken Markdown.cs
- [__#127__](https://github.com/chocolatey/ChocolateyGUI/issues/127) Potential issue with "INSTALLED" checkbox
- [__#126__](https://github.com/chocolatey/ChocolateyGUI/issues/126) Unable to run latest ChocolateyGUI package from MyGet
- [__#123__](https://github.com/chocolatey/ChocolateyGUI/issues/123) When not running ChocolateyGUI, each package update present UAC prompt
- [__#120__](https://github.com/chocolatey/ChocolateyGUI/issues/120) Index was outside the bounds of the array
- [__#117__](https://github.com/chocolatey/ChocolateyGUI/issues/117) [BUG] Packages appear twice in Local Tab
- [__#116__](https://github.com/chocolatey/ChocolateyGUI/issues/116) [BUG] Delete button can't always be seen
- [__#105__](https://github.com/chocolatey/ChocolateyGUI/issues/105) Details Page: Items not quite lined up
- [__#104__](https://github.com/chocolatey/ChocolateyGUI/issues/104) Problem with searching when on second page of results
- [__#100__](https://github.com/chocolatey/ChocolateyGUI/issues/100) Remove packages from Local package list when they're uninstalled
- [__#97__](https://github.com/chocolatey/ChocolateyGUI/issues/97) Fix cuninst of ChocolateyGUI
- [__#85__](https://github.com/chocolatey/ChocolateyGUI/issues/85) Unable to get packages and package information from behind proxy.
- [__#70__](https://github.com/chocolatey/ChocolateyGUI/issues/70) Problem adding new package source
- [__#67__](https://github.com/chocolatey/ChocolateyGUI/issues/67) Unhandled Exception

## Where to get it
You can download this release from [chocolatey](https://chocolatey.org/packages/ChocolateyGUI)

## 0.11.4 (September 16, 2014)

The following issues have been worked on in this release:

__Bugs__

- [__#136__](https://github.com/chocolatey/ChocolateyGUI/issues/136) - Correct releasenotes in Chocolatey nuspec file
 

## 0.11.2 (September 16, 2014)

The following issues have been worked on in this release:

__Improvements__

- [__#75__](https://github.com/chocolatey/ChocolateyGUI/issues/75) - Move settings to main menu strip
- [__#63__](https://github.com/chocolatey/ChocolateyGUI/issues/63) - Include the chocolateyUninstall.ps1 in the nuspec file

__Bugs__

- [__#78__](https://github.com/chocolatey/ChocolateyGUI/issues/78) - ChocolateyVersionUnknownException incorrectly thrown
- [__#64__](https://github.com/chocolatey/ChocolateyGUI/issues/64) - Fix typo in release notes for 0.11.0
- [__#3__](https://github.com/chocolatey/ChocolateyGUI/issues/3) - There's no uninstall script in the 0.0.5 package

## 0.11.1 (February 24, 2013)

__Bugs__

 - uninstall should now work
 - program crashing when packages came out of cache.

## 0.11.0 (February 24, 2013)

__Improvements__

 - Many new features, including UI enhancements and auto building from MyGet.
 - Better logging and Log form so you can find those log files more easily.
 - In the settings you can empty the cache, better caching of packages and list
 - better parsing of dependencies
 - selecting of sources from the sources.xml file. For now just 2 sources.
 - release notes and copyright information in seperate tab
 - chocolatey run information in seperate tab

## 0.1.4 (February 10, 2013)

__Improvements__

 - Better description for the package.

## 0.1.3 (February 10, 2013)

__Bugs__

 - Installer did not remove older version.

## 0.1.2 (February 10, 2013)

__Improvements__

 - package now has iconURL.

## 0.1.1 (February 10, 2013)

__Improvements__

 - The installer should now check if .Net framework 4.0 is installed.

## 0.1.0 (February 9, 2013)

__Improvements__

 - More information about a package
 - Shows chocolatey as an installed package.
 - Performance improvement
 - You can search in the list. It is a StartsWith search.
 - You can install the available package by clicking on the checkbox.
 - You can uninstall a package.

## 0.0.5 (September 10, 2011)

 - First working version
