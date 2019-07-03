// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="RoutedCommandMonitor.cs">
//   Copyright 2017 - Present Chocolatey Software, LLC
//   Copyright 2014 - 2017 Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Expression = System.Linq.Expressions.Expression;

namespace ChocolateyGui.Common.Windows.Commands
{
    /// <summary>
    ///     This class listens to WPF's <see cref="RoutedEvent" /> system for
    ///     <see cref="RoutedCommand" /> events and invokes the appropriate methods of the
    ///     <see cref="RoutedCommandBinding" /> class when necessary.
    /// </summary>
    internal static class RoutedCommandMonitor
    {
        // The three objects below provide access to an internal property named
        // CommandBindingsInternal that is defined in each of the three types: UIElement,
        // UIElement3D and ContentElement.
        // The internal property is used because the CommandBindings property is lazy loaded.
        // If we accessed the CommandBindings property directly, we would cause the CommandBindings
        // collection to become instanciated on every element up the heirarchy until we reached
        // an element handling the command.
        private static readonly CommandBindingsProvider<UIElement> UIElementCommandBindingsProvider
            = new CommandBindingsProvider<UIElement>();

        private static readonly CommandBindingsProvider<UIElement3D> UIElement3DCommandBindingsProvider
            = new CommandBindingsProvider<UIElement3D>();

        private static readonly CommandBindingsProvider<ContentElement> ContentElementCommandBindingsProvider
            = new CommandBindingsProvider<ContentElement>();

        /// <summary>
        ///     The RoutedCommandBinding class calls this method inside its static constructor in
        ///     order to cause the <see cref="RoutedCommandMonitor" /> to initialize its RoutedEvent
        ///     listeners.
        /// </summary>
        internal static void Init()
        {
            RegisterClassHandlers(typeof(UIElement));
            RegisterClassHandlers(typeof(UIElement3D));
            RegisterClassHandlers(typeof(ContentElement));
        }

        private static void RegisterClassHandlers(Type type)
        {
            // Register handlers to listen for the RoutedEvents of RoutedCommands so that
            // we can look for RoutedCommandBinding when the command/event fires.
            // Although using the RegisterClassHandler method is generally frowned upon due
            // to memory leak concerns (i.e. there is not UnregisterClassHandler), it is ok here
            // because the 4 handlers are only referenced by the RoutedCommandBinding type.
            EventManager.RegisterClassHandler(
                type,
                CommandManager.PreviewCanExecuteEvent,
                new CanExecuteRoutedEventHandler(OnCommandCanExecute),
                true);

            EventManager.RegisterClassHandler(
                type,
                CommandManager.CanExecuteEvent,
                new CanExecuteRoutedEventHandler(OnCommandCanExecute),
                true);

            EventManager.RegisterClassHandler(
                type,
                CommandManager.PreviewExecutedEvent,
                new ExecutedRoutedEventHandler(OnCommandExecuted),
                true);

            EventManager.RegisterClassHandler(
                type,
                CommandManager.ExecutedEvent,
                new ExecutedRoutedEventHandler(OnCommandExecuted),
                true);
        }

        private static void OnCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            TryExecuteRoutedCommandBinding(e.Command, sender, e);
        }

        private static void OnCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TryExecuteRoutedCommandBinding(e.Command, sender, e);
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private static bool TryExecuteRoutedCommandBinding(ICommand command, object sender, RoutedEventArgs e)
        {
            // Look for a matching RoutedCommandBinding on the sender and execute it if found.
            CommandBindingCollection commandBindings = null;
            var uie = sender as UIElement;
            if (uie != null)
            {
                commandBindings = UIElementCommandBindingsProvider.GetCommandBindings(uie);
            }
            else
            {
                var uie3D = sender as UIElement3D;
                if (uie3D != null)
                {
                    commandBindings = UIElement3DCommandBindingsProvider.GetCommandBindings(uie3D);
                }
                else
                {
                    var ce = sender as ContentElement;
                    if (ce != null)
                    {
                        commandBindings = ContentElementCommandBindingsProvider.GetCommandBindings(ce);
                    }
                }
            }

            if (commandBindings == null)
            {
                return false;
            }

            foreach (var binding in commandBindings
                .OfType<RoutedCommandBinding>()
                .Where(binding => binding.Command == command && (!e.Handled || binding.ViewHandledEvents)))
            {
                if (e.RoutedEvent == CommandManager.PreviewCanExecuteEvent)
                {
                    binding.OnPreviewCanExecute(sender, (CanExecuteRoutedEventArgs)e);
                }
                else if (e.RoutedEvent == CommandManager.CanExecuteEvent)
                {
                    binding.OnCanExecute(sender, (CanExecuteRoutedEventArgs)e);
                }
                else if (e.RoutedEvent == CommandManager.PreviewExecutedEvent)
                {
                    binding.OnPreviewExecuted(sender, (ExecutedRoutedEventArgs)e);
                }
                else if (e.RoutedEvent == CommandManager.ExecutedEvent)
                {
                    binding.OnExecuted(sender, (ExecutedRoutedEventArgs)e);
                }

                if (e.Handled)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Provides access to the CommandBindingsInternal property for a target type.
        /// </summary>
        /// <typeparam name="TElementType">
        ///     Specified Target LineType
        /// </typeparam>
        private class CommandBindingsProvider<TElementType>
        {
            // _GetCommandBindings is an Func that is used to access the
            // CommandBindingsInternal property of UIElements, UIElement3D sand ContentElements.
            private Func<TElementType, CommandBindingCollection> _getCommandBindings;
            ////Delegate.CreateDelegate(typeof(CommandBindingsInternalGetter), null,
            ////typeof(TElementType).GetProperty("CommandBindingsInternal", BindingFlags.Instance | BindingFlags.NonPublic)
            ////.GetGetMethod(true));

            /// <summary>
            ///     Gets the collection of CommandBindings or null the collection is not
            ///     instantiated.
            /// </summary>
            /// <returns>The collection of CommandBindings</returns>
            internal Func<TElementType, CommandBindingCollection> GetCommandBindings
            {
                get
                {
                    if (_getCommandBindings == null)
                    {
                        var param = Expression.Parameter(typeof(TElementType));
                        _getCommandBindings =
                            Expression.Lambda<Func<TElementType, CommandBindingCollection>>(
                                Expression.Property(param, "CommandBindingsInternal"), param).Compile();
                    }

                    return _getCommandBindings;
                }
            }
        }
    }
}