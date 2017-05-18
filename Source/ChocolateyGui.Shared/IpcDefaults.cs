using System;
using System.ServiceModel;

namespace ChocolateyGui
{
    public static class IpcDefaults
    {
        public static readonly NetNamedPipeBinding DefaultBinding =
            new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport)
            {
                MaxReceivedMessageSize = int.MaxValue - 1,
                MaxBufferSize = int.MaxValue - 1,
                MaxBufferPoolSize = int.MaxValue - 1,
                ReaderQuotas =
                {
                    MaxArrayLength = int.MaxValue - 1,
                    MaxDepth = 32,
                    MaxStringContentLength = int.MaxValue - 1
                }
            };

        public static readonly Uri DefaultPipeUri = new Uri("net.pipe://localhost/chocolateygui");
    }
}
