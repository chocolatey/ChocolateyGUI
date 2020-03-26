// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="BindingProxy.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;

namespace ChocolateyGui.Common.Windows.Utilities
{
    /// <summary>
    /// BindingProxy class with a DependencyProperty as the backing store for Data.
    /// This enables animation, styling, binding, etc...
    /// And prevents errors like:
    /// System.Windows.Data Error: 2 : Cannot find governing FrameworkElement or FrameworkContentElement for target element. BindingExpression:Path=ShowConsoleOutput; DataItem=null; target element is 'ChocolateyGUISetting' (HashCode=61659320); target property is 'Enabled' (type 'Boolean')
    /// </summary>
    public class BindingProxy : Freezable
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Data",
            typeof(object),
            typeof(BindingProxy),
            new UIPropertyMetadata(null));

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }
    }
}