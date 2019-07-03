// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="SourcesListBoxItemStyleSelector.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using ChocolateyGui.Common.ViewModels;

namespace ChocolateyGui.Common.Windows.ViewModels
{
    public class SourcesListBoxItemStyleSelector : StyleSelector
    {
        public Style ListBoxItemContainerStyleKey { get; set; }

        public Style SeparatorContainerStyleKey { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is SourceSeparatorViewModel && SeparatorContainerStyleKey != null)
            {
                return SeparatorContainerStyleKey;
            }
            else if (item is ISourceViewModelBase && ListBoxItemContainerStyleKey != null)
            {
                return ListBoxItemContainerStyleKey;
            }

            return base.SelectStyle(item, container);
        }
    }
}