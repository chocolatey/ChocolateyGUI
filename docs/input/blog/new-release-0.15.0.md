---
Title: Chocolatey GUI 0.15.0
Category: Release
Author: gep13
Published: 12/10/2017
---

Version 0.15.0 of Chocolatey GUI has been released.

Contributions were included from:

- [RichiCoder1](https://github.com/RichiCoder1)
- [pascalberger](https://github.com/pascalberger)
- [gep13](https://github.com/gep13)
- [mwallner](https://github.com/mwallner)
- AdmiringWorm
- [magol](https://github.com/magol)
- [cniweb](https://github.com/cniweb)
- [ferventcoder](https://github.com/ferventcoder)
- Magnus Österlund
- [punker76](https://github.com/punker76)

Full details of everything that was included in this release can be seen below.

<!--excerpt-->

### Issues

As part of this release we had [115 issues](https://github.com/chocolatey/ChocolateyGUI/milestone/6?closed=1) closed.

__Feature__

- [__#473__](https://github.com/chocolatey/ChocolateyGUI/issues/473) Self Service within ChocolateyGUI.
- [__#451__](https://github.com/chocolatey/ChocolateyGUI/pull/451) (GH-371) Enable self-service mode.
- [__#382__](https://github.com/chocolatey/ChocolateyGUI/pull/382) Add check for background service.
- [__#371__](https://github.com/chocolatey/ChocolateyGUI/issues/371) Add check for useBackgroundService .
- [__#295__](https://github.com/chocolatey/ChocolateyGUI/issues/295) Missing Code Signature.
- [__#275__](https://github.com/chocolatey/ChocolateyGUI/issues/275) Chocolatey GUI doesn't save user's preferred window position, size and state.
- [__#246__](https://github.com/chocolatey/ChocolateyGUI/issues/246) Allow config editing.
- [__#119__](https://github.com/chocolatey/ChocolateyGUI/issues/119) Support file system sources.
- [__#115__](https://github.com/chocolatey/ChocolateyGUI/issues/115) Provide ability to update Chocolatey.
- [__#62__](https://github.com/chocolatey/ChocolateyGUI/issues/62) Add posibility to have personal sources.
- [__#61__](https://github.com/chocolatey/ChocolateyGUI/issues/61) show list of installed packages for that packagename.

__Improvement__

- [__#484__](https://github.com/chocolatey/ChocolateyGUI/pull/484) Match Gallery Styles.
- [__#482__](https://github.com/chocolatey/ChocolateyGUI/pull/482) (GH-452) Support SxS Packages.
- [__#471__](https://github.com/chocolatey/ChocolateyGUI/issues/471) Fixes in German translation.
- [__#465__](https://github.com/chocolatey/ChocolateyGUI/issues/465) Use highestAvailable instead of subprocess.
- [__#464__](https://github.com/chocolatey/ChocolateyGUI/issues/464) Provide ability to set Admin Only flag for source.
- [__#453__](https://github.com/chocolatey/ChocolateyGUI/issues/453) Please run processes on another thread.
- [__#439__](https://github.com/chocolatey/ChocolateyGUI/issues/439) Package refresh should be automatic and hidden.
- [__#425__](https://github.com/chocolatey/ChocolateyGUI/issues/425) Add additional properties to Sources Tab in Settings screen.
- [__#414__](https://github.com/chocolatey/ChocolateyGUI/issues/414) update to chocolatey 0.10.4.
- [__#404__](https://github.com/chocolatey/ChocolateyGUI/issues/404) Globalization - Norwegian Bokmål.
- [__#399__](https://github.com/chocolatey/ChocolateyGUI/issues/399) The "title" of the UI is often wrong or non-sensical.
- [__#359__](https://github.com/chocolatey/ChocolateyGUI/issues/359) Order sources by priority and state.
- [__#355__](https://github.com/chocolatey/ChocolateyGUI/issues/355) Link not parsed in Release Notes.
- [__#330__](https://github.com/chocolatey/ChocolateyGUI/issues/330) Update all button hard to find.
- [__#329__](https://github.com/chocolatey/ChocolateyGUI/issues/329) Can't control main window while actions are in progress.
- [__#323__](https://github.com/chocolatey/ChocolateyGUI/issues/323) Update dependencies in Chocolatey GUI Wix Script.
- [__#308__](https://github.com/chocolatey/ChocolateyGUI/pull/308) (GH-288) Filter packages list after refreshing or updating all packages.
- [__#298__](https://github.com/chocolatey/ChocolateyGUI/pull/298) (GH-288) Improved GUI behavior after "update all".
- [__#292__](https://github.com/chocolatey/ChocolateyGUI/issues/292) Scroll to top of packages list when going to a new page.
- [__#286__](https://github.com/chocolatey/ChocolateyGUI/pull/286) (GH-285) Add Control Margins.
- [__#285__](https://github.com/chocolatey/ChocolateyGUI/issues/285) Add UI padding.
- [__#267__](https://github.com/chocolatey/ChocolateyGUI/issues/267) Intergrate with Chocolatey API.
- [__#265__](https://github.com/chocolatey/ChocolateyGUI/issues/265) Cannot search for packages AND only show those with updates.
- [__#253__](https://github.com/chocolatey/ChocolateyGUI/issues/253) Use the global config sources.
- [__#202__](https://github.com/chocolatey/ChocolateyGUI/issues/202) Support for SVG package icons.

__Documentation__

- [__#478__](https://github.com/chocolatey/ChocolateyGUI/issues/478) German texts in settings page are to long.
- [__#467__](https://github.com/chocolatey/ChocolateyGUI/pull/467) Update swedish localization.
- [__#444__](https://github.com/chocolatey/ChocolateyGUI/pull/444) (GH-443) Fix inconsistencies in German translation.
- [__#443__](https://github.com/chocolatey/ChocolateyGUI/issues/443) Fix inconsistencies in German translation.
- [__#427__](https://github.com/chocolatey/ChocolateyGUI/issues/427) More explicitly call out OS support.
- [__#413__](https://github.com/chocolatey/ChocolateyGUI/issues/413) Add/Update missing German translation.
- [__#412__](https://github.com/chocolatey/ChocolateyGUI/pull/412) (GH-396) Added Swedish translation.
- [__#410__](https://github.com/chocolatey/ChocolateyGUI/pull/410) Localization/missing translations.
- [__#408__](https://github.com/chocolatey/ChocolateyGUI/pull/408) (GH-404) Added Norwegian Bokmål translation.
- [__#406__](https://github.com/chocolatey/ChocolateyGUI/pull/406) (GH-405) Moved hard coded text strings to resource file.
- [__#403__](https://github.com/chocolatey/ChocolateyGUI/pull/403) Fix typo.
- [__#402__](https://github.com/chocolatey/ChocolateyGUI/issues/402) ChocolateyGUI, Chocolatey GUI, Chocolatey Gui, or something else.
- [__#397__](https://github.com/chocolatey/ChocolateyGUI/issues/397) Chocolatey GUI Globalisation HowTo.
- [__#394__](https://github.com/chocolatey/ChocolateyGUI/issues/394) Added Gitter and GitHub links.
- [__#373__](https://github.com/chocolatey/ChocolateyGUI/issues/373) Generate Documentation/Blog using Wyam.
- [__#315__](https://github.com/chocolatey/ChocolateyGUI/issues/315) Update Screenshot in README.
- [__#314__](https://github.com/chocolatey/ChocolateyGUI/issues/314) Update Credits.
- [__#311__](https://github.com/chocolatey/ChocolateyGUI/pull/311) Update chocolatey's icon.
- [__#280__](https://github.com/chocolatey/ChocolateyGUI/issues/280) About > Making a chocolatey package for Chocolatey GUI > 404 not found.

__Bug__

- [__#480__](https://github.com/chocolatey/ChocolateyGUI/issues/480) Incorrect option in Package Details view.
- [__#470__](https://github.com/chocolatey/ChocolateyGUI/issues/470) Seemingly Random Chocolatey GUI Crash.
- [__#469__](https://github.com/chocolatey/ChocolateyGUI/issues/469) Issue Launching with Chocolatey Licensed.
- [__#463__](https://github.com/chocolatey/ChocolateyGUI/issues/463) List of packages needing updates is truncated randomly.
- [__#462__](https://github.com/chocolatey/ChocolateyGUI/issues/462) Updating a package doesn't honour "Show Only Packages with Updates" when list is refreshed.
- [__#460__](https://github.com/chocolatey/ChocolateyGUI/issues/460) Access Denied on Launch.
- [__#458__](https://github.com/chocolatey/ChocolateyGUI/issues/458) ChocoGUI Crash on Startup -- Windows 10.
- [__#455__](https://github.com/chocolatey/ChocolateyGUI/issues/455) Source Configuration - Seeing and Setting Bypass Proxy and Allow Self-Service on a Source doesn't work.
- [__#452__](https://github.com/chocolatey/ChocolateyGUI/issues/452) Support displaying multiple versions of locally installed packages.
- [__#450__](https://github.com/chocolatey/ChocolateyGUI/issues/450) Chocolatey GUI crashes when started from another working-directory.
- [__#449__](https://github.com/chocolatey/ChocolateyGUI/issues/449) Chocolatey GUI crashes when closed while still loading packages.
- [__#448__](https://github.com/chocolatey/ChocolateyGUI/issues/448) Random Chocolatey GUI crash.
- [__#447__](https://github.com/chocolatey/ChocolateyGUI/issues/447) FileLoadException on launch.
- [__#446__](https://github.com/chocolatey/ChocolateyGUI/pull/446) (GH-445) Fix outdated check.
- [__#445__](https://github.com/chocolatey/ChocolateyGUI/issues/445) Chocolatey GUI doesn't display VersionInfo from internal feeds.
- [__#440__](https://github.com/chocolatey/ChocolateyGUI/issues/440) NullRefEx on launch.
- [__#436__](https://github.com/chocolatey/ChocolateyGUI/issues/436) Remove the Hacks where you access Chocolatey's SimpleInjector.Container.
- [__#434__](https://github.com/chocolatey/ChocolateyGUI/issues/434) Remove AlphaFS dependency.
- [__#424__](https://github.com/chocolatey/ChocolateyGUI/issues/424) Selecting multiple packages with CTRL key to update results in only the first package being updated.
- [__#421__](https://github.com/chocolatey/ChocolateyGUI/issues/421) After selecting single package to update upon update completion the UI list of packages remains blank does not refresh.
- [__#411__](https://github.com/chocolatey/ChocolateyGUI/issues/411) Pressing setting multiple times make it impossible to go back.
- [__#405__](https://github.com/chocolatey/ChocolateyGUI/issues/405) Text/Header strings not prepared to be translated.
- [__#380__](https://github.com/chocolatey/ChocolateyGUI/issues/380) Dates are displayed in US format not in machine locale format.
- [__#372__](https://github.com/chocolatey/ChocolateyGUI/issues/372) Chocolatey GUI errors on subprocess when attempting to open more than one instance of Chocolatey GUI.
- [__#370__](https://github.com/chocolatey/ChocolateyGUI/issues/370) Duplicated entries.
- [__#368__](https://github.com/chocolatey/ChocolateyGUI/issues/368) Win7 Powershell5 update.
- [__#360__](https://github.com/chocolatey/ChocolateyGUI/issues/360) Error starting v0.14.0-unstable0147.
- [__#358__](https://github.com/chocolatey/ChocolateyGUI/issues/358) Option for hiding disabled package sources.
- [__#357__](https://github.com/chocolatey/ChocolateyGUI/issues/357) NullReferenceException opening settings while packages are loading.
- [__#353__](https://github.com/chocolatey/ChocolateyGUI/issues/353) Errors on console updating package.
- [__#352__](https://github.com/chocolatey/ChocolateyGUI/issues/352) NullReferenceException on closing settings.
- [__#333__](https://github.com/chocolatey/ChocolateyGUI/issues/333) Chocoatey-View (not This PC): sorting list by downloads causes crash.
- [__#332__](https://github.com/chocolatey/ChocolateyGUI/issues/332) Win32Exception upon clicking the "Install GoogleChrome instead" link in "Google Chrome (64-bit only)" detail page.
- [__#331__](https://github.com/chocolatey/ChocolateyGUI/issues/331) missing packages in Chocolatey GUI.
- [__#327__](https://github.com/chocolatey/ChocolateyGUI/issues/327) System.ArgumentException: An item with the same key has already been added.
- [__#319__](https://github.com/chocolatey/ChocolateyGUI/issues/319) Crash using 'Update all'.
- [__#309__](https://github.com/chocolatey/ChocolateyGUI/issues/309) Looses filter on refresh.
- [__#307__](https://github.com/chocolatey/ChocolateyGUI/issues/307) Installed Packages Not Properly Handled.
- [__#301__](https://github.com/chocolatey/ChocolateyGUI/issues/301) Erroneous date format.
- [__#297__](https://github.com/chocolatey/ChocolateyGUI/issues/297) Runtime error.
- [__#288__](https://github.com/chocolatey/ChocolateyGUI/issues/288) Shows all packages although "Show Only Packages with Updates" is ticked after "update all".
- [__#266__](https://github.com/chocolatey/ChocolateyGUI/issues/266) Chocolatey GUI stuck on "Retrieving installed packages".
- [__#264__](https://github.com/chocolatey/ChocolateyGUI/issues/264) Refreshing package list ignores search query/options.
- [__#263__](https://github.com/chocolatey/ChocolateyGUI/issues/263) Settings flyout showing twice.
- [__#256__](https://github.com/chocolatey/ChocolateyGUI/issues/256) PSSecurityException.
- [__#225__](https://github.com/chocolatey/ChocolateyGUI/issues/225) Error messages.
- [__#124__](https://github.com/chocolatey/ChocolateyGUI/issues/124) Problem with opening Sources setting page.

__Internal Refactoring__

- [__#393__](https://github.com/chocolatey/ChocolateyGUI/issues/393) Remove hard coded path to ruleset file.
- [__#392__](https://github.com/chocolatey/ChocolateyGUI/issues/392) Prevent execution of GitLink.
- [__#391__](https://github.com/chocolatey/ChocolateyGUI/issues/391) Correct AppVeyor Skip Regular Expression.
- [__#390__](https://github.com/chocolatey/ChocolateyGUI/issues/390) Make tools folder invalidation dependent on setup.cake.
- [__#389__](https://github.com/chocolatey/ChocolateyGUI/issues/389) Remove Gitter Configuration from yml file.
- [__#388__](https://github.com/chocolatey/ChocolateyGUI/issues/388) Change name of appveyor file.
- [__#387__](https://github.com/chocolatey/ChocolateyGUI/issues/387) Remove unnecessary Wyam variables.
- [__#82__](https://github.com/chocolatey/ChocolateyGUI/issues/82) Update Build Script to include running of Unit Tests.

__:Infastructure__

- [__#349__](https://github.com/chocolatey/ChocolateyGUI/issues/349) Prepare 0.15 Release.

__Build__

- [__#341__](https://github.com/chocolatey/ChocolateyGUI/issues/341) Switch Build Infrastructure to Cake.
- [__#317__](https://github.com/chocolatey/ChocolateyGUI/issues/317) 0.9.10 and exit codes.
- [__#310__](https://github.com/chocolatey/ChocolateyGUI/pull/310) Update and remove unused dependencies.
- [__#209__](https://github.com/chocolatey/ChocolateyGUI/issues/209) Review MSI package.

### Where to get it
You can download this release from [chocolatey](https://chocolatey.org/packages/chocolateyGUI/0.15.0)

**NOTE**:  This blog post was created after the actual release occured, and is added here in order to preserve the history of the Chocolatey GUI project.