using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;
using TFSideKicks.Configuration;
using TFSideKicks.Helpers;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TFSideKicks
{
    /// <summary>
    /// CollectionHistory.xaml 的交互逻辑
    /// </summary>
    public partial class ShowHistory : Window
    {
        public ShowHistory()
        {
            InitializeComponent();
        }
        private IDictionary<TfsTeamProjectCollectionUri, IList<Project>> _projects = new Dictionary<TfsTeamProjectCollectionUri, IList<Project>>();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (TfsUrlElement item in ConfigHelper.ProjectCollection)
            {
                _cboTfsUris.Items.Add(new TfsTeamProjectCollectionUri(item.VersionName, item.TfsUrl));
            }

            string location = Assembly.GetExecutingAssembly().Location;
            FileInfo fi = new FileInfo(location);
            this.Title = string.Format("查看历史记录 Built-{0:yyyyMMdd.HH.mm}", fi.LastWriteTime);
            if (_cboTfsUris.Items.Count > 0) { _cboTfsUris.SelectedIndex = 0; }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (_cboTfsUris.Items.Count > 0) { _cboTfsUris.SelectedIndex = 0; }
        }

        private void _btnShowHistory_Click(object sender, RoutedEventArgs e)
        {
            TfsTeamProjectCollectionUri uri = _cboTfsUris.SelectedItem as TfsTeamProjectCollectionUri;
            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri(uri.Value), CredentialCache.DefaultNetworkCredentials);
            tpc.Authenticate();

            VersionControlServer version = tpc.GetService(typeof(VersionControlServer)) as VersionControlServer;
            //查询历史版本
            var histories = version.QueryHistory("$/", VersionSpec.Latest, 0, RecursionType.Full, null, null, null, 18, true, false);
            _tbStatus.Text = "查询历史记录结果：";
            //遍历路径下的内容的所有历史版本
            foreach (Changeset change in histories)//每个历史版本下修改了几个文件
            {
                _tbStatus.Text += string.Format("\r\n{0} {1} {2:yyyy-MM-dd HH:mm:ss}: {3}", change.ChangesetId, change.CommitterDisplayName, change.CreationDate, change.Comment);
            }

        }

        private void _btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
