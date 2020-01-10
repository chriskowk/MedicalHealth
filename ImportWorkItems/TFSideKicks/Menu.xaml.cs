using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TFSideKicks
{
    /// <summary>
    /// Menu.xaml 的交互逻辑
    /// </summary>
    public partial class Menu : Window
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)e.OriginalSource;
            if (button.Tag.ToString() == "exit")
            {
                this.Close();
                Application.Current.Shutdown();
                return;
            }

            if (string.Equals(button.Tag.ToString(), "CompareVersion", StringComparison.OrdinalIgnoreCase))
            {
                CompareVersion form = new CompareVersion();
                form.DevelopMode = true;
                form.Activated -= Win_Activated;
                form.Activated += Win_Activated;
                form.FormClosed += Win_FormClosed;
                form.FormClosed += Win_FormClosed;

                form.Show();
            }
            else
            {
                // Create an instance of the window named
                // by the current button.
                Type type = this.GetType();
                Assembly assembly = type.Assembly;

                Window win = (Window)assembly.CreateInstance(type.Namespace + "." + (string)button.Tag);
                win.Activated -= Win_Activated;
                win.Activated += Win_Activated;
                win.Unloaded -= Win_Unloaded;
                win.Unloaded += Win_Unloaded;

                // Show the window.
                win.Show();
            }
        }

        private void Win_Activated(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Win_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Show();
        }

        private void Win_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            this.Show();
        }
    }
}
