using System;
using System.ServiceModel;

namespace ChocolateyGui
{
    public static class IpcDefaults
    {
        public static readonly NetTcpBinding DefaultBinding = 
            new NetTcpBinding(SecurityMode.Transport)
            {
                MaxReceivedMessageSize = int.MaxValue - 1,
                MaxBufferSize = int.MaxValue - 1,
                MaxBufferPoolSize = int.MaxValue - 1,
                ReaderQuotas =
                {
                    MaxArrayLength = int.MaxValue - 1,
                    MaxDepth = 32,
                    MaxStringContentLength = int.MaxValue - 1
                },
                SendTimeout = TimeSpan.FromMinutes(10),
                ReceiveTimeout = TimeSpan.FromMinutes(10)
            };

        public static readonly Uri DefaultServiceUri = new Uri("net.tcp://localhost:24020");
    }
}
