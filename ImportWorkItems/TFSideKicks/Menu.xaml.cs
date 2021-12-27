using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
            //bool ret = CheckIdentifyCard("350501202202297032");
        }

        private static DateTime? GetDateTime(int year, int month, int day)
        {
            DateTime? dt;
            try
            {
                dt = new DateTime(year, month, day);
                return dt;
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }

        }

        private static string _IdPattern18 = @"^[1-9]\d{5}(18|19|([23]\d))\d{2}((0[1-9])|(10|11|12))(([0-2][1-9])|10|20|30|31)\d{3}[0-9Xx]$";
        private static string _IdPattern15 = @"^[1-9]\d{5}\d{2}((0[1-9])|(10|11|12))(([0-2][1-9])|10|20|30|31)\d{2}[0-9Xx]$";
        public static bool CheckIdentifyCard(string idCardStr)
        {
            if (idCardStr.Length == 15)
            {
                if (!Regex.IsMatch(idCardStr, _IdPattern15))
                {
                    MessageBox.Show("15位身份证号不符合标准", "提示");
                    return false;
                }

                int year = int.Parse(idCardStr.Substring(6, 2)) + 1900;
                int month;
                int day;
                string monthStr = idCardStr.Substring(8, 2);
                string dayStr = idCardStr.Substring(10, 2);
                if (monthStr.Substring(0, 1) == "0")
                {
                    monthStr = monthStr.Substring(1, 1);
                }
                if (dayStr.Substring(0, 1) == "0")
                {
                    dayStr = dayStr.Substring(1, 1);
                }
                month = int.Parse(monthStr);
                day = int.Parse(dayStr);
                if (GetDateTime(year, month, day) == null)
                {
                    MessageBox.Show("15位身份证号不符合标准", "提示");
                    return false;
                }
            }
            else if (idCardStr.Length == 18)
            {
                if (!Regex.IsMatch(idCardStr, _IdPattern18))
                {
                    MessageBox.Show("18位身份证号不符合标准", "提示");
                    return false;
                }

                int year = int.Parse(idCardStr.Substring(6, 4));
                int month;
                int day;
                string monthStr = idCardStr.Substring(10, 2);
                string dayStr = idCardStr.Substring(12, 2);
                if (monthStr.Substring(0, 1) == "0")
                {
                    monthStr = monthStr.Substring(1, 1);
                }
                if (dayStr.Substring(0, 1) == "0")
                {
                    dayStr = dayStr.Substring(1, 1);
                }
                month = int.Parse(monthStr);
                day = int.Parse(dayStr);
                if (GetDateTime(year, month, day) == null)
                {
                    MessageBox.Show("18位身份证号不符合标准", "提示");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("身份证号必须为15或18位", "提示");
                return false;
            }

            return true;
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
