// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionClosedException.cs" company="Chocolatey">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace ChocolateyGui.Common.Base
{
    /// <summary>
    /// Used to represent that a connection closed peacefully while performing operation. Likely means application is closing.
    /// </summary>
    public class ConnectionClosedException : Exception
    {
    }
}