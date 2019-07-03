// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="PackageAuthorsComparer.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ChocolateyGui.Common.ViewModels.Items;

namespace ChocolateyGui.Common.Windows.Utilities
{
    public class PackageAuthorsComparer : IDataGridColumnComparer, IComparer<IPackageViewModel>
    {
        public ListSortDirection SortDirection { get; set; }

        public int Compare(object x, object y)
        {
            return Compare(x as IPackageViewModel, y as IPackageViewModel);
        }

        public int Compare(IPackageViewModel x, IPackageViewModel y)
        {
            if (x?.Authors == null)
            {
                return -1;
            }

            if (y?.Authors == null)
            {
                return 1;
            }

            var a = string.Join(", ", x.Authors.Select(item => item.Trim()).ToList());
            var b = string.Join(", ", y.Authors.Select(item => item.Trim()).ToList());
            var result = string.Compare(a, b, StringComparison.OrdinalIgnoreCase);
            return SortDirection == ListSortDirection.Ascending ? result : -result;
        }
    }
}