// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="IDataGridColumnComparer.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.ComponentModel;

namespace ChocolateyGui.Utilities
{
    public interface IDataGridColumnComparer : IComparer
    {
        ListSortDirection SortDirection { get; set; }
    }
}