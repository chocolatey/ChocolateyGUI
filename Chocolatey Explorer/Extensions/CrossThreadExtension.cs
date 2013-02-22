using System;
using System.Windows.Forms;

namespace Chocolatey.Explorer.Extensions
{
    public static class CrossThreadExtension
    {
        public static void Invoke(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new MethodInvoker(action), null);
            }
            else
            {
                action.Invoke();
            }
        }
    }
}