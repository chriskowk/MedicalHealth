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

namespace TFSideKicks
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private const string _HKEY_LOCAL_MACHINE = "HKEY_LOCAL_MACHINE";
        private static string _tfsUrisKeyPath = @"SOFTWARE\JetSun\3.0\Quartz\TfsTeamProjectCollectionUris";
        private IDictionary<TfsTeamProjectCollectionUri, IList<Project>> _projects = new Dictionary<TfsTeamProjectCollectionUri, IList<Project>>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RegistryKey rk = RegistryHelper.GetRegistryKey(_tfsUrisKeyPath);
            if (rk != null)
            {
                foreach (string name in rk.GetValueNames())
                {
                    object obj = RegistryHelper.GetRegistryValue(_tfsUrisKeyPath, name);
                    _cboTfsUris.Items.Add(new TfsTeamProjectCollectionUri(name, obj == null ? string.Empty : obj.ToString()));
                }
                rk.Close();
            }

            string str1 = Process.GetCurrentProcess().MainModule.FileName;
            string location = Assembly.GetExecutingAssembly().Location;
            FileInfo fi = new FileInfo(location);
            this.Title = string.Format("导入工作项 Built-{0:yyyyMMdd.HH.mm}", fi.LastWriteTime);
            if (_cboTfsUris.Items.Count > 0) { _cboTfsUris.SelectedIndex = 0; }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (_cboTfsUris.Items.Count > 0) { _cboTfsUris.SelectedIndex = 0; }
        }

        private void RefreshProjectComboItems(TfsTeamProjectCollectionUri item)
        {
            IList<Project> projs = BuildProjectItems(item);
            _cboProjects.Items.Clear();
            foreach (Project p in projs)
            {
                _cboProjects.Items.Add(p.Name);
            }
            _cboProjects.SelectedIndex = 0;
        }

        private void _cboTfsUris_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            TfsTeamProjectCollectionUri uri = _cboTfsUris.SelectedItem as TfsTeamProjectCollectionUri;
            RefreshProjectComboItems(uri);
        }

        private IList<Project> BuildProjectItems(TfsTeamProjectCollectionUri uri)
        {
            IList<Project> projs = new List<Project>();
            if (!_projects.ContainsKey(uri))
            {
                NetworkCredential credential = CredentialCache.DefaultNetworkCredentials; //初始化用户  
                TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri(uri.Value), credential);
                tpc.Authenticate();

                //运行路径（E:\VSTS\VS2015\ImportWorkItems\TFSideKicks\bin\Debug）下必须存在如下文件：Microsoft.WITDataStore64.dll，否则报错。另外“生成”Any CPU；去掉勾选“首选32位”选项
                WorkItemStore workItemStore = tpc.GetService<WorkItemStore>();
                foreach (Project item in workItemStore.Projects)
                {
                    if (!item.Name.StartsWith("CDSS")) projs.Add(item);
                }
                _projects[uri] = projs;
            }
            return _projects[uri];
        }

        private void _btnSave_Click(object sender, RoutedEventArgs e)
        {
            string ids = _txtWorkItemIDs.Text.Replace(" ", "");
            ids = ids.Replace("、", ",");
            ids = ids.Replace(";", ",");
            if (SaveWorkItem(ids, _cboProjects.SelectedItem.ToString()))
            {
                _tbStatus.Text = string.Format("{0:yyyy-MM-dd HH:mm:ss}: 工作项（{1}）导入成功。", DateTime.Now, ids);
            }
        }

        private void _btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private bool SaveWorkItem(string workItemIds, string projectName)
        {
            WorkItemStore workItemStore;
            WorkItemCollection queryResults;
            WorkItem workItem;
            string updateSQL = string.Empty;

            TfsTeamProjectCollectionUri uri = _cboTfsUris.SelectedItem as TfsTeamProjectCollectionUri;
            NetworkCredential credential = CredentialCache.DefaultNetworkCredentials; //初始化用户  
            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri(uri.Value), credential);
            tpc.Authenticate();

            // [System.Title], [System.WorkItemType], [System.State], [System.ChangedDate], [System.Id]
            string base_sql = string.Format("Select * From WorkItems Where [System.TeamProject] = '{0}' ", projectName);
            string sql;
            string query = string.Format("select e.FullName, b.* from tBug b, tbEmployee e where b.CodeEmployeeID = e.EmployeeID and b.BugId in ( {0} )", workItemIds);
            DataSet ds = SqlDbHelper.Query(query);
            string rets = string.Empty;
            foreach (DataRowView item in ds.Tables[0].DefaultView)
            {
                Debug.Print(item["BugID"].ToString());
                sql = string.Format("{0} and [System.Title] = '{1}'", base_sql, item["BugID"].ToString());
                workItemStore = tpc.GetService<WorkItemStore>();
                queryResults = workItemStore.Query(sql);
                int cnt = queryResults.Count;
                if (cnt > 0)
                {
                    workItem = queryResults[0];
                    if (!workItem.IsOpen) workItem.Open();
                }
                else
                {
                    Project project = workItemStore.Projects[projectName];
                    workItem = new WorkItem(int.Parse(item["CustomerCaseID"].ToString()) == -1 ? project.WorkItemTypes["Bug"] : project.WorkItemTypes["任务"]);
                    workItem.Fields["团队项目"].Value = projectName;
                    workItem.Fields["标题"].Value = item["BugID"].ToString();
                }
                if (int.Parse(item["CustomerCaseID"].ToString()) == -1)
                {
                    workItem.Fields["重现步骤"].Value = item["CaseDesc"].ToString();
                }
                else
                {
                    workItem.Fields["说明"].Value = item["CaseDesc"].ToString();
                }
                workItem.Fields["指派给"].Value = item["FullName"].ToString();
                ArrayList ar = workItem.Validate();
                workItem.Save();

                int workItemId = int.Parse(workItem.Fields["ID"].Value.ToString());
                string s = string.Format("UPDATE tBug SET TFSWorkItemID = {0} WHERE BugID = {1};", workItemId, item["BugID"]);
                updateSQL = string.IsNullOrEmpty(updateSQL) ? s : string.Format("{0}\r\n {1}", updateSQL, s);
                rets = string.IsNullOrEmpty(rets) ? workItem.Fields["ID"].Value.ToString() : string.Format("{0}, {1}", rets, workItem.Fields["ID"].Value);
            }
            _txtTFSWorkItemIDs.Text = rets;
            if (!string.IsNullOrEmpty(updateSQL))
            {
                if (SqlDbHelper.ExecuteSql(updateSQL) == 0) MessageBox.Show("更新JSDesk平台工作项失败！");
            }

            return true;
        }
    }
}
