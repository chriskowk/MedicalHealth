﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using TFSideKicks.Helpers;

namespace TFSideKicks
{
    /// <summary>
    /// FileDateModifier.xaml 的交互逻辑
    /// </summary>
    public partial class FileDateModifier : Window
    {
        public FileDateModifier()
        {
            InitializeComponent();

            _btnDecrypt.IsEnabled = false;
            if (ConfigHelper.Arguments.Parameters.ContainsKey("IsDecryptEnabled"))
            {
                bool enabled;
                bool.TryParse(ConfigHelper.Arguments["IsDecryptEnabled"], out enabled);
                _btnDecrypt.IsEnabled = enabled;
            }
            this.DataContext = new FileDateModifierViewMode() { CreatedChecked = true, AccessedChecked = true, ModifiedChecked = true };
        }

        private void _btnClose_Click(object sender, RoutedEventArgs e)
        {
            //WinRARHelper.Decompress("F:\\FDiskTemp\\MedicalHealthSYCode.rar", "F:\\FDiskTemp\\MedicalHealthSYCode");
            this.Close();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            _txtSelectedPath.Focus();
        }
    }
}
