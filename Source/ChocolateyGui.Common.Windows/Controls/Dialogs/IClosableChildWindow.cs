// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IClosableChildWindow.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace ChocolateyGui.Common.Windows.Controls.Dialogs
{
    public interface IClosableChildWindow<TResult>
    {
        Action<TResult> Close { get; set; }
    }
}