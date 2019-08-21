//------------------------------------------------------------------------------
// <copyright file="ImportWorkItemsCommand.cs" company="Microsoft">
//     Copyright (c) Microsoft.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;
using System.IO;

namespace ImportWorkItems
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ImportWorkItemsCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("4184bdbf-a358-4dc0-ab7a-31e92adf177a");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportWorkItemsCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ImportWorkItemsCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                //var menuItem = new MenuCommand(this.StartTFSidekicks, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        private void StartTFSidekicks(object sender, EventArgs e)
        {
            string filename = string.Format("{0}\\{1}", Environment.CurrentDirectory, "TFSideKicks.exe");
            using (Process pro = new Process())
            {
                FileInfo file = new FileInfo(filename);
                pro.StartInfo.WorkingDirectory = file.Directory.FullName;
                pro.StartInfo.FileName = filename;
                pro.StartInfo.CreateNoWindow = false;
                pro.StartInfo.UseShellExecute = false;

                pro.Start();
                pro.WaitForExit();
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ImportWorkItemsCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new ImportWorkItemsCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            //string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
            //string title = "Import Work Items";

            //// Show a message box to prove we were here
            //VsShellUtilities.ShowMessageBox(
            //    this.ServiceProvider,
            //    message,
            //    title,
            //    OLEMSGICON.OLEMSGICON_INFO,
            //    OLEMSGBUTTON.OLEMSGBUTTON_OK,
            //    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

            MainForm.ShowDialog();
        }

        private static MainWindow _mainForm;
        public static MainWindow MainForm
        {
            get
            {
                if (_mainForm == null) _mainForm = new MainWindow();
                return _mainForm;
            }
        }
    }
}
