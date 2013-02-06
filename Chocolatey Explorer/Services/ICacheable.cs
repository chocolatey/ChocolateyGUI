using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chocolatey.Explorer.Services
{
    interface ICacheable
    {
        void InvalidateCache();
    }
}
