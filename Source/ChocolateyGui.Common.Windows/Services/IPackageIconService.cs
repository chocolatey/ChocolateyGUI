// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageIconService.cs" company="Chocolatey">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ChocolateyGui.Common.Windows.Services
{
    public interface IPackageIconService
    {
        Task<ImageSource> GetImage(string url, Size desiredSize, DateTime absoluteExpiration);

        ImageSource GetEmptyIconImage();

        ImageSource GetErrorIconImage();
    }
}