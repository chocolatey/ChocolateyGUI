// <copyright file="IIpcServiceCallbacks.cs" company="Chocolatey">
//  Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>

using System.ServiceModel;
using ChocolateyGui.Models;

namespace ChocolateyGui
{
    [ServiceContract]
    public interface IIpcServiceCallbacks
    {
        [OperationContract(IsOneWay = true)]
        void LogMessage(StreamingLogMessage message);
    }
}