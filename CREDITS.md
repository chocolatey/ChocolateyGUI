<img alt="Chocolatey logo" width="260" style="margin-right: 1rem;" src="https://img.chocolatey.org/logos/chocolatey.png"> <img alt="Chocolatey icon logo" width="200" src="https://img.chocolatey.org/logos/chocolatey-icon.png">

# Chocolatey GUI Third Party Licenses

---

Chocolatey uses a number of 3rd party components. Their details are below.

* [Open Source License Types (Reference)](#open-source-license-types-reference)
  * [Apache v2.0 License](#apache-v20-license)
  * [BSD-3-Clause](#bsd-3-clause)
  * [MIT License](#mit-license)
  * [BSD-2-Clause License](#bsd-2-clause-license)
  * [GNU Lesser General Public License (LGPL) v3.0](#gnu-lesser-general-public-license-\(lgpl\)-v30)
  * [Microsoft Public License](#microsoft-public-license)
  * [Microsoft Software License](#microsoft-software-license)
* [Chocolatey Software Component License](#chocolatey-software-component-licenses)
  * [Chocolatey Open Source](#chocolatey-open-source)
* [Chocolatey CLI / Chocolatey.Lib](#chocolatey-cli--chocolateylib)
  * [Apache v2.0 License](#apache-v20-license-1)
    * [Checksum@0.2.0](#checksum020)
    * [Chocolatey.NuGet.Client@3.1.0](#chocolateynugetclient310)
    * [log4net@rel/2.0.12](#log4netrel2012)
    * [Microsoft.Web.Xdt@2.1.1](#microsoftwebxdt211)
  * [BSD-3-Clause](#bsd-3-clause-1)
    * [Rhino.Licensing@1.4.1](#rhinolicensing141)
  * [MIT License](#mit-license-1)
    * [AlphaFS@2.1.3](#alphafs213)
    * [Microsoft.Bcl.HashCode@1.1.1](#microsoftbclhashcode111)
    * [Newtonsoft.Json@13.0.1](#newtonsoftjson1301)
    * [SimpleInjector@2.8.3](#simpleinjector283)
    * [System.Reactive@rxnet-v5.0.0](#systemreactiverxnet-v500)
    * [System.Runtime.CompilerServices.Unsafe@4.5.3](#systemruntimecompilerservicesunsafe453)
    * [System.Threading.Tasks.Extensions@4.5.4](#systemthreadingtasksextensions454)
  * [Other](#other)
    * [7-Zip@21.07](#7-zip2107)
    * [Shim Generator (shimgen)@1.0.0](#shim-generator-\(shimgen\)100)
* [Chocolatey GUI](#chocolatey-gui)
  * [Apache v2.0 License](#apache-v20-license-2)
    * [Chocolatey CLI / Chocolatey.Lib@2.0.0](#chocolatey-cli--chocolateylib200)
    * [Serilog@2.5.0](#serilog250)
    * [Serilog.Formatting.Compact@1.0.0](#serilogformattingcompact100)
    * [Serilog.Sinks.Async@1.1.0](#serilogsinksasync110)
    * [Serilog.Sinks.Console@3.1.0](#serilogsinksconsole310)
    * [Serilog.Sinks.File@3.2.0](#serilogsinksfile320)
    * [Serilog.Sinks.PeriodicBatching@2.1.1](#serilogsinksperiodicbatching211)
    * [Serilog.Sinks.RollingFile@3.3.0](#serilogsinksrollingfile330)
    * [System.Reactive.Core@3.1.1](#systemreactivecore311)
    * [System.Reactive.Interfaces@3.1.1](#systemreactiveinterfaces311)
    * [System.Reactive.Linq@3.1.1](#systemreactivelinq311)
    * [System.Reactive.PlatformServices@3.1.1](#systemreactiveplatformservices311)
    * [System.Reactive.Windows.Threading@3.1.1](#systemreactivewindowsthreading311)
  * [BSD-2-Clause License](#bsd-2-clause-license-1)
    * [Markdig.Signed@0.23.0](#markdigsigned0230)
  * [GNU Lesser General Public License (LGPL) v3.0](#gnu-lesser-general-public-license-\(lgpl\)-v30-1)
    * [Fizzler@1.2.0](#fizzler120)
  * [Microsoft Public License](#microsoft-public-license-1)
    * [Svg.Custom@release/0.3.0](#svgcustomrelease030)
  * [Microsoft Software License](#microsoft-software-license-1)
    * [System.Runtime.InteropServices.RuntimeInformation@4.3.0](#systemruntimeinteropservicesruntimeinformation430)
  * [MIT License](#mit-license-2)
    * [Autofac@4.6.1](#autofac461)
    * [AutoMapper@7.0.1](#automapper701)
    * [Caliburn.Micro@3.2.0](#caliburnmicro320)
    * [Caliburn.Micro.Core@3.2.0](#caliburnmicrocore320)
    * [ControlzEx@4.4.0](#controlzex440)
    * [HarfBuzzSharp@2.6.1.4](#harfbuzzsharp2614)
    * [LiteDB@5.0.15](#litedb5015)
    * [MahApps.Metro@2.4.4](#mahappsmetro244)
    * [MahApps.Metro.IconPacks.BoxIcons@4.8.0](#mahappsmetroiconpacksboxicons480)
    * [MahApps.Metro.IconPacks.Entypo@4.8.0](#mahappsmetroiconpacksentypo480)
    * [MahApps.Metro.IconPacks.FontAwesome@4.8.0](#mahappsmetroiconpacksfontawesome480)
    * [MahApps.Metro.IconPacks.Modern@4.8.0](#mahappsmetroiconpacksmodern480)
    * [MahApps.Metro.IconPacks.Octicons@4.8.0](#mahappsmetroiconpacksocticons480)
    * [MahApps.Metro.SimpleChildWindow@2.0.0](#mahappsmetrosimplechildwindow200)
    * [Markdig.Wpf.Signed@0.5.0.1](#markdigwpfsigned0501)
    * [Microsoft.VisualStudio.Threading@15.4.4](#microsoftvisualstudiothreading1544)
    * [Microsoft.VisualStudio.Validation@15.3.32](#microsoftvisualstudiovalidation15332)
    * [Microsoft.Xaml.Behaviors.Wpf@1.1.19](#microsoftxamlbehaviorswpf1119)
    * [SkiaSharp@1.68.3](#skiasharp1683)
    * [SkiaSharp.HarfBuzz@1.68.3](#skiasharpharfbuzz1683)
    * [Splat@2.0.0](#splat200)
    * [Svg.Skia@0.3.0](#svgskia030)
    * [System.Buffers@4.5.1](#systembuffers451)
    * [System.Memory@4.5.4](#systemmemory454)
    * [System.Numerics.Vectors@4.5.0](#systemnumericsvectors450)
    * [System.Runtime.CompilerServices.Unsafe@4.7.1](#systemruntimecompilerservicesunsafe471)
    * [System.Threading.Tasks.Extensions@4.4.0](#systemthreadingtasksextensions440)
    * [System.ValueTuple@4.5.0](#systemvaluetuple450)

## Open Source License Types (Reference)

There are some regularly used open source license types - to reduce the sheer size of this document, we will provide a reference to them here. Each particular component will link directly to the actual license or notice file.

### Apache v2.0 License

The [Apache v2.0 License](https://www.apache.org/licenses/LICENSE-2.0) has the following terms:

```text
                              Apache License
                        Version 2.0, January 2004
                     http://www.apache.org/licenses/

TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION

1. Definitions.

  "License" shall mean the terms and conditions for use, reproduction,
  and distribution as defined by Sections 1 through 9 of this document.

  "Licensor" shall mean the copyright owner or entity authorized by
  the copyright owner that is granting the License.

  "Legal Entity" shall mean the union of the acting entity and all
  other entities that control, are controlled by, or are under common
  control with that entity. For the purposes of this definition,
  "control" means (i) the power, direct or indirect, to cause the
  direction or management of such entity, whether by contract or
  otherwise, or (ii) ownership of fifty percent (50%) or more of the
  outstanding shares, or (iii) beneficial ownership of such entity.

  "You" (or "Your") shall mean an individual or Legal Entity
  exercising permissions granted by this License.

  "Source" form shall mean the preferred form for making modifications,
  including but not limited to software source code, documentation
  source, and configuration files.

  "Object" form shall mean any form resulting from mechanical
  transformation or translation of a Source form, including but
  not limited to compiled object code, generated documentation,
  and conversions to other media types.

  "Work" shall mean the work of authorship, whether in Source or
  Object form, made available under the License, as indicated by a
  copyright notice that is included in or attached to the work
  (an example is provided in the Appendix below).

  "Derivative Works" shall mean any work, whether in Source or Object
  form, that is based on (or derived from) the Work and for which the
  editorial revisions, annotations, elaborations, or other modifications
  represent, as a whole, an original work of authorship. For the purposes
  of this License, Derivative Works shall not include works that remain
  separable from, or merely link (or bind by name) to the interfaces of,
  the Work and Derivative Works thereof.

  "Contribution" shall mean any work of authorship, including
  the original version of the Work and any modifications or additions
  to that Work or Derivative Works thereof, that is intentionally
  submitted to Licensor for inclusion in the Work by the copyright owner
  or by an individual or Legal Entity authorized to submit on behalf of
  the copyright owner. For the purposes of this definition, "submitted"
  means any form of electronic, verbal, or written communication sent
  to the Licensor or its representatives, including but not limited to
  communication on electronic mailing lists, source code control systems,
  and issue tracking systems that are managed by, or on behalf of, the
  Licensor for the purpose of discussing and improving the Work, but
  excluding communication that is conspicuously marked or otherwise
  designated in writing by the copyright owner as "Not a Contribution."

  "Contributor" shall mean Licensor and any individual or Legal Entity
  on behalf of whom a Contribution has been received by Licensor and
  subsequently incorporated within the Work.

2. Grant of Copyright License. Subject to the terms and conditions of
  this License, each Contributor hereby grants to You a perpetual,
  worldwide, non-exclusive, no-charge, royalty-free, irrevocable
  copyright license to reproduce, prepare Derivative Works of,
  publicly display, publicly perform, sublicense, and distribute the
  Work and such Derivative Works in Source or Object form.

3. Grant of Patent License. Subject to the terms and conditions of
  this License, each Contributor hereby grants to You a perpetual,
  worldwide, non-exclusive, no-charge, royalty-free, irrevocable
  (except as stated in this section) patent license to make, have made,
  use, offer to sell, sell, import, and otherwise transfer the Work,
  where such license applies only to those patent claims licensable
  by such Contributor that are necessarily infringed by their
  Contribution(s) alone or by combination of their Contribution(s)
  with the Work to which such Contribution(s) was submitted. If You
  institute patent litigation against any entity (including a
  cross-claim or counterclaim in a lawsuit) alleging that the Work
  or a Contribution incorporated within the Work constitutes direct
  or contributory patent infringement, then any patent licenses
  granted to You under this License for that Work shall terminate
  as of the date such litigation is filed.

4. Redistribution. You may reproduce and distribute copies of the
  Work or Derivative Works thereof in any medium, with or without
  modifications, and in Source or Object form, provided that You
  meet the following conditions:

  (a) You must give any other recipients of the Work or
      Derivative Works a copy of this License; and

  (b) You must cause any modified files to carry prominent notices
      stating that You changed the files; and

  (c) You must retain, in the Source form of any Derivative Works
      that You distribute, all copyright, patent, trademark, and
      attribution notices from the Source form of the Work,
      excluding those notices that do not pertain to any part of
      the Derivative Works; and

  (d) If the Work includes a "NOTICE" text file as part of its
      distribution, then any Derivative Works that You distribute must
      include a readable copy of the attribution notices contained
      within such NOTICE file, excluding those notices that do not
      pertain to any part of the Derivative Works, in at least one
      of the following places: within a NOTICE text file distributed
      as part of the Derivative Works; within the Source form or
      documentation, if provided along with the Derivative Works; or,
      within a display generated by the Derivative Works, if and
      wherever such third-party notices normally appear. The contents
      of the NOTICE file are for informational purposes only and
      do not modify the License. You may add Your own attribution
      notices within Derivative Works that You distribute, alongside
      or as an addendum to the NOTICE text from the Work, provided
      that such additional attribution notices cannot be construed
      as modifying the License.

  You may add Your own copyright statement to Your modifications and
  may provide additional or different license terms and conditions
  for use, reproduction, or distribution of Your modifications, or
  for any such Derivative Works as a whole, provided Your use,
  reproduction, and distribution of the Work otherwise complies with
  the conditions stated in this License.

5. Submission of Contributions. Unless You explicitly state otherwise,
  any Contribution intentionally submitted for inclusion in the Work
  by You to the Licensor shall be under the terms and conditions of
  this License, without any additional terms or conditions.
  Notwithstanding the above, nothing herein shall supersede or modify
  the terms of any separate license agreement you may have executed
  with Licensor regarding such Contributions.

6. Trademarks. This License does not grant permission to use the trade
  names, trademarks, service marks, or product names of the Licensor,
  except as required for reasonable and customary use in describing the
  origin of the Work and reproducing the content of the NOTICE file.

7. Disclaimer of Warranty. Unless required by applicable law or
  agreed to in writing, Licensor provides the Work (and each
  Contributor provides its Contributions) on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
  implied, including, without limitation, any warranties or conditions
  of TITLE, NON-INFRINGEMENT, MERCHANTABILITY, or FITNESS FOR A
  PARTICULAR PURPOSE. You are solely responsible for determining the
  appropriateness of using or redistributing the Work and assume any
  risks associated with Your exercise of permissions under this License.

8. Limitation of Liability. In no event and under no legal theory,
  whether in tort (including negligence), contract, or otherwise,
  unless required by applicable law (such as deliberate and grossly
  negligent acts) or agreed to in writing, shall any Contributor be
  liable to You for damages, including any direct, indirect, special,
  incidental, or consequential damages of any character arising as a
  result of this License or out of the use or inability to use the
  Work (including but not limited to damages for loss of goodwill,
  work stoppage, computer failure or malfunction, or any and all
  other commercial damages or losses), even if such Contributor
  has been advised of the possibility of such damages.

9. Accepting Warranty or Additional Liability. While redistributing
  the Work or Derivative Works thereof, You may choose to offer,
  and charge a fee for, acceptance of support, warranty, indemnity,
  or other liability obligations and/or rights consistent with this
  License. However, in accepting such obligations, You may act only
  on Your own behalf and on Your sole responsibility, not on behalf
  of any other Contributor, and only if You agree to indemnify,
  defend, and hold each Contributor harmless for any liability
  incurred by, or claims asserted against, such Contributor by reason
  of your accepting any such warranty or additional liability.

END OF TERMS AND CONDITIONS
```

### BSD-3-Clause

The [BSD 3-Clause License](https://opensource.org/license/bsd-3-clause) has also been called the "New BSD License", "Revised BSD License", or "Modified BSD License." It has the following terms:

```text
Copyright <YEAR> <COPYRIGHT HOLDER>

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice,
   this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS “AS IS”
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
POSSIBILITY OF SUCH DAMAGE.
```

### MIT License

The [MIT License](https://mit-license.org/) has the following terms:

```text
Copyright © <YEAR> <COPYRIGHT HOLDER>

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
“Software”), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject
to the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

### BSD-2-Clause License

The [BSD 2-Clause License](https://opensource.org/license/bsd-2-clause) has also been called the "Simplified BSD License" and the "FreeBSD License." It has the following terms:

```text
Copyright <YEAR> <COPYRIGHT HOLDER>

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice,
   this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS “AS IS”
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
POSSIBILITY OF SUCH DAMAGE.
```

### GNU Lesser General Public License (LGPL) v3.0

The [GNU Lesser General Public License](https://www.gnu.org/licenses/lgpl-3.0.html) has the following terms:

```txt
### GNU LESSER GENERAL PUBLIC LICENSE

Version 3, 29 June 2007

Copyright © 2007 Free Software Foundation, Inc. <[https://fsf.org/](https://fsf.org/)\>

Everyone is permitted to copy and distribute verbatim copies of this license document, but changing it is not allowed.

This version of the GNU Lesser General Public License incorporates the terms and conditions of version 3 of the GNU General Public License, supplemented by the additional permissions listed below.

0. **Additional Definitions.**

As used herein, “this License” refers to version 3 of the GNU Lesser General Public License, and the “GNU GPL” refers to version 3 of the GNU General Public License.

“The Library” refers to a covered work governed by this License, other than an Application or a Combined Work as defined below.

An “Application” is any work that makes use of an interface provided by the Library, but which is not otherwise based on the Library. Defining a subclass of a class defined by the Library is deemed a mode of using an interface provided by the Library.

A “Combined Work” is a work produced by combining or linking an Application with the Library. The particular version of the Library with which the Combined Work was made is also called the “Linked Version”.

The “Minimal Corresponding Source” for a Combined Work means the Corresponding Source for the Combined Work, excluding any source code for portions of the Combined Work that, considered in isolation, are based on the Application, and not on the Linked Version.

The “Corresponding Application Code” for a Combined Work means the object code and/or source code for the Application, including any data and utility programs needed for reproducing the Combined Work from the Application, but excluding the System Libraries of the Combined Work.

1. **Exception to Section 3 of the GNU GPL.**

You may convey a covered work under sections 3 and 4 of this License without being bound by section 3 of the GNU GPL.

2. **Conveying Modified Versions.**

If you modify a copy of the Library, and, in your modifications, a facility refers to a function or data to be supplied by an Application that uses the facility (other than as an argument passed when the facility is invoked), then you may convey a copy of the modified version:

* a) under this License, provided that you make a good faith effort to ensure that, in the event an Application does not supply the function or data, the facility still operates, and performs whatever part of its purpose remains meaningful, or
* b) under the GNU GPL, with none of the additional permissions of this License applicable to that copy.

3. **Object Code Incorporating Material from Library Header Files.**

The object code form of an Application may incorporate material from a header file that is part of the Library. You may convey such object code under terms of your choice, provided that, if the incorporated material is not limited to numerical parameters, data structure layouts and accessors, or small macros, inline functions and templates (ten or fewer lines in length), you do both of the following:

* a) Give prominent notice with each copy of the object code that the Library is used in it and that the Library and its use are covered by this License.
* b) Accompany the object code with a copy of the GNU GPL and this license document.

4. **Combined Works.**

You may convey a Combined Work under terms of your choice that, taken together, effectively do not restrict modification of the portions of the Library contained in the Combined Work and reverse engineering for debugging such modifications, if you also do each of the following:

* a) Give prominent notice with each copy of the Combined Work that the Library is used in it and that the Library and its use are covered by this License.
  
* b) Accompany the Combined Work with a copy of the GNU GPL and this license document.
  
* c) For a Combined Work that displays copyright notices during execution, include the copyright notice for the Library among these notices, as well as a reference directing the user to the copies of the GNU GPL and this license document.
  
* d) Do one of the following:
  
  * 0) Convey the Minimal Corresponding Source under the terms of this License, and the Corresponding Application Code in a form suitable for, and under terms that permit, the user to recombine or relink the Application with a modified version of the Linked Version to produce a modified Combined Work, in the manner specified by section 6 of the GNU GPL for conveying Corresponding Source.
  * 1) Use a suitable shared library mechanism for linking with the Library. A suitable mechanism is one that (a) uses at run time a copy of the Library already present on the user's computer system, and (b) will operate properly with a modified version of the Library that is interface-compatible with the Linked Version.
* e) Provide Installation Information, but only if you would otherwise be required to provide such information under section 6 of the GNU GPL, and only to the extent that such information is necessary to install and execute a modified version of the Combined Work produced by recombining or relinking the Application with a modified version of the Linked Version. (If you use option 4d0, the Installation Information must accompany the Minimal Corresponding Source and Corresponding Application Code. If you use option 4d1, you must provide the Installation Information in the manner specified by section 6 of the GNU GPL for conveying Corresponding Source.)
  

5. **Combined Libraries.**

You may place library facilities that are a work based on the Library side by side in a single library together with other library facilities that are not Applications and are not covered by this License, and convey such a combined library under terms of your choice, if you do both of the following:

* a) Accompany the combined library with a copy of the same work based on the Library, uncombined with any other library facilities, conveyed under the terms of this License.
* b) Give prominent notice with the combined library that part of it is a work based on the Library, and explaining where to find the accompanying uncombined form of the same work.

6. **Revised Versions of the GNU Lesser General Public License.**

The Free Software Foundation may publish revised and/or new versions of the GNU Lesser General Public License from time to time. Such new versions will be similar in spirit to the present version, but may differ in detail to address new problems or concerns.

Each version is given a distinguishing version number. If the Library as you received it specifies that a certain numbered version of the GNU Lesser General Public License “or any later version” applies to it, you have the option of following the terms and conditions either of that published version or of any later version published by the Free Software Foundation. If the Library as you received it does not specify a version number of the GNU Lesser General Public License, you may choose any version of the GNU Lesser General Public License ever published by the Free Software Foundation.

If the Library as you received it specifies that a proxy can decide whether future versions of the GNU Lesser General Public License shall apply, that proxy's public statement of acceptance of any version is permanent authorization for you to choose that version for the Library.
```

### Microsoft Public License

The [Microsoft Public License](https://opensource.org/license/ms-pl-html) has the following terms:

```txt
This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.

1. Definitions

The terms “reproduce,” “reproduction,” “derivative works,” and “distribution” have the same meaning here as under U.S. copyright law.

A “contribution” is the original software, or any additions or changes to the software.

A “contributor” is any person that distributes its contribution under this license.

“Licensed patents” are a contributor’s patent claims that read directly on its contribution.

2. Grant of Rights

(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.

(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.

3. Conditions and Limitations

(A) No Trademark License- This license does not grant you rights to use any contributors’ name, logo, or trademarks.

(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.

(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.

(D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.

(E) The software is licensed “as-is.” You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.
```

### Microsoft Software License

The [Microsoft Software License](https://github.com/dotnet/core/blob/4156c6415620a004f294998ada55881ad203da18/license-information.md) has the following terms:

```txt
# License Information

The .NET project uses source and binaries from multiple sources that may be important to your use of .NET.

This document is provided for informative purposes only and is not itself a license.

## Source code

.NET source uses the MIT license.

[Each repo](./Documentation/core-repos.md) has:

* A license, for example, [dotnet/runtime LICENSE.TXT](https://github.com/dotnet/runtime/blob/main/LICENSE.TXT).
* Third party notice file, for example, [dotnet/runtime THIRD-PARTY-NOTICES.TXT](https://github.com/dotnet/runtime/blob/main/THIRD-PARTY-NOTICES.TXT)

More information:

* [Project copyright guidance](https://github.com/dotnet/runtime/blob/main/docs/project/copyright.md)

## Product distributions

Product distributions use the following license:

* On Linux and macOS: [MIT license](https://github.com/dotnet/core/blob/main/LICENSE.TXT)
* On Windows: [.NET Library License](https://dotnet.microsoft.com/dotnet_library_license.htm)

Product distributions include [downloadable assets](https://dotnet.microsoft.com/download/dotnet) and [runtime packs](https://www.nuget.org/packages/Microsoft.NETCore.App.Runtime.win-x64/).

More information:

* [Windows license information](https://github.com/dotnet/core/blob/main/license-information-windows.md).
* [.NET Asset Licensing Model](https://github.com/dotnet/runtime/blob/main/docs/project/licensing-assets.md)

## Package distributions

Library packages use the MIT license, for example [System.Text.Json](https://www.nuget.org/packages/System.Text.Json).

## Redistribution

Binaries produced by .NET SDK compilers (C#, F#, VB) can be redistributed without additional restrictions. The only restrictions are based on the license of the compiler inputs used to produce the binary.

Applications are subject to the same terms as are covered by "Product distributions" and "Package distributions", above.

Parts of the .NET runtime are embedded in applications, including [platform-specific executable hosts](https://learn.microsoft.com/dotnet/core/deploying/deploy-with-cli#framework-dependent-executable), and [self-contained deployments](https://learn.microsoft.com/dotnet/core/deploying/deploy-with-cli#self-contained-deployment).
```

## Chocolatey Software Component Licenses

### Chocolatey Open Source

Chocolatey Open Source components fall under the [Apache v2.0 license](https://www.apache.org/licenses/LICENSE-2.0).

```text
https://www.apache.org/licenses/LICENSE-2.0
```

* [Chocolatey CLI / Chocolatey.Lib](https://github.com/chocolatey/choco) - [License terms.](https://github.com/chocolatey/choco/blob/master/LICENSE)
* [Chocolatey GUI](https://github.com/chocolatey/ChocolateyGUI) - [License terms.](https://github.com/chocolatey/ChocolateyGUI/blob/master/LICENSE.txt)

## Chocolatey CLI / Chocolatey.Lib

### Apache v2.0 License

#### Checksum@0.2.0

[Checksum](https://github.com/chocolatey/checksum) - [License terms.](https://github.com/chocolatey/checksum/blob/e6f5645610c7bc15084b48f69d4cdb056106f956/LICENSE)

#### Chocolatey.NuGet.Client@3.1.0

[Chocolatey.NuGet.Client](https://github.com/NuGet/NuGet.Client) [(modified)](https://github.com/chocolatey/NuGet.Client) - [License terms.](https://github.com/NuGet/NuGet.Client/blob/72f9f2b2eab28c9d91a22065c55aa7702abf7e01/LICENSE.txt)

#### log4net@rel/2.0.12

[log4net](https://github.com/apache/logging-log4net) - [License terms.](https://github.com/apache/logging-log4net/blob/dbad144815221ffe4ed85efa73134583253dc75b/LICENSE)

#### Microsoft.Web.Xdt@2.1.1

[Microsoft.Web.Xdt](https://www.nuget.org/packages/Microsoft.Web.Xdt/2.1.1) - [License terms.](https://licenses.nuget.org/Apache-2.0)

### BSD-3-Clause

#### Rhino.Licensing@1.4.1

[Rhino.Licensing](https://github.com/ayende/rhino-licensing) [(modified)](https://github.com/chocolatey/rhino-licensing) - [License terms.](https://github.com/ayende/rhino-licensing/blob/1fc90c984b0c3012465a73afae0a53492c969eb5/license.txt)

### MIT License

#### AlphaFS@2.1.3

[AlphaFS](https://github.com/alphaleonis/AlphaFS) - [License terms.](https://github.com/alphaleonis/AlphaFS/blob/c63d46894e08d5a4e993b35131051f13203c3321/LICENSE.md)

#### Microsoft.Bcl.HashCode@1.1.1

[Microsoft.Bcl.HashCode](https://github.com/dotnet/corefx) - [License terms.](https://github.com/dotnet/corefx/blob/bdaf5f50f035df0aa98bd69b400b5d1dcff6a7b0/LICENSE)

#### Newtonsoft.Json@13.0.1

[Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) - [License terms.](https://github.com/JamesNK/Newtonsoft.Json/blob/ae9fe44e1323e91bcbd185ca1a14099fba7c021f/LICENSE.md)

#### SimpleInjector@2.8.3

[SimpleInjector](https://simpleinjector.org/) - [License terms.](https://github.com/simpleinjector/SimpleInjector/blob/0687195a7691363d4b4918e36b5e4d708e88253c/licence.txt)

#### System.Reactive@rxnet-v5.0.0

[System.Reactive](https://github.com/dotnet/reactive) - [License terms.](https://github.com/dotnet/reactive/blob/8a2df0b7850a373b3bad68b43b3839d1cb47eb2e/LICENSE)

#### System.Runtime.CompilerServices.Unsafe@4.5.3

[System.Runtime.CompilerServices.Unsafe](https://www.nuget.org/packages/System.Runtime.CompilerServices.Unsafe/4.5.3) - [License terms.](https://github.com/dotnet/corefx/blob/master/LICENSE.TXT)

#### System.Threading.Tasks.Extensions@4.5.4

[System.Threading.Tasks.Extensions](https://www.nuget.org/packages/System.Threading.Tasks.Extensions/4.5.4) - [License terms.](https://github.com/dotnet/corefx/blob/master/LICENSE.TXT)

### Other

#### 7-Zip@21.07

[7-Zip](https://www.7-zip.org/) - [License terms.](https://www.7-zip.org/license.txt)

```text
7-Zip Copyright (C) 1999-2025 Igor Pavlov.

  The licenses for files are:

    - 7z.dll:
         - The "GNU LGPL" as main license for most of the code
         - The "GNU LGPL" with "unRAR license restriction" for some code
         - The "BSD 3-clause License" for some code
         - The "BSD 2-clause License" for some code
    - All other files: the "GNU LGPL".

  Redistributions in binary form must reproduce related license information from this file.

  Note:
    You can use 7-Zip on any computer, including a computer in a commercial
    organization. You don't need to register or pay for 7-Zip.


GNU LGPL information
--------------------

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You can receive a copy of the GNU Lesser General Public License from
    http://www.gnu.org/




BSD 3-clause License in 7-Zip code
----------------------------------

  The "BSD 3-clause License" is used for the following code in 7z.dll
    1) LZFSE data decompression.
       That code was derived from the code in the "LZFSE compression library" developed by Apple Inc,
       that also uses the "BSD 3-clause License".
    2) ZSTD data decompression.
       that code was developed using original zstd decoder code as reference code.
       The original zstd decoder code was developed by Facebook Inc,
       that also uses the "BSD 3-clause License".

  Copyright (c) 2015-2016, Apple Inc. All rights reserved.
  Copyright (c) Facebook, Inc. All rights reserved.
  Copyright (c) 2023-2025 Igor Pavlov.

Text of the "BSD 3-clause License"
----------------------------------

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

3. Neither the name of the copyright holder nor the names of its contributors may
   be used to endorse or promote products derived from this software without
   specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

---




BSD 2-clause License in 7-Zip code
----------------------------------

  The "BSD 2-clause License" is used for the XXH64 code in 7-Zip.

  XXH64 code in 7-Zip was derived from the original XXH64 code developed by Yann Collet.

  Copyright (c) 2012-2021 Yann Collet.
  Copyright (c) 2023-2025 Igor Pavlov.

Text of the "BSD 2-clause License"
----------------------------------

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

---




unRAR license restriction
-------------------------

The decompression engine for RAR archives was developed using source
code of unRAR program.
All copyrights to original unRAR code are owned by Alexander Roshal.

The license for original unRAR code has the following restriction:

  The unRAR sources cannot be used to re-create the RAR compression algorithm,
  which is proprietary. Distribution of modified unRAR sources in separate form
  or as a part of other software is permitted, provided that it is clearly
  stated in the documentation and source comments that the code may
  not be used to develop a RAR (WinRAR) compatible archiver.

--
```

#### Shim Generator (shimgen)@1.0.0

[Shim Generator (shimgen)](https://github.com/chocolatey/shimgen) - [License terms.](https://github.com/chocolatey/choco/blob/d25f993696b4d665ee2dc94ceb0937a2ed5698eb/src/chocolatey.resources/tools/shimgen.license.txt)

```text
Shim Generator - shimgen.exe
Copyright (C) 2017 - Present Chocolatey Software, Inc ("CHOCOLATEY")
Copyright (C) 2013 - 2017 RealDimensions Software, LLC ("RDS")
===================================================================
Grant of License
===================================================================
ATTENTION: Shim Generator ("shimgen.exe") is a closed source application with
a proprietary license and its use is strictly limited to the terms of this 
license agreement.

RealDimensions Software, LLC ("RDS") grants Chocolatey Software, Inc a revocable, 
non-exclusive license to distribute and use shimgen.exe with the official 
Chocolatey client (https://chocolatey.org). This license file must be stored in 
Chocolatey source next to shimgen.exe and distributed with every copy of 
shimgen.exe. The distribution or use of shimgen.exe outside of these terms 
without the express written permission of RDS is strictly prohibited.

While the source for shimgen.exe is closed source, the shims have reference 
source at https://github.com/chocolatey/shimgen/tree/master/shim.

===================================================================
End-User License Agreement
===================================================================
EULA - Shim Generator

IMPORTANT- READ CAREFULLY: This RealDimensions Software, LLC ("RDS") End-User License
Agreement ("EULA") is a legal agreement between you ("END USER") and RDS for all 
RDS products, controls, source code, demos, intermediate files, media, printed 
materials, and "online" or electronic documentation (collectively "SOFTWARE 
PRODUCT(S)") contained with this distribution.

RDS grants to you as an individual or entity, a personal, non-exclusive license 
to install and use the SOFTWARE PRODUCT(S) for the sole purpose of use with the 
official Chocolatey client. By installing, copying, or otherwise using the 
SOFTWARE PRODUCT(S), END USER agrees to be bound by the terms of this EULA. If 
END USER does not agree to any part of the terms of this EULA, DO NOT INSTALL, 
USE, OR EVALUATE, ANY PART, FILE OR PORTION OF THE SOFTWARE PRODUCT(S).

In no event shall RDS be liable to END USER for damages, including any direct, 
indirect, special, incidental, or consequential damages of any character arising
as a result of the use or inability to use the SOFTWARE PRODUCT(S) (including 
but not limited to damages for loss of goodwill, work stoppage, computer failure
or malfunction, or any and all other commercial damages or losses).

The liability of RDS to END USER for any reason and upon any cause of action 
related to the performance of the work under this agreement whether in tort or 
in contract or otherwise shall be limited to the amount paid by the END USER to 
RDS pursuant to this agreement or as determined by written agreement signed 
by both RDS and END USER.

ALL SOFTWARE PRODUCT(S) are licensed not sold. If you are an individual, you 
must acquire an individual license for the SOFTWARE PRODUCT(S) from RDS or its 
authorized resellers. If you are an entity, you must acquire an individual license 
for each machine running the SOFTWARE PRODUCT(S) within your organization from RDS 
or its authorized resellers. Both virtual and physical machines running the SOFTWARE 
PRODUCT(S) must be counted in the SOFTWARE PRODUCT(S) licenses quantity of the 
organization.

===================================================================
Commercial / Personal Licensing
===================================================================
Shim Generator ("shimgen.exe") is also offered under personal and commercial 
licenses. You can learn more by contacting Chocolatey at https://chocolatey.org/contact.
```

## Chocolatey GUI

### Apache v2.0 License

#### Chocolatey CLI / Chocolatey.Lib@2.0.0

[Chocolatey CLI / Chocolatey.Lib](https://github.com/chocolatey/choco) - [License terms.](https://github.com/chocolatey/choco/blob/38b5e5ce3b083acf591f988243e9d8e8ccbfb832/LICENSE)

#### Serilog@2.5.0

[Serilog](https://github.com/serilog/serilog) - [License terms.](https://github.com/serilog/serilog/blob/7e9b58adcbd5b300a21811943c911fa3040bfc25/LICENSE)

#### Serilog.Formatting.Compact@1.0.0

[Serilog.Formatting.Compact](https://github.com/serilog/serilog-formatting-compact) - [License terms.](https://github.com/serilog/serilog-formatting-compact/blob/06e18e46e5c2e40feb6f880b8d420aec7677c274/LICENSE)

#### Serilog.Sinks.Async@1.1.0

[Serilog.Sinks.Async](https://github.com/serilog/serilog-sinks-async) - [License terms.](https://github.com/serilog/serilog-sinks-async/blob/752c27f0b0cd1771c344c52f4485d61bff438ed4/LICENSE)

#### Serilog.Sinks.Console@3.1.0

[Serilog.Sinks.Console](https://github.com/serilog/serilog-sinks-console) - [License terms.](https://github.com/serilog/serilog-sinks-console/blob/4b5ef34643b5d0e76973e5256bebe05fd3b88280/LICENSE)

#### Serilog.Sinks.File@3.2.0

[Serilog.Sinks.File](https://github.com/serilog/serilog-sinks-file) - [License terms.](https://github.com/serilog/serilog-sinks-file/blob/c8418ed96ad8c02fa4b472b03459153175adb4d6/LICENSE)

#### Serilog.Sinks.PeriodicBatching@2.1.1

[Serilog.Sinks.PeriodicBatching](https://github.com/serilog/serilog-sinks-periodicbatching) - [License terms.](https://github.com/serilog/serilog-sinks-periodicbatching/blob/16c50371424f3626f75ab19b593043555b97c676/LICENSE)

#### Serilog.Sinks.RollingFile@3.3.0

[Serilog.Sinks.RollingFile](https://github.com/serilog/serilog-sinks-rollingfile) - [License terms.](https://github.com/serilog/serilog-sinks-rollingfile/blob/2a24d25b8fb56f9aab7eeb9887b728c060bf50d6/LICENSE)

#### System.Reactive.Core@3.1.1

[System.Reactive.Core](https://github.com/dotnet/reactive/) - [License terms.](https://github.com/dotnet/reactive/blob/e0b6af3e204feb8aa13841a8a873d78ae6c43467/LICENSE)

#### System.Reactive.Interfaces@3.1.1

[System.Reactive.Interfaces](https://github.com/dotnet/reactive) - [License terms.](https://github.com/dotnet/reactive/blob/e0b6af3e204feb8aa13841a8a873d78ae6c43467/LICENSE)

#### System.Reactive.Linq@3.1.1

[System.Reactive.Linq](https://github.com/dotnet/reactive) - [License terms.](https://github.com/dotnet/reactive/blob/e0b6af3e204feb8aa13841a8a873d78ae6c43467/LICENSE)

#### System.Reactive.PlatformServices@3.1.1

[System.Reactive.PlatformServices](https://github.com/dotnet/reactive) - [License terms.](https://github.com/dotnet/reactive/blob/e0b6af3e204feb8aa13841a8a873d78ae6c43467/LICENSE)

#### System.Reactive.Windows.Threading@3.1.1

[System.Reactive.Windows.Threading](https://github.com/dotnet/reactive) - [License terms.](https://github.com/dotnet/reactive/blob/e0b6af3e204feb8aa13841a8a873d78ae6c43467/LICENSE)

### BSD-2-Clause License

#### Markdig.Signed@0.23.0

[Markdig.Signed](https://github.com/xoofx/markdig) - [License terms.](https://github.com/xoofx/markdig/blob/3030b72f781f902cb947a173853de95de8924c6c/license.txt)

### GNU Lesser General Public License (LGPL) v3.0

#### Fizzler@1.2.0

[Fizzler](https://github.com/atifaziz/Fizzler) - [License terms.](https://github.com/atifaziz/Fizzler/blob/f5f2e983e58f2198229d63de4c0563e2952bdb68/COPYING.LESSER.txt)

### Microsoft Public License

#### Svg.Custom@release/0.3.0

[Svg.Custom](https://github.com/wieslawsoltes/Svg.Skia) - [License terms.](https://github.com/wieslawsoltes/Svg.Skia/blob/c65367905b814451a9e3d00dc8da31ecb5d5914a/src/Svg.Custom/LICENSE.TXT)

### Microsoft Software License

#### System.Runtime.InteropServices.RuntimeInformation@4.3.0

[System.Runtime.InteropServices.RuntimeInformation](https://www.nuget.org/packages/System.Runtime.InteropServices.RuntimeInformation/4.3.0) - [License terms.](https://github.com/dotnet/core/blob/main/license-information.md)

### MIT License

#### Autofac@4.6.1

[Autofac](https://github.com/autofac/Autofac) - [License terms.](https://github.com/autofac/Autofac/blob/c5e5cfb38fc5ec363b96e95917b77d86d577c03f/LICENSE)

#### AutoMapper@7.0.1

[AutoMapper](https://github.com/AutoMapper/AutoMapper) - [License terms.](https://github.com/AutoMapper/AutoMapper/blob/75344b28d2152f1570c8261dce7346abfed2b837/LICENSE.txt)

#### Caliburn.Micro@3.2.0

[Caliburn.Micro](https://github.com/Caliburn-Micro/Caliburn.Micro) - [License terms.](https://github.com/Caliburn-Micro/Caliburn.Micro/blob/544babd1865846af175ba32d86c839eb354714aa/License.txt)

#### Caliburn.Micro.Core@3.2.0

[Caliburn.Micro.Core](https://github.com/Caliburn-Micro/Caliburn.Micro/) - [License terms.](https://github.com/Caliburn-Micro/Caliburn.Micro/blob/544babd1865846af175ba32d86c839eb354714aa/License.txt)

#### ControlzEx@4.4.0

[ControlzEx](https://github.com/ControlzEx/ControlzEx) - [License terms.](https://github.com/ControlzEx/ControlzEx/blob/3d188a23b59272b6feff707c4f96be8743576e0d/LICENSE)

#### HarfBuzzSharp@2.6.1.4

[HarfBuzzSharp](https://github.com/mono/SkiaSharp) - [License terms.](https://github.com/mono/SkiaSharp/blob/fd9484a06ae96f0195a80d93e80d6b54323f450a/LICENSE.md)

#### LiteDB@5.0.15

[LiteDB](https://github.com/litedb-org/LiteDB) - [License terms.](https://github.com/litedb-org/LiteDB/blob/d91d495e5f0e5ff08f74837b794545e40de34fcd/LICENSE)

#### MahApps.Metro@2.4.4

[MahApps.Metro](https://github.com/MahApps/MahApps.Metro) - [License terms.](https://github.com/MahApps/MahApps.Metro/blob/29a0ac40a9b99e192aeff63a6def6580c0203076/LICENSE)

#### MahApps.Metro.IconPacks.BoxIcons@4.8.0

[MahApps.Metro.IconPacks.BoxIcons](https://github.com/MahApps/MahApps.Metro.IconPacks) - [License terms.](https://github.com/MahApps/MahApps.Metro.IconPacks/blob/a24bc41dddd8c896949e6f6d01512ffbb14bed94/LICENSE)

#### MahApps.Metro.IconPacks.Entypo@4.8.0

[MahApps.Metro.IconPacks.Entypo](https://github.com/MahApps/MahApps.Metro.IconPacks) - [License terms.](https://github.com/MahApps/MahApps.Metro.IconPacks/blob/a24bc41dddd8c896949e6f6d01512ffbb14bed94/LICENSE)

#### MahApps.Metro.IconPacks.FontAwesome@4.8.0

[MahApps.Metro.IconPacks.FontAwesome](https://github.com/MahApps/MahApps.Metro.IconPacks) - [License terms.](https://github.com/MahApps/MahApps.Metro.IconPacks/blob/a24bc41dddd8c896949e6f6d01512ffbb14bed94/LICENSE)

#### MahApps.Metro.IconPacks.Modern@4.8.0

[MahApps.Metro.IconPacks.Modern](https://github.com/MahApps/MahApps.Metro.IconPacks) - [License terms.](https://github.com/MahApps/MahApps.Metro.IconPacks/blob/a24bc41dddd8c896949e6f6d01512ffbb14bed94/LICENSE)

#### MahApps.Metro.IconPacks.Octicons@4.8.0

[MahApps.Metro.IconPacks.Octicons](https://github.com/MahApps/MahApps.Metro.IconPacks) - [License terms.](https://github.com/MahApps/MahApps.Metro.IconPacks/blob/a24bc41dddd8c896949e6f6d01512ffbb14bed94/LICENSE)

#### MahApps.Metro.SimpleChildWindow@2.0.0

[MahApps.Metro.SimpleChildWindow](https://github.com/punker76/MahApps.Metro.SimpleChildWindow) - [License terms.](https://github.com/punker76/MahApps.Metro.SimpleChildWindow/blob/663cb42663c2e8d6f68e553878d4001360be5b21/LICENSE)

#### Markdig.Wpf.Signed@0.5.0.1

[Markdig.Wpf.Signed](https://github.com/Kryptos-FR/markdig.wpf) - [License terms.](https://github.com/Kryptos-FR/markdig.wpf/blob/f172c1337eacd80f5fd138b1eb2547e62690b243/LICENSE.md)

#### Microsoft.VisualStudio.Threading@15.4.4

[Microsoft.VisualStudio.Threading](https://github.com/microsoft/vs-threading) - [License terms.](https://github.com/microsoft/vs-threading/blob/da0daeef0c0c21913f59f099fe1cf684431a3b6a/LICENSE)

#### Microsoft.VisualStudio.Validation@15.3.32

[Microsoft.VisualStudio.Validation](https://www.nuget.org/packages/Microsoft.VisualStudio.Validation/15.3.32) - [License terms.](https://raw.githubusercontent.com/Microsoft/vs-validation/653b14c15e/LICENSE)

#### Microsoft.Xaml.Behaviors.Wpf@1.1.19

[Microsoft.Xaml.Behaviors.Wpf](https://github.com/microsoft/XamlBehaviorsWpf) - [License terms.](https://github.com/microsoft/XamlBehaviorsWpf/blob/b88a49dfadcc98f41634624124ee19f9d9dc4df2/LICENSE)

#### SkiaSharp@1.68.3

[SkiaSharp](https://github.com/mono/SkiaSharp) - [License terms.](https://github.com/mono/SkiaSharp/blob/fd9484a06ae96f0195a80d93e80d6b54323f450a/LICENSE.txt)

#### SkiaSharp.HarfBuzz@1.68.3

[SkiaSharp.HarfBuzz](https://github.com/mono/SkiaSharp/tree/main/changelogs/SkiaSharp.HarfBuzz) - [License terms.](https://github.com/mono/SkiaSharp/blob/fd9484a06ae96f0195a80d93e80d6b54323f450a/LICENSE.txt)

#### Splat@2.0.0

[Splat](https://github.com/reactiveui/splat) - [License terms.](https://github.com/reactiveui/splat/blob/5a4912c1e0f5eeb434ec9f2452f6571d2e34c34b/LICENSE)

#### Svg.Skia@0.3.0

[Svg.Skia](https://github.com/wieslawsoltes/Svg.Skia) - [License terms.](https://github.com/wieslawsoltes/Svg.Skia/blob/c65367905b814451a9e3d00dc8da31ecb5d5914a/LICENSE.TXT)

#### System.Buffers@4.5.1

[System.Buffers](https://www.nuget.org/packages/System.Buffers/4.5.1) - [License terms.](https://github.com/dotnet/corefx/blob/master/LICENSE.TXT)

#### System.Memory@4.5.4

[System.Memory](https://www.nuget.org/packages/System.Memory/4.5.4) - [License terms.](https://github.com/dotnet/corefx/blob/master/LICENSE.TXT)

#### System.Numerics.Vectors@4.5.0

[System.Numerics.Vectors](https://www.nuget.org/packages/System.Numerics.Vectors/4.5.0) - [License terms.](https://github.com/dotnet/corefx/blob/master/LICENSE.TXT)

#### System.Runtime.CompilerServices.Unsafe@4.7.1

[System.Runtime.CompilerServices.Unsafe](https://www.nuget.org/packages/System.Runtime.CompilerServices.Unsafe/4.7.1) - [License terms.](https://licenses.nuget.org/MIT)

#### System.Threading.Tasks.Extensions@4.4.0

[System.Threading.Tasks.Extensions](https://www.nuget.org/packages/System.Threading.Tasks.Extensions/4.4.0) - [License terms.](https://github.com/dotnet/corefx/blob/master/LICENSE.TXT)

#### System.ValueTuple@4.5.0

[System.ValueTuple](https://www.nuget.org/packages/System.ValueTuple/4.5.0) - [License terms.](https://github.com/dotnet/corefx/blob/master/LICENSE.TXT)