// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IDataGridColumnComparer.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.ComponentModel;

namespace ChocolateyGui.Common.Windows.Utilities
{
    public interface IDataGridColumnComparer : IComparer
    {
        ListSortDirection SortDirection { get; set; }
    }
}