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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VsixDeskTools
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void MainWindow_Load(object sender, EventArgs e)
        {
            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri("http://svrdevelop:8080/tfs/medicalhealthsy"), CredentialCache.DefaultCredentials);
            tpc.Authenticate();

            string str1 = Process.GetCurrentProcess().MainModule.FileName;
            string str2 = Environment.CurrentDirectory;

            //运行路径（E:\VSTS\VS2015\ImportWorkItems\TFSideKicks\bin\Debug）下必须存在如下文件：Microsoft.WITDataStore64.dll，否则报错。另外“生成”Any CPU；去掉勾选“首选32位”选项
            WorkItemStore workItemStore = tpc.GetService<WorkItemStore>();
            _cboProject.Items.Clear();
            foreach (Project item in workItemStore.Projects)
            {
                if (!item.Name.StartsWith("CDSS")) _cboProject.Items.Add(item.Name);
            }
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            _cboProject.SelectedIndex = 0;
        }

        private void _btnSave_Click(object sender, EventArgs e)
        {
            string ids = _txtWorkItemIDs.Text.Replace(" ", "");
            ids = ids.Replace("、", ",");
            ids = ids.Replace(";", ",");
            if (SaveWorkItem(ids, _cboProject.SelectedItem.ToString()))
            {
                _tbStatus.Text = string.Format("{0:yyyy-MM-dd HH:mm:ss}: 工作项（{1}）导入成功。", DateTime.Now, ids);
            }
        }
        private bool SaveWorkItem(string workItemIds, string projectName)
        {
            WorkItemStore workItemStore;
            WorkItemCollection queryResults;
            WorkItem workItem;
            NetworkCredential credential = new NetworkCredential("guoshaoyue", "netboy", "hissoft.com");//初始化用户  
            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri("http://svrdevelop:8080/tfs/medicalhealthsy"), credential);
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

                rets = string.IsNullOrEmpty(rets) ? workItem.Fields["ID"].Value.ToString() : string.Format("{0}, {1}", rets, workItem.Fields["ID"].Value);
            }
            _txtTFSWorkItemIDs.Text = rets;

            return true;
        }

        private void _btnClose_Click(object sender, EventArgs e)
        {
            //Environment.Exit(0);
            //只能关闭窗口，不能使用上面语句（退出进程），否则会卡死
            this.Close();
        }
    }
}
