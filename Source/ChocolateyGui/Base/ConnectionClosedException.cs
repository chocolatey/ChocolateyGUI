// <copyright file="ConnectionClosedException.cs" company="Chocolatey">
// Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>

using System;

namespace ChocolateyGui.Base
{
    /// <summary>
    /// Used to represent that a connection closed peacefully while performing operation. Likely means application is closing.
    /// </summary>
    public class ConnectionClosedException : Exception
    {
    }
}