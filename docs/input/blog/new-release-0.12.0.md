---
Title: Chocolatey GUI 0.12.0
Category: Release
Author: gep13
Published: 28/2/2015
---

This release is a complete re-architecture of Chocolatey GUI, switching from a Windows Forms Application to a WPF Application.  All credit goes to [Richard Simpson](https://github.com/RichiCoder1) for the amazing amount of work put into this.  In addition, we have added a more stringent build process, making use of tools like FxCop, StyleCop, DupFinder and InspectCode, as well as Continuous Integration services like AppVeyor and TeamCity CodeBetter.

## Features

- [**#189**](https://github.com/chocolatey/ChocolateyGUI/issues/189) Administrator Issues
- [**#185**](https://github.com/chocolatey/ChocolateyGUI/issues/185) Right click context menu for packages should show "Details"
- [**#177**](https://github.com/chocolatey/ChocolateyGUI/issues/177) A feature under the menu strip to be able to refresh package lists.
- [**#163**](https://github.com/chocolatey/ChocolateyGUI/issues/163) Add EditorConfig File
- [**#125**](https://github.com/chocolatey/ChocolateyGUI/issues/125) [Feature] Add Raygun.io to application
- [**#122**](https://github.com/chocolatey/ChocolateyGUI/issues/122) Should be able to cancel Update All
- [**#121**](https://github.com/chocolatey/ChocolateyGUI/issues/121) Change "Updating all packages."
- [**#118**](https://github.com/chocolatey/ChocolateyGUI/issues/118) Add Maintainers to Package View
- [**#114**](https://github.com/chocolatey/ChocolateyGUI/issues/114) [Suggestion] Provide context menu for common actions
- [**#113**](https://github.com/chocolatey/ChocolateyGUI/issues/113) [Suggestion] Ability to do update all
- [**#112**](https://github.com/chocolatey/ChocolateyGUI/issues/112) [Suggestion] Provide additional information in Local Grid
- [**#111**](https://github.com/chocolatey/ChocolateyGUI/issues/111) [Suggestion] Rename "Local" to "This PC"
- [**#110**](https://github.com/chocolatey/ChocolateyGUI/issues/110) [Suggestion] Change the way the application starts up
- [**#109**](https://github.com/chocolatey/ChocolateyGUI/issues/109) Fill out support for alernate package sources
- [**#106**](https://github.com/chocolatey/ChocolateyGUI/issues/106) Add a way to show all updates or maybe sort by last updated
- [**#101**](https://github.com/chocolatey/ChocolateyGUI/issues/101) Navigate back to Local Source List after uninstalling or updating a package.
- [**#96**](https://github.com/chocolatey/ChocolateyGUI/issues/96) Implement Package Service
- [**#94**](https://github.com/chocolatey/ChocolateyGUI/issues/94) Add Package View Page
- [**#92**](https://github.com/chocolatey/ChocolateyGUI/issues/92) Chocolatey GUI should show license information for packages
- [**#91**](https://github.com/chocolatey/ChocolateyGUI/issues/91) Update Readme with information about contributing
- [**#90**](https://github.com/chocolatey/ChocolateyGUI/issues/90) Update Wix package to include output from WPF Project
- [**#81**](https://github.com/chocolatey/ChocolateyGUI/issues/81) Update information in Help | About Page
- [**#40**](https://github.com/chocolatey/ChocolateyGUI/issues/40) .Net Framework 4.0 Dependency
- [**#32**](https://github.com/chocolatey/ChocolateyGUI/issues/32) chocolatey allows you to do a force update
- [**#9**](https://github.com/chocolatey/ChocolateyGUI/issues/9) Add better search via the odata feed
- [**#8**](https://github.com/chocolatey/ChocolateyGUI/issues/8) Add paging to the available packages list

## Improvements

- [**#200**](https://github.com/chocolatey/ChocolateyGUI/issues/200) Correct Line Endings Configuration for repository
- [**#199**](https://github.com/chocolatey/ChocolateyGUI/issues/199) Add a title in the taskbar
- [**#196**](https://github.com/chocolatey/ChocolateyGUI/issues/196) Fix AppVeyor Build
- [**#195**](https://github.com/chocolatey/ChocolateyGUI/issues/195) Add AppVeyor notifications
- [**#191**](https://github.com/chocolatey/ChocolateyGUI/pull/191) Checkbox replaced with tick/cross chars
- [**#180**](https://github.com/chocolatey/ChocolateyGUI/issues/180) Adding Build Caching in AppVeyor
- [**#172**](https://github.com/chocolatey/ChocolateyGUI/issues/172) Update the Build and Testing Process
- [**#168**](https://github.com/chocolatey/ChocolateyGUI/issues/168) Fix Code Analysis Issues
- [**#160**](https://github.com/chocolatey/ChocolateyGUI/issues/160) Setup ReSharper Duplicate Checker
- [**#159**](https://github.com/chocolatey/ChocolateyGUI/issues/159) Setup ReSharper Code Inspections
- [**#158**](https://github.com/chocolatey/ChocolateyGUI/issues/158) Setup Code Analysis
- [**#157**](https://github.com/chocolatey/ChocolateyGUI/issues/157) Setup StyleCop
- [**#152**](https://github.com/chocolatey/ChocolateyGUI/issues/152) Add Deployment Task to Chocolatey
- [**#151**](https://github.com/chocolatey/ChocolateyGUI/issues/151) Add Deployment Task to MyGet
- [**#149**](https://github.com/chocolatey/ChocolateyGUI/issues/149) Inspect AppVeyor Environment Variables to decide what action should be taken
- [**#148**](https://github.com/chocolatey/ChocolateyGUI/issues/148) Add Solution Folders for BuildScripts and appveyor.yml file
- [**#146**](https://github.com/chocolatey/ChocolateyGUI/issues/146) Only run GitVersion UpdateAssemblyInfo if on AppVeyor
- [**#144**](https://github.com/chocolatey/ChocolateyGUI/issues/144) Setup Build Caching in AppVeyor to speed up build
- [**#142**](https://github.com/chocolatey/ChocolateyGUI/issues/142) Add appveyor.yml to control build process
- [**#140**](https://github.com/chocolatey/ChocolateyGUI/issues/140) Add GitVersion to source code and execute from psake
- [**#135**](https://github.com/chocolatey/ChocolateyGUI/issues/135) Update MahApps.Metro version.
- [**#98**](https://github.com/chocolatey/ChocolateyGUI/issues/98) Implement Logging
- [**#89**](https://github.com/chocolatey/ChocolateyGUI/issues/89) Create MyGet Build Service for the wpf-refresh branch

## Bugs

- [**#204**](https://github.com/chocolatey/ChocolateyGUI/issues/204) Unable to add source
- [**#201**](https://github.com/chocolatey/ChocolateyGUI/issues/201) Uninstall script doesn't wait for the uninstaller to finish
- [**#198**](https://github.com/chocolatey/ChocolateyGUI/issues/198) Installation broken in latest release
- [**#193**](https://github.com/chocolatey/ChocolateyGUI/issues/193) Not all installed packages get listed
- [**#174**](https://github.com/chocolatey/ChocolateyGUI/issues/174) Fix Broken Markdown.cs
- [**#127**](https://github.com/chocolatey/ChocolateyGUI/issues/127) Potential issue with "INSTALLED" checkbox
- [**#126**](https://github.com/chocolatey/ChocolateyGUI/issues/126) Unable to run latest Chocolatey GUI package from MyGet
- [**#123**](https://github.com/chocolatey/ChocolateyGUI/issues/123) When not running Chocolatey GUI, each package update present UAC prompt
- [**#120**](https://github.com/chocolatey/ChocolateyGUI/issues/120) Index was outside the bounds of the array
- [**#117**](https://github.com/chocolatey/ChocolateyGUI/issues/117) [BUG] Packages appear twice in Local Tab
- [**#116**](https://github.com/chocolatey/ChocolateyGUI/issues/116) [BUG] Delete button can't always be seen
- [**#105**](https://github.com/chocolatey/ChocolateyGUI/issues/105) Details Page: Items not quite lined up
- [**#104**](https://github.com/chocolatey/ChocolateyGUI/issues/104) Problem with searching when on second page of results
- [**#100**](https://github.com/chocolatey/ChocolateyGUI/issues/100) Remove packages from Local package list when they're uninstalled
- [**#97**](https://github.com/chocolatey/ChocolateyGUI/issues/97) Fix cuninst of Chocolatey GUI
- [**#85**](https://github.com/chocolatey/ChocolateyGUI/issues/85) Unable to get packages and package information from behind proxy.
- [**#70**](https://github.com/chocolatey/ChocolateyGUI/issues/70) Problem adding new package source
- [**#67**](https://github.com/chocolatey/ChocolateyGUI/issues/67) Unhandled Exception

**NOTE**:  This blog post was created after the actual release occured, and is added to here in order to preserve the history of the Chocolatey GUI project.