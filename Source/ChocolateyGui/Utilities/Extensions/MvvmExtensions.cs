// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MvvmExtensions.cs" company="Chocolatey">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace ChocolateyGui.Utilities.Extensions
{
    internal static class MvvmExtensions
    {
        public static bool SetPropertyValue<T>(this PropertyChangedBase @base, ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(property, value))
            {
                return false;
            }

            property = value;
            @base.NotifyOfPropertyChange(propertyName);
            return true;
        }

        public static async Task RunOnUIThreadAsync(this Func<Task> func)
        {
            var tcs = new TaskCompletionSource<bool>();
            System.Action action = async () =>
            {
                try
                {
                    await func();
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            };
            await action.OnUIThreadAsync();
            await tcs.Task;
        }

        public static async Task<T> RunOnUIThreadAsync<T>(this Func<Task<T>> func)
        {
            var tcs = new TaskCompletionSource<T>();
            System.Action action = async () =>
            {
                try
                {
                    var result = await func();
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            };
            await action.OnUIThreadAsync();
            return await tcs.Task;
        }
    }
}
