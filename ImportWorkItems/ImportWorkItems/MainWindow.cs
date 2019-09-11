using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ImportWorkItems
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private const string _HKEY_LOCAL_MACHINE = "HKEY_LOCAL_MACHINE";
        private static string _tfsUrisKeyPath = @"SOFTWARE\JetSun\3.0\Quartz\TfsTeamProjectCollectionUris";
        private IDictionary<TfsTeamProjectCollectionUri, IList<Project>> _projects = new Dictionary<TfsTeamProjectCollectionUri, IList<Project>>();
        private bool _formLoaded = false;
        private void MainWindow_Load(object sender, EventArgs e)
        {
            if (_formLoaded) return;

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
            string location = Assembly.GetExecutingAssembly().Location;
            FileInfo fi = new FileInfo(location);
            this.Text = string.Format("导入工作项 Built-{0:yyyyMMdd.HH.mm}", fi.LastWriteTime);
            if (_cboTfsUris.Items.Count > 0) { _cboTfsUris.SelectedIndex = 0; }

            _formLoaded = true;
        }

        private void MainWindow_Activated(object sender, EventArgs e)
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

        private void _cboTfsUris_SelectedIndexChanged(object sender, EventArgs e)
        {
            TfsTeamProjectCollectionUri uri = _cboTfsUris.SelectedItem as TfsTeamProjectCollectionUri;
            RefreshProjectComboItems(uri);
        }

        private IList<Project> BuildProjectItems(TfsTeamProjectCollectionUri uri)
        {
            IList<Project> projs = new List<Project>();
            if (!_projects.ContainsKey(uri))
            {
                TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri(uri.Value), CredentialCache.DefaultNetworkCredentials);
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

        private void _btnSave_Click(object sender, EventArgs e)
        {
            string ids = _txtWorkItemIDs.Text.Replace(" ", "");
            ids = ids.Replace("、", ",");
            ids = ids.Replace(";", ",");
            if (SaveWorkItem(ids, _cboProjects.SelectedItem.ToString()))
            {
                _tbStatus.Text = string.Format("{0:yyyy-MM-dd HH:mm:ss}: 工作项（{1}）导入成功。", DateTime.Now, ids);
            }
        }
        private bool SaveWorkItem(string workItemIds, string projectName)
        {
            WorkItemStore workItemStore;
            WorkItemCollection queryResults;
            WorkItem workItem;
            string updateSQL = string.Empty;

            TfsTeamProjectCollectionUri uri = _cboTfsUris.SelectedItem as TfsTeamProjectCollectionUri;
            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri(uri.Value), CredentialCache.DefaultNetworkCredentials);
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
                if (SqlDbHelper.ExecuteSql(updateSQL) == 0) System.Windows.Forms.MessageBox.Show("更新JSDesk平台工作项失败！");
            }

            return true;
        }

        private void _btnClose_Click(object sender, EventArgs e)
        {
            //Environment.Exit(0);
            //只能关闭窗口，不能使用上面语句（退出进程），否则会卡死
            this.Close();
        }

        private void btnRetrieve_Click(object sender, EventArgs e)
        {
            if (!IsNumberic(_txtVersionID.Text)) return;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("declare @s varchar(4000), @ids varchar(4000);");
            sb.AppendLine("set @s = ''; set @ids = '<SYSTEMVERSIONID>';");
            sb.AppendLine("select @s = @s + ', ' + convert(varchar(10), BugID) from tBug where SystemVersionID in (select ID from dbo.fnIDInString(@IDs) );");
            sb.AppendLine("select substring(@s, 3, len(@s)) BugIDs;"); 

            _txtWorkItemIDs.Text = string.Empty;
            string query = sb.ToString().Replace("<SYSTEMVERSIONID>", _txtVersionID.Text);
            DataSet ds = SqlDbHelper.Query(query);
            string rets = string.Empty;
            foreach (DataRowView item in ds.Tables[0].DefaultView)
            {
                _txtWorkItemIDs.AppendText(item["BugIDs"].ToString());
            }
        }

        private bool IsNumberic(string num)
        {
            double ret;
            bool result = double.TryParse(num, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out ret);
            return result;
        }
    }
}
