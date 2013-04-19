using System;
using System.Windows.Forms;

namespace Chocolatey.Explorer.Extensions
{
    public static class CrossThreadExtension
    {
        /// <summary>
        /// Easy to use CrossThreadCall extension.
        /// </summary>
        /// <param name="control">Any control that has an InvokeRequired method, usually a form</param>
        /// <param name="action">The code to execute that is otherwise not threadsafe.</param>
        /// <example>In a form
        /// this.Invoke(() => 
        ///   {
        ///     lbl1.Text = "test";
        ///     lbl2.Text = "test";
        ///   }
        /// );</example>
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