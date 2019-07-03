// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="CommandExecutionManager.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Expression = System.Linq.Expressions.Expression;

namespace ChocolateyGui.Common.Windows.Commands
{
    /// <summary>
    ///     This class provides a single method that allows other classes to dynamically execute
    ///     command methods on objects. The execution is performed using mechanisms that provide
    ///     better performance than using reflection.
    /// </summary>
    public static class CommandExecutionManager
    {
        private static readonly ConcurrentDictionary<CommandExecutionProviderKey, Func<object>> CompiledConstructors
            = new ConcurrentDictionary<CommandExecutionProviderKey, Func<object>>();

        private static readonly ConcurrentDictionary<CommandExecutionProviderKey, ICommandExecutionProvider>
            ExecutionProviders
                = new ConcurrentDictionary<CommandExecutionProviderKey, ICommandExecutionProvider>();

        private static object _disconnectedItemSentinelValue;

        private static Func<object, bool> _isDisconnected;

        /// <summary>
        ///     Represents an object that is capable of executing a specific CanExecute method and
        ///     Execute method for a specific LineType on any object of the specific type.
        /// </summary>
        private interface ICommandExecutionProvider
        {
            string CanExecuteMethodName { get; }

            string ExecutedMethodName { get; }

            Type TargetType { get; }

            bool InvokeCanExecuteMethod(object target, object parameter);

            void InvokeExecutedMethod(object target, object parameter);
        }

        private static Func<object, bool> IsDisconnected
        {
            get
            {
                if (_isDisconnected == null)
                {
                    var objectType = typeof(object);
                    var param = Expression.Parameter(objectType, "Target");
                    var label = Expression.Label();

                    var targetTypeVarExpr = Expression.Variable(typeof(Type), "targetType");
                    var setTargetTypeVarExpr = Expression.Assign(
                        targetTypeVarExpr,
                        Expression.Call(param, objectType.GetMethod("GetType")));
                    var targetTypeNameExpr = Expression.PropertyOrField(targetTypeVarExpr, "FullName");

                    var targetNameExpr = Expression.PropertyOrField(param, "_name");
                    var returnExpr = Expression.IfThenElse(
                        Expression.Equal(targetNameExpr, Expression.Constant("DisconnectedItem")),
                        Expression.Return(label, Expression.Constant(true)),
                        Expression.Return(label, Expression.Constant(false)));

                    var branchExpr =
                        Expression.IfThen(
                            Expression.Equal(targetTypeNameExpr, Expression.Constant("MS.Internal.NamedObject")),
                            returnExpr);

                    var block = Expression.Block(
                        targetTypeVarExpr,
                        setTargetTypeVarExpr,
                        branchExpr);

                    _isDisconnected = Expression.Lambda<Func<object, bool>>(block, param).Compile();
                }

                return _isDisconnected;
            }
        }

        /// <summary>
        ///     Attempts to dynamically execute the method indicated by canExecuteMethodName
        ///     and, if necessary, the method indicated by executedMethodName on the provided
        ///     target object.
        /// </summary>
        /// <param name="target">
        ///     The object on which the command methods are to executed.
        /// </param>
        /// <param name="parameter">
        ///     The command parameter.
        /// </param>
        /// <param name="execute">
        ///     True if the execute method should be executed; otherwise only the can execute
        ///     method is called.
        /// </param>
        /// <param name="executedMethodName">
        ///     The name of the method on the target object that contains the execution logic for
        ///     the command.
        /// </param>
        /// <param name="canExecuteMethodName">
        ///     The name of the method on the target object that contains the can execute logic for
        ///     the command.
        /// </param>
        /// <param name="canExecute">
        ///     The return value of the call to the can execute method. If canExecuteMethodName is
        ///     null, true is returned.
        /// </param>
        /// <returns>
        ///     True if the command logic was successfully executed; otherwise false.
        /// </returns>
        public static bool TryExecuteCommand(
            object target,
            object parameter,
            bool execute,
            string executedMethodName,
            string canExecuteMethodName,
            out bool canExecute)
        {
            var designTime = DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (designTime)
            {
                canExecute = true;
                return true;
            }

            if (target != null && !string.IsNullOrEmpty(executedMethodName))
            {
                var executionProvider = GetCommandExecutionProvider(target, canExecuteMethodName, executedMethodName);
                if (executionProvider != null)
                {
                    canExecute = executionProvider.InvokeCanExecuteMethod(target, parameter);
                    if (canExecute && execute)
                    {
                        executionProvider.InvokeExecutedMethod(target, parameter);
                    }

                    return true;
                }
            }

            canExecute = false;
            return false;
        }

        private static ICommandExecutionProvider GetCommandExecutionProvider(
            object target,
            string canExecuteMethodName,
            string executedMethodName)
        {
            if (target == _disconnectedItemSentinelValue)
            {
                return null;
            }

            var key = new CommandExecutionProviderKey(target.GetType(), canExecuteMethodName, executedMethodName);
            ICommandExecutionProvider executionProvider;
            if (!ExecutionProviders.TryGetValue(key, out executionProvider))
            {
                try
                {
                    executionProvider = (ICommandExecutionProvider)GetCommandExecutionProviderConstructor(key)();
                }
                catch (TargetInvocationException)
                {
                    // Thanks to Mark Bergan for finding this issue!
                    // Unfortunately we have some nastiness around a performance optimization in C# 4.0.
                    // Because we are listening to DataContext events we may end up being provided a DataContext
                    // value that is an internal place holder for the DataContext of disconnected containers WPF.
                    // There is no easy way to detect this object, hence the reflection. Basically we just want to
                    // ignore any disconnected DataContext items. The best documentation I have found is located in
                    // Answer 10 on the forum located here:
                    // http://www.go4answers.com/Example/disconnecteditem-causing-it-115624.aspx
                    if (_disconnectedItemSentinelValue == null)
                    {
                        if (IsDisconnected(target))
                        {
                            _disconnectedItemSentinelValue = target;
                        }

                        ////var targetType = target.GetType();
                        ////if (targetType.FullName == "MS.Internal.NamedObject")
                        ////{
                        ////    var nameField = targetType.GetField("_name", BindingFlags.Instance | BindingFlags.NonPublic);
                        ////    if (nameField != null)
                        ////        if ((string)nameField.GetValue(target) == "DisconnectedItem")
                        ////        {
                        ////            _DisconnectedItemSentinelValue = target;
                        ////        }
                        ////}
                    }

                    if (target != _disconnectedItemSentinelValue)
                    {
                        throw;
                    }
                }

                ExecutionProviders.TryAdd(key, executionProvider);
            }

            return executionProvider;
        }

        private static Func<object> GetCommandExecutionProviderConstructor(CommandExecutionProviderKey key)
        {
            Func<object> constructor;
            if (!CompiledConstructors.TryGetValue(key, out constructor))
            {
                var executionProviderType = typeof(CommandExecutionProvider<>).MakeGenericType(key.TargetType);
                var executionProviderCtor =
                    executionProviderType.GetConstructor(new[] { typeof(Type), typeof(string), typeof(string) });

                var executionProviderCtorParamaters = new Expression[]
                {
                    Expression.Constant(key.TargetType),
                    Expression.Constant(key.CanExecuteMethodName, typeof(string)),
                    Expression.Constant(key.ExecutedMethodName, typeof(string))
                };

                Debug.Assert(executionProviderCtor != null, "executionProviderCtor != null");
                var executionProviderCtorExpression = Expression.New(
                    executionProviderCtor,
                    executionProviderCtorParamaters);
                constructor = Expression.Lambda<Func<object>>(executionProviderCtorExpression).Compile();
            }

            return constructor;
        }

        /// <summary>
        ///     Represents a unique combination of target object LineType, can execute name and executed
        ///     method name. This key is used to cache ICommandExecutionProvider implementations
        ///     that are specifically tailored to the combination these three values.
        /// </summary>
        private struct CommandExecutionProviderKey
        {
            public CommandExecutionProviderKey(Type targetType, string canExecuteMethodName, string executedMethodName)
                : this()
            {
                TargetType = targetType;
                CanExecuteMethodName = canExecuteMethodName;
                ExecutedMethodName = executedMethodName;
            }

            public string CanExecuteMethodName { get; }

            public string ExecutedMethodName { get; }

            public Type TargetType { get; }
        }

        /// <summary>
        ///     Represents an object that is capable of executing a specific CanExecute method and
        ///     Execute method for a specific type on any object of the specific type.
        /// </summary>
        /// <typeparam name="TTarget">The target LineType</typeparam>
        private class CommandExecutionProvider<TTarget> : ICommandExecutionProvider
        {
            private readonly Func<TTarget, bool> _canExecute;
            private readonly Func<TTarget, object, bool> _canExecuteWithParam;
            private readonly Action<TTarget> _executed;
            private readonly Action<TTarget, object> _executedWithParam;

            public CommandExecutionProvider(Type targetType, string canExecuteMethodName, string executedMethodName)
            {
                TargetType = targetType;
                CanExecuteMethodName = canExecuteMethodName;
                ExecutedMethodName = executedMethodName;

                var targetParameter = Expression.Parameter(targetType);
                var paramParamater = Expression.Parameter(typeof(object));

                var canExecuteMethodInfo = GetMethodInfo(CanExecuteMethodName);
                if (canExecuteMethodInfo != null && canExecuteMethodInfo.ReturnType == typeof(bool))
                {
                    if (canExecuteMethodInfo.GetParameters().Length == 0)
                    {
                        _canExecute =
                            Expression.Lambda<Func<TTarget, bool>>(
                                Expression.Call(targetParameter, canExecuteMethodInfo),
                                targetParameter).Compile();
                    }
                    else
                    {
                        _canExecuteWithParam =
                            Expression.Lambda<Func<TTarget, object, bool>>(
                                Expression.Call(targetParameter, canExecuteMethodInfo, paramParamater),
                                targetParameter,
                                paramParamater).Compile();
                    }
                }

                if (_canExecute == null && _canExecuteWithParam == null)
                {
                    _canExecute =
                        Expression.Lambda<Func<TTarget, bool>>(Expression.Constant(true), targetParameter).Compile();
                    ////throw new Exception(string.Format(
                    ////    "Method {0} on type {1} does not have a valid method signature. The method must have one of the following signatures: 'public bool CanExecute()' or 'public bool CanExecute(object parameter)'",
                    ////    CanExecuteMethodName, typeof(TTarget)));
                }

                var executedMethodInfo = GetMethodInfo(ExecutedMethodName);
                if (executedMethodInfo != null &&
                    (executedMethodInfo.ReturnType == typeof(void) || executedMethodInfo.ReturnType == typeof(Task)))
                {
                    if (executedMethodInfo.GetParameters().Length == 0)
                    {
                        _executed =
                            Expression.Lambda<Action<TTarget>>(
                                Expression.Call(targetParameter, executedMethodInfo),
                                targetParameter).Compile();
                    }
                    else
                    {
                        _executedWithParam =
                            Expression.Lambda<Action<TTarget, object>>(
                                Expression.Call(targetParameter, executedMethodInfo, paramParamater),
                                targetParameter,
                                paramParamater).Compile();
                    }
                }

                if (_executed == null && _executedWithParam == null)
                {
                    throw new Exception(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            "Method {0} on type {1} does not have a valid method signature. The method must have one of the following signatures: 'public void {2}()' or 'public void {2}(object parameter)'",
                            ExecutedMethodName,
                            typeof(TTarget),
                            executedMethodName));
                }
            }

            public string CanExecuteMethodName { get; }

            public string ExecutedMethodName { get; }

            public Type TargetType { get; }

            public bool InvokeCanExecuteMethod(object target, object parameter)
            {
                if (_canExecute != null)
                {
                    return _canExecute((TTarget)target);
                }

                if (_canExecuteWithParam != null)
                {
                    return _canExecuteWithParam((TTarget)target, parameter);
                }

                return false;
            }

            public void InvokeExecutedMethod(object target, object parameter)
            {
                if (_executed != null)
                {
                    _executed((TTarget)target);
                }
                else if (_executedWithParam != null)
                {
                    _executedWithParam((TTarget)target, parameter);
                }
            }

            private static MethodInfo GetMethodInfo(string methodName)
            {
                if (string.IsNullOrWhiteSpace(methodName))
                {
                    return null;
                }

                return typeof(TTarget).GetMethod(methodName, new[] { typeof(object) })
                       ?? typeof(TTarget).GetMethod(methodName, new Type[0]);
            }
        }
    }
}