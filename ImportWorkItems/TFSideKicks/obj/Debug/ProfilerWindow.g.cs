﻿#pragma checksum "..\..\ProfilerWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A53C9A96AD9FCBE2808F987F4F615CCD"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using TFSideKicks;


namespace TFSideKicks {
    
    
    /// <summary>
    /// ProfilerWindow
    /// </summary>
    public partial class ProfilerWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 46 "..\..\ProfilerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tb_source;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\ProfilerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tb_log;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\ProfilerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tb_user;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\ProfilerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox tb_password;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\ProfilerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cb_save;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\ProfilerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tb_oraname;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\ProfilerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cb_curruser;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\ProfilerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tb_module;
        
        #line default
        #line hidden
        
        
        #line 57 "..\..\ProfilerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button1;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\ProfilerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button2;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\ProfilerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dg_SQLlines;
        
        #line default
        #line hidden
        
        
        #line 74 "..\..\ProfilerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tb_Status;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/TFSideKicks;component/profilerwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ProfilerWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\ProfilerWindow.xaml"
            ((TFSideKicks.ProfilerWindow)(target)).Activated += new System.EventHandler(this.Window_Activated);
            
            #line default
            #line hidden
            return;
            case 2:
            this.tb_source = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.tb_log = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.tb_user = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.tb_password = ((System.Windows.Controls.PasswordBox)(target));
            return;
            case 6:
            this.cb_save = ((System.Windows.Controls.CheckBox)(target));
            
            #line 52 "..\..\ProfilerWindow.xaml"
            this.cb_save.Click += new System.Windows.RoutedEventHandler(this.cb_save_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.tb_oraname = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.cb_curruser = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 9:
            this.tb_module = ((System.Windows.Controls.TextBox)(target));
            return;
            case 10:
            this.button1 = ((System.Windows.Controls.Button)(target));
            
            #line 57 "..\..\ProfilerWindow.xaml"
            this.button1.Click += new System.Windows.RoutedEventHandler(this.button1_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.button2 = ((System.Windows.Controls.Button)(target));
            
            #line 58 "..\..\ProfilerWindow.xaml"
            this.button2.Click += new System.Windows.RoutedEventHandler(this.button2_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.dg_SQLlines = ((System.Windows.Controls.DataGrid)(target));
            
            #line 60 "..\..\ProfilerWindow.xaml"
            this.dg_SQLlines.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this._dgSQLlines_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 13:
            this.tb_Status = ((System.Windows.Controls.TextBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

