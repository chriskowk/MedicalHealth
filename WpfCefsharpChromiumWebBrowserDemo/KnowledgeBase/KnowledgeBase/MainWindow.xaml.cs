﻿using System;
using System.Windows;
using System.Windows.Threading;
using CefSharp;
using KnowledgeBase.ViewModels;

namespace KnowledgeBase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// MainWindow Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            AppViewModel.RegisterTestResources(browser);
            DataContext = new AppViewModel();
            browser.StatusMessage += Browser_StatusMessage;
            browser.NavStateChanged += Browser_NavStateChanged;
        }

        /// <summary>
        /// Is called on change of browser load state and notifies the statusbar
        /// to say 'Loading...' or 'Loading done.'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Browser_NavStateChanged(object sender, NavStateChangedEventArgs e)
        {
            // Do this on the UI thread since it otherwise throws an exception ...
            Dispatcher.BeginInvoke(new Action(() => { Status.Text = e.CanReload ? "Loading done." : "Loading..."; }), DispatcherPriority.Background);
        }

        private void Browser_StatusMessage(object sender, StatusMessageEventArgs e)
        {
            // Do this on the UI thread since it otherwise throws an exception ...
            Dispatcher.BeginInvoke(new Action(() => { Status.Text = e.Value; }), DispatcherPriority.Background);
        }
    }
}