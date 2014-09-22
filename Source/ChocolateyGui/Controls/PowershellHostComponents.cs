// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ChocolateyHost.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Controls
{
    using ChocolateyGui.Models;
    using ChocolateyGui.Services;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Management.Automation;
    using System.Management.Automation.Host;
    using System.Reflection;

    internal class ChocolateyHost : PSHost
    {
        private readonly ChocolateyHostUserInterface _chocolateyHostUserInterface;

        private readonly Guid _hostInstanceId = Guid.NewGuid();

        private readonly CultureInfo _originalCultureInfo =
            System.Threading.Thread.CurrentThread.CurrentCulture;

        private readonly CultureInfo _originalUiCultureInfo =
            System.Threading.Thread.CurrentThread.CurrentUICulture;

        public ChocolateyHost(IProgressService progressService)
        {
            this._chocolateyHostUserInterface = new ChocolateyHostUserInterface(progressService);
        }

        public override CultureInfo CurrentCulture
        {
            get { return this._originalCultureInfo; }
        }

        public override CultureInfo CurrentUICulture
        {
            get { return this._originalUiCultureInfo; }
        }

        public override Guid InstanceId
        {
            get { return this._hostInstanceId; }
        }

        public override string Name
        {
            get { return @"ChocolateyGUI PowerShell Host"; }
        }

        public override PSHostUserInterface UI
        {
            get { return this._chocolateyHostUserInterface; }
        }

        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public override void EnterNestedPrompt()
        {
            throw new NotImplementedException();
        }

        public override void ExitNestedPrompt()
        {
            throw new NotImplementedException();
        }

        public override void NotifyBeginApplication()
        {
        }

        public override void NotifyEndApplication()
        {
        }

        public override void SetShouldExit(int exitCode)
        {
            throw new NotImplementedException();
        }
    }

    internal class ChocolateyHostRawUserInterface : PSHostRawUserInterface
    {
        /// <summary>
        /// Gets or sets the background color of the displayed text.
        /// This maps to the corresponding Console.Background property.
        /// </summary>
        public override ConsoleColor BackgroundColor
        {
            get { return Console.BackgroundColor; }
            set { Console.BackgroundColor = value; }
        }

        /// <summary>
        /// Gets or sets the size of the host buffer. In this example the 
        /// buffer size is adapted from the Console buffer size members.
        /// </summary>
        public override Size BufferSize
        {
            get { return new Size(0, 0); }
            set { }
        }

        /// <summary>
        /// Gets or sets the cursor position. In this example this 
        /// functionality is not needed so the property throws a 
        /// NotImplementException exception.
        /// </summary>
        public override Coordinates CursorPosition
        {
            get
            {
                throw new NotImplementedException(
                     "CursorPosition is not implemented.");
            }

            set
            {
                throw new NotImplementedException(
                     "NotImplementedException is not implemented.");
            }
        }

        /// <summary>
        /// Gets or sets the size of the displayed cursor. In this example 
        /// the cursor size is taken directly from the Console.CursorSize 
        /// property.
        /// </summary>
        public override int CursorSize
        {
            get { return Console.CursorSize; }
            set { Console.CursorSize = value; }
        }

        /// <summary>
        /// Gets or sets the foreground color of the displayed text.
        /// This maps to the corresponding Console.ForgroundColor property.
        /// </summary>
        public override ConsoleColor ForegroundColor
        {
            get { return Console.ForegroundColor; }
            set { Console.ForegroundColor = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the user has pressed a key. This maps   
        /// to the corresponding Console.KeyAvailable property.
        /// </summary>
        public override bool KeyAvailable
        {
            get { return Console.KeyAvailable; }
        }

        /// <summary>
        /// Gets the dimensions of the largest window that could be 
        /// rendered in the current display, if the buffer was at the least 
        /// that large. This example uses the Console.LargestWindowWidth and 
        /// Console.LargestWindowHeight properties to determine the returned 
        /// value of this property.
        /// </summary>
        public override Size MaxPhysicalWindowSize
        {
            get { return new Size(Console.LargestWindowWidth, Console.LargestWindowHeight); }
        }

        /// <summary>
        /// Gets the dimensions of the largest window size that can be 
        /// displayed. This example uses the Console.LargestWindowWidth and 
        /// console.LargestWindowHeight properties to determine the returned 
        /// value of this property.
        /// </summary>
        public override Size MaxWindowSize
        {
            get { return new Size(Console.LargestWindowWidth, Console.LargestWindowHeight); }
        }

        /// <summary>
        /// Gets or sets the position of the displayed window. This example 
        /// uses the Console window position APIs to determine the returned 
        /// value of this property.
        /// </summary>
        public override Coordinates WindowPosition
        {
            get { return new Coordinates(Console.WindowLeft, Console.WindowTop); }
            set { Console.SetWindowPosition(value.X, value.Y); }
        }

        /// <summary>
        /// Gets or sets the size of the displayed window. This example 
        /// uses the corresponding Console window size APIs to determine the  
        /// returned value of this property.
        /// </summary>
        public override Size WindowSize
        {
            get { return new Size(Console.WindowWidth, Console.WindowHeight); }
            set { Console.SetWindowSize(value.Width, value.Height); }
        }

        /// <summary>
        /// Gets or sets the title of the displayed window. The example 
        /// maps the Console.Title property to the value of this property.
        /// </summary>
        public override string WindowTitle
        {
            get { return Console.Title; }
            set { Console.Title = value; }
        }

        /// <summary>
        /// This API resets the input buffer. In this example this 
        /// functionality is not needed so the method returns nothing.
        /// </summary>
        public override void FlushInputBuffer()
        {
        }

        /// <summary>
        /// This API returns a rectangular region of the screen buffer. In 
        /// this example this functionality is not needed so the method throws 
        /// a NotImplementException exception.
        /// </summary>
        /// <param name="rectangle">Defines the size of the rectangle.</param>
        /// <returns>Throws a NotImplementedException exception.</returns>
        public override BufferCell[,] GetBufferContents(Rectangle rectangle)
        {
            throw new NotImplementedException(
                     "GetBufferContents is not implemented.");
        }

        /// <summary>
        /// This API reads a pressed, released, or pressed and released keystroke 
        /// from the keyboard device, blocking processing until a keystroke is 
        /// typed that matches the specified keystroke options. In this example 
        /// this functionality is not needed so the method throws a
        /// NotImplementException exception.
        /// </summary>
        /// <param name="options">Options, such as IncludeKeyDown,  used when 
        /// reading the keyboard.</param>
        /// <returns>Throws a NotImplementedException exception.</returns>
        public override KeyInfo ReadKey(ReadKeyOptions options)
        {
            throw new NotImplementedException(
                      "ReadKey is not implemented.");
        }

        /// <summary>
        /// This API crops a region of the screen buffer. In this example 
        /// this functionality is not needed so the method throws a
        /// NotImplementException exception.
        /// </summary>
        /// <param name="source">The region of the screen to be scrolled.</param>
        /// <param name="destination">The region of the screen to receive the 
        /// source region contents.</param>
        /// <param name="clip">The region of the screen to include in the operation.</param>
        /// <param name="fill">The character and attributes to be used to fill all cell.</param>
        public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
        {
            throw new NotImplementedException(
                      "ScrollBufferContents is not implemented.");
        }

        /// <summary>
        /// This method copies an array of buffer cells into the screen buffer 
        /// at a specified location. In this example this functionality is 
        /// not needed so the method throws a NotImplementedException exception.
        /// </summary>
        /// <param name="origin">The parameter is not used.</param>
        /// <param name="contents">The parameter is not used.</param>
        public override void SetBufferContents(Coordinates origin,
                                               BufferCell[,] contents)
        {
            throw new NotImplementedException(
                      "SetBufferContents is not implemented.");
        }

        /// <summary>
        /// This method copies a given character, foreground color, and background 
        /// color to a region of the screen buffer. In this example this 
        /// functionality is not needed so the method throws a
        /// NotImplementException exception./// </summary>
        /// <param name="rectangle">Defines the area to be filled. </param>
        /// <param name="fill">Defines the fill character.</param>
        public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
        {
            throw new NotImplementedException(
                      "SetBufferContents is not implemented.");
        }
    }

    internal class ChocolateyHostUserInterface : PSHostUserInterface
    {
        private readonly ChocolateyHostRawUserInterface _chocoRawUI = new ChocolateyHostRawUserInterface();
        private readonly IProgressService _progressService;

        public ChocolateyHostUserInterface(IProgressService progressService)
        {
            _progressService = progressService;
        }

        public override PSHostRawUserInterface RawUI
        {
            get { return this._chocoRawUI; }
        }

        public override Dictionary<string, PSObject> Prompt(
                                                            string caption,
                                                            string message,
                                                            System.Collections.ObjectModel.Collection<FieldDescription> descriptions)
        {
            throw new NotImplementedException(
                "Propmt is not implemented.");
        }

        public override int PromptForChoice(string caption, string message, System.Collections.ObjectModel.Collection<ChoiceDescription> choices, int defaultChoice)
        {
            throw new NotImplementedException("PromptForChoice is not implemented.");
        }

        public override PSCredential PromptForCredential(
                                                         string caption,
                                                         string message,
                                                         string userName,
                                                         string targetName)
        {
            throw new NotImplementedException("PromptForCredential is not implemented.");
        }

        public override PSCredential PromptForCredential(
                                                         string caption,
                                                         string message,
                                                         string userName,
                                                         string targetName,
                                                         PSCredentialTypes allowedCredentialTypes,
                                                         PSCredentialUIOptions options)
        {
            Console.WriteLine("ReadLine");
            throw new NotImplementedException("PromptForCredential is not implemented.");
        }

        public override string ReadLine()
        {
            throw new NotImplementedException("ReadLine is not implemented.");
        }

        public override System.Security.SecureString ReadLineAsSecureString()
        {
            throw new NotImplementedException("ReadLineAsSecureString is not implemented.");
        }

        public override void Write(string value)
        {
            this._progressService.Output.Add(new PowerShellOutputLine(value, PowerShellLineType.Output, newLine: false));
        }

        public override void Write(
                                   ConsoleColor foregroundColor,
                                   ConsoleColor backgroundColor,
                                   string value)
        {
            // Colors are ignored.
            this._progressService.Output.Add(new PowerShellOutputLine(value, PowerShellLineType.Output, newLine: false));
        }

        public override void WriteDebugLine(string message)
        {
            this._progressService.Output.Add(new PowerShellOutputLine(message, PowerShellLineType.Debug));
        }

        public override void WriteErrorLine(string value)
        {
            this._progressService.Output.Add(new PowerShellOutputLine(value, PowerShellLineType.Error));
        }

        public override void WriteLine()
        {
            this._progressService.Output.Add(new PowerShellOutputLine(string.Empty, PowerShellLineType.Output));
        }

        public override void WriteLine(string value)
        {
            this._progressService.Output.Add(new PowerShellOutputLine(value, PowerShellLineType.Output));
        }

        public override void WriteLine(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            // Write to the output stream, ignore the colors
            this._progressService.Output.Add(new PowerShellOutputLine(value, PowerShellLineType.Output));
        }

        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
            this._progressService.Report(record.PercentComplete);
        }

        public override void WriteVerboseLine(string message)
        {
            this._progressService.Output.Add(new PowerShellOutputLine(message, PowerShellLineType.Warning));
        }

        public override void WriteWarningLine(string message)
        {
            this._progressService.Output.Add(new PowerShellOutputLine(message, PowerShellLineType.Warning));
        }
    }
}