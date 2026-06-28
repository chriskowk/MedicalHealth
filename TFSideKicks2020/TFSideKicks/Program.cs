using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TFSideKicks.Helpers;

namespace TFSideKicks
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (Debugger.IsAttached)
            {
                Run(args);
            }
            else
            {
                try
                {
                    Run(args);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private static void Run(string[] args)
        {
            CommandLineArguments arguments = new CommandLineArguments(args);
            ConfigHelper.Arguments = arguments;
            App.Main();
        }

        static void app_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            bool isPTS = e.Exception.GetType().FullName.Contains("PTS");
            if (isPTS && e.Exception.HResult == -2146233088)  //MS.Internal.PtsHost.UnsafeNativeMethods.PTS.SecondaryException
            {
                e.Handled = true;
                return;
            }

            var comException = e.Exception as System.Runtime.InteropServices.COMException;
            if (comException != null && comException.ErrorCode == -2147221040)
                e.Handled = true;

            e.Handled = true;
        }
    }
}
