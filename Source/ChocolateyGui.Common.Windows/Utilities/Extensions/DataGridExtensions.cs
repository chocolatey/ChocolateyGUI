// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="DataGridExtensions.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace ChocolateyGui.Common.Windows.Utilities.Extensions
{
    internal static class DataGridExtensions
    {
        public static string GetSortMemberPath(this DataGridColumn column)
        {
            // find the sortmemberpath
            string sortPropertyName;

            var boundColumn = column as DataGridBoundColumn;
            if (boundColumn == null)
            {
                return null;
            }

            var binding = boundColumn.Binding as Binding;
            if (binding == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(binding.XPath))
            {
                sortPropertyName = binding.XPath;
            }
            else if (binding.Path != null)
            {
                sortPropertyName = binding.Path.Path;
            }
            else
            {
                sortPropertyName = null;
            }

            return sortPropertyName;
        }

        public static int FindSortDescription(this SortDescriptionCollection sortDescriptions, string sortPropertyName)
        {
            var index = -1;
            var i = 0;
            foreach (var sortDesc in sortDescriptions)
            {
                if (string.CompareOrdinal(sortDesc.PropertyName, sortPropertyName) == 0)
                {
                    index = i;
                    break;
                }

                i++;
            }

            return index;
        }
    }
}