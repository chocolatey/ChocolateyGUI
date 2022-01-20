// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="LocalizeExtension.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Data;
using ChocolateyGui.Common.Utilities;

namespace ChocolateyGui.Common.Windows.Utilities.Extensions
{
    public sealed class LocalizeExtension : Binding
    {
        public LocalizeExtension(string name)
            : base("[" + name + "]")
        {
            Mode = BindingMode.OneWay;
            Source = TranslationSource.Instance;
        }
    }
}