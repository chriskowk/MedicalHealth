﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;

using System.Xml;
using System.Threading;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Collections;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.Windows;
using SqlExtensionVB;
using System.Linq;
using System.Management;
using static System.Management.PropertyDataCollection;
using MyJob;
using System.Configuration;
using Microsoft.Win32;

namespace JobController
{
    public partial class Form1 : Form
    {
        private HandleMask _batch;
        private bool _isExiting = false;

        private System.Threading.Timer _timer;
        private class Emp
        {
            public Emp(int a) { Age = a; }
            public int Age;
        }

        public static string GetAppConfig(string strKey)
        {
            string file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);
            foreach (string key in config.AppSettings.Settings.AllKeys)
            {
                if (key == strKey)
                {
                    return config.AppSettings.Settings[strKey].Value.ToString();
                }
            }
            return null;
        }

        public Form1()
        {
            try
            {
                IList<Emp> lst = new List<Emp>();
                int cnt = lst.Count(a => a.Age == 0);

                string plaintext = UserDefinedFunctions.fnGetOriginalCode("N/a\\").Value;
                int passwordCode = UserDefinedFunctions.fnGetHashCode(plaintext);
                StringComparer sc = StringComparer.OrdinalIgnoreCase;
                int pc = sc.GetHashCode(plaintext);

                string result = string.Format("属于{0}，定额 {1:0.00} 元（费用已超定额）。", "PIC", 12345);

                string wrong1 = string.Format("{0:yyyy-MM-dd hh:MM}", System.DateTime.Now);
                string wrong2 = string.Format("{0:yyyy-MM-dd HH:MM}", System.DateTime.Now);
                string correct = string.Format("{0:yyyy-MM-dd HH:mm}", System.DateTime.Now);
                Console.WriteLine("{0}\t{1}\t{2}", wrong1, wrong2, correct);

                this.Top = (int)(SystemParameters.WorkArea.Height - this.Height - 105);
                this.Left = (int)(SystemParameters.WorkArea.Width - this.Width - 360);

                TaskJob.BASE_PATH = GetAppConfig("TaskJob");
                TaskJobA.BASE_PATH = GetAppConfig("TaskJobA");
                TaskJobB.BASE_PATH = GetAppConfig("TaskJobB");
                TaskJobC.BASE_PATH = GetAppConfig("TaskJobC");
                TaskJobD.BASE_PATH = GetAppConfig("TaskJobD");
                TaskJobE.BASE_PATH = GetAppConfig("TaskJobE");

                SetFilesWritable(Path.Combine(TaskJob.BASE_PATH, @"bin\Resources"));
                SetFilesWritable(Path.Combine(TaskJobA.BASE_PATH, @"bin\Resources"));
                SetFilesWritable(Path.Combine(TaskJobB.BASE_PATH, @"bin\Resources"));
                SetFilesWritable(Path.Combine(TaskJobC.BASE_PATH, @"bin\Resources"));
                SetFilesWritable(Path.Combine(TaskJobD.BASE_PATH, @"bin\Resources"));
                SetFilesWritable(Path.Combine(TaskJobE.BASE_PATH, @"bin\Resources"));

                SetFilesWritable(Path.Combine(TaskJob.BASE_PATH, @"\DataModel\Oracle"));
                SetFilesWritable(Path.Combine(TaskJobA.BASE_PATH, @"\DataModel\Oracle"));
                SetFilesWritable(Path.Combine(TaskJobB.BASE_PATH, @"\DataModel\Oracle"));
                SetFilesWritable(Path.Combine(TaskJobC.BASE_PATH, @"\DataModel\Oracle"));
                SetFilesWritable(Path.Combine(TaskJobD.BASE_PATH, @"\DataModel\Oracle"));
                SetFilesWritable(Path.Combine(TaskJobE.BASE_PATH, @"\DataModel\Oracle"));

                //NetworkCredential credential = new NetworkCredential("guoshaoyue", "netboy", "hissoft.com");//初始化用户  
                //TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri("http://svrdevelop:8080/tfs/medicalhealthsy"), credential);
                TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri("http://svrdevelop:8080/tfs/medicalhealthsy"), CredentialCache.DefaultCredentials);
                tpc.Authenticate();
                WorkItemStore workItemStore = tpc.GetService<WorkItemStore>();

                ////特有的Wilq查询，2008和2010还不一样  
                //// WorkItemCollection queryResults = workItemStore.Query(
                ////"Select *  From WorkItems Where [System.Teamproject]='ClinicalManagement'" +
                ////" and [System.WorkItemType] = 'Bug' and [System.State]='活动的' " +
                ////"Order By [System.State] Asc, [System.ChangedDate] Desc");

                //WorkItem workItem = queryResults[1];
                //Debug.Print(workItem.Fields["标题"].Value.ToString());
                //workItem.Fields["指派给"].Value = "宁美玲";

                ////验证工作项的各字段是否有效,如果save出错,则可通过此方式验证哪出错  
                //ArrayList ar = workItem.Validate();
                //foreach (var item in ar)
                //{
                //    Debug.Print(item.ToString());
                //}
                //workItem.Save();
                ////工作项的字段信息  
                //FieldCollection fl = workItem.Fields;
                //foreach (Field item in fl)
                //{
                //    Debug.Print(item.Name);
                //}

                bool newMutexCreated = false;
                string mutexName = "Global\\" + Assembly.GetExecutingAssembly().GetName().Name;
                Mutex mutex = null;
                try
                {
                    mutex = new Mutex(false, mutexName, out newMutexCreated);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.ToString());
                    Environment.Exit(1);
                }
                if (newMutexCreated)
                {
                    InitializeComponent();
                    lblCurState.Text = "作业已启动，启动时间：" + DateTime.Now;
                    _batch = new HandleMask();
                    _starting = DateTime.Now;
                    _batch.Start();
                }
                else
                {
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                Environment.Exit(1);
            }

            LoadResources();

            _timer = new System.Threading.Timer(new TimerCallback(ResetTrayIcon));
            _timer.Change(0, 1000);
        }

        private bool SaveWorkItem(string workItemIds, string projectName, out string builtWorkItemIDs)
        {
            WorkItemStore workItemStore;
            WorkItemCollection queryResults;
            WorkItem workItem;
            string updateSQL = string.Empty;

            //NetworkCredential credential = new NetworkCredential("guoshaoyue", "netboy", "hissoft.com");//初始化用户  
            //TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri("http://svrdevelop:8080/tfs/medicalhealthsy"), credential);
            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri("http://svrdevelop:8080/tfs/medicalhealthsy"), CredentialCache.DefaultCredentials);
            tpc.Authenticate();

            // [System.Title], [System.WorkItemType], [System.State], [System.ChangedDate], [System.Id]
            string base_sql = string.Format("Select * From WorkItems Where [System.TeamProject] = '{0}' ", projectName);
            string sql;
            string query = string.Format("select e.FullName, b.* from tBug b, tbEmployee e where b.CodeEmployeeID = e.EmployeeID and b.BugId in ( {0} )", workItemIds);
            DataSet ds = SqlDbHelper.Query(query);
            builtWorkItemIDs = string.Empty;
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
                builtWorkItemIDs = string.IsNullOrEmpty(builtWorkItemIDs) ? workItemId.ToString() : string.Format("{0}, {1}", builtWorkItemIDs, workItemId);
            }

            if (!string.IsNullOrEmpty(updateSQL))
            {
                if (SqlDbHelper.ExecuteSql(updateSQL) == 0) System.Windows.Forms.MessageBox.Show("更新JSDesk平台工作项失败！");
            }

            return true;
        }

        private void SetFilesWritable(string foldername)
        {
            if (!Directory.Exists(foldername)) return;

            DirectoryInfo folder = new DirectoryInfo(foldername);
            FileInfo[] fileInfos = folder.GetFiles("*", SearchOption.AllDirectories);
            foreach (FileInfo fi in fileInfos)
            {
                if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                {
                    fi.Attributes = FileAttributes.Normal;
                }
            }
        }

        private int _index = 0;
        private bool _dailyreset = false;
        private DateTime _date;
        private void ResetTrayIcon(object state)
        {
            Assembly a = Assembly.GetExecutingAssembly();
            string path = Environment.CurrentDirectory;
            _index = (_index + 1) % 7;

            this.notifyIcon1.Icon = _icons[string.Format("{0}.Tray{1}.ico", _trayIconPath, _index)] as Icon;
            RefreshDurationText();
            RewriteConfigAndRestartJob();
            CheckAllJobExecuteStates();
        }

        private IDictionary<string, string> _states = new Dictionary<string, string>();
        private void CheckAllJobExecuteStates()
        {
            CheckJobExecuteState(typeof(TaskJob).ToString());
            CheckJobExecuteState(typeof(TaskJobA).ToString());
            CheckJobExecuteState(typeof(TaskJobB).ToString());
            CheckJobExecuteState(typeof(TaskJobC).ToString());
            CheckJobExecuteState(typeof(TaskJobD).ToString());
            CheckJobExecuteState(typeof(TaskJobE).ToString());
        }

        private void CheckJobExecuteState(string jobType)
        {
            string temp = GetJobExecuteState(jobType);
            bool askuser = _states.ContainsKey(jobType) && _states[jobType] != temp && temp == "STATE_COMPLETE";
            _states[jobType] = temp;
            if (askuser)
            {
                System.Windows.Forms.MessageBox.Show(string.Format("{0} 任务刚结束！", jobType), "检查任务状态", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.ServiceNotification);
            }
        }

        private string GetJobExecuteState(string jobType)
        {
            object state = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\JetSun\3.0\Quartz\Job\" + jobType, "State", "");
            return state == null ? "" : state.ToString();
        }

        private void RewriteConfigAndRestartJob()
        {
            if (_date != DateTime.Now.Date)
            {
                _date = DateTime.Now.Date;
                _dailyreset = false;
            }
            if (!_dailyreset && DateTime.Now.Hour > 23)
            {
                _btnResetJobConfig_Click(this, new EventArgs());
                _dailyreset = true;
            }
        }

        private string _trayIconPath;
        private Hashtable _icons = new Hashtable();
        private void LoadResources()
        {
            Stream imgStream = null;
            Icon icon = null;
            int pos = 0;

            // get a reference to the current assembly
            Assembly a = Assembly.GetExecutingAssembly();

            // get a list of resource names from the manifest
            string[] resNames = a.GetManifestResourceNames();

            foreach (string s in resNames)
            {
                pos = s.IndexOf(".Tray");
                if (pos > -1 && s.EndsWith(".ico"))
                {
                    if (string.IsNullOrEmpty(_trayIconPath)) _trayIconPath = s.Substring(0, pos);
                    // attach to stream to the resource in the manifest
                    imgStream = a.GetManifestResourceStream(s);
                    if (imgStream != null)
                    {
                        // create a new bitmap from this stream and 
                        // add it to the arraylist
                        icon = new Icon(imgStream);
                        if (icon != null) _icons.Add(s, icon);

                        imgStream.Close();
                    }
                }
            }
        }

        private void _btnResetJobConfig_Click(object sender, EventArgs e)
        {
            _starting = DateTime.Now;
            WriteJobConfig("JobConfig.xml", "", DateTime.Now.Date.AddHours(21.5));
            WriteJobConfig("JobAConfig.xml", "A", DateTime.Now.Date.AddHours(21));
            WriteJobConfig("JobBConfig.xml", "B", DateTime.Now.Date.AddHours(20.5));
            WriteJobConfig("JobCConfig.xml", "C", DateTime.Now.Date.AddHours(20));
            WriteJobConfig("JobDConfig.xml", "D", DateTime.Now.Date.AddHours(19.5));
            WriteJobConfig("JobEConfig.xml", "E", DateTime.Now.Date.AddHours(19.5));
            _txtStatus.Text = string.Format("{0:yyyy-MM-dd HH:mm:ss}: 作业调度计划已重置。", DateTime.Now);

            Restart();
        }

        private DateTime _starting;
        private void tscbStart_Click(object sender, EventArgs e)
        {
            _starting = DateTime.Now;
            _dtpRestart.Value = _starting.AddMinutes(1);

            string filename = string.Format("Job{0}Config.xml", this.VersionTag);
            WriteJobConfig(filename, this.VersionTag, _dtpRestart.Value);

            Restart();
        }

        private string VersionTag
        {
            get { return VersionSelected.Tag == null ? string.Empty : VersionSelected.Tag.ToString(); }
        }

        private RadioButton VersionSelected
        {
            get
            {
                if (_rbZSYK.Checked)
                    return _rbZSYK;
                else if (_rbSY.Checked)
                    return _rbSY;
                else if (_rbS12.Checked)
                    return _rbS12;
                else if (_rbGH.Checked)
                    return _rbGH;
                else if (_rbS1.Checked)
                    return _rbS1;
                else if (_rbSGS1.Checked)
                    return _rbSGS1;
                return null;
            }
        }

        private bool _isCompiledPending = false;
        private void Restart()
        {
            _batch.Stop();
            _batch.Start();
            tscbStart.Enabled = false;
            tscbStop.Enabled = true;
            tscbExit.Enabled = true;

            lblCurState.ForeColor = Color.Black;
            lblCurState.Text = "作业已启动，启动时间：" + _starting;
            _isCompiledPending = true;
        }

        private void tscbStop_Click(object sender, EventArgs e)
        {
            _batch.Stop();
            tscbStart.Enabled = true;
            tscbStop.Enabled = false;
            tscbExit.Enabled = true;
            lblCurState.ForeColor = Color.Red;
            lblCurState.Text = "作业已暂停，停止时间：" + DateTime.Now;
            _isCompiledPending = false;
        }

        private void tscbExit_Click(object sender, EventArgs e)
        {
            _batch.Stop();
            _isExiting = true;
            System.Windows.Forms.Application.Exit();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.MinimizedToNormal();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isExiting)
            {
                e.Cancel = true;
                NormalToMinimized();
                return;
            }

            if (_timer != null) _timer.Dispose();
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            RefreshDurationText();

            _cboProjects.Items.Clear();
            //NetworkCredential credential = new NetworkCredential("guoshaoyue", "netboy", "hissoft.com");//初始化用户  
            //TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri("http://svrdevelop:8080/tfs/medicalhealthsy"), credential);
            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri("http://svrdevelop:8080/tfs/medicalhealthsy"), CredentialCache.DefaultCredentials);
            tpc.Authenticate();

            WorkItemStore workItemStore = tpc.GetService<WorkItemStore>();
            foreach (Project item in workItemStore.Projects)
            {
                if (!item.Name.StartsWith("CDSS")) _cboProjects.Items.Add(item.Name);
            }
            _cboProjects.SelectedIndex = 0;
            _dtpRestart.Value = DateTime.Now;
            if (_currentVersion == null)
                _rbZSYK.Checked = true;
            else
                _currentVersion.Checked = true;
        }

        private void RefreshDurationText()
        {
            try
            {
                TimeSpan ts = DateTime.Now.Subtract(_starting);
                lblDuration.Text = string.Format("{0}:{1}:{2}", ts.Hours.ToString("00"), ts.Minutes.ToString("00"), ts.Seconds.ToString("00"));
            }
            catch (Exception)
            {
            }
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            if (!_isExiting) NormalToMinimized();
        }

        private void MinimizedToNormal()
        {
            this.WindowState = FormWindowState.Normal;
            this.Visible = true;
            this.TopMost = true;
        }
        private void NormalToMinimized()
        {
            this.TopMost = false;
            this.Visible = false;
            this.WindowState = FormWindowState.Minimized;
        }

        private void _btnBuild_Click(object sender, EventArgs e)
        {
            string builtWorkItemIDs;
            string ids = _txtWorkItemIDs.Text.Replace(" ", "");
            ids = ids.Replace("、", ",");
            ids = ids.Replace(";", ",");
            if (SaveWorkItem(ids, _cboProjects.SelectedItem.ToString(), out builtWorkItemIDs))
            {
                _txtStatus.Text = string.Format("{0:yyyy-MM-dd HH:mm:ss}: 工作项（{1}）导入成功：{2}。", DateTime.Now, ids, builtWorkItemIDs);
            }
        }

        private bool WriteJobConfig(string fileName, string postfix, DateTime dt)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf - 8\" ?>");
            sw.WriteLine("<quartz xmlns=\"http://quartznet.sourceforge.net/JobSchedulingData\"");
            sw.WriteLine("    xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"");
            sw.WriteLine("    version=\"1.0\"");
            sw.WriteLine("    overwrite-existing-jobs=\"true\">");
            sw.WriteLine("    <job>");
            sw.WriteLine("      <job-detail>");
            sw.WriteLine("          <name>MyJob{0}</name>", postfix);
            sw.WriteLine("          <group>MyJob{0}</group>", postfix);
            sw.WriteLine("          <job-type>MyJob.TaskJob{0}, MyJob</job-type>", postfix);
            sw.WriteLine("      </job-detail>");
            sw.WriteLine("      <trigger>");
            sw.WriteLine("          <cron>");
            sw.WriteLine("            <name>cronMyJob{0}</name>", postfix);
            sw.WriteLine("            <group>cronMyJob{0}</group>", postfix);
            sw.WriteLine("            <job-name>MyJob{0}</job-name>", postfix);
            sw.WriteLine("            <job-group>MyJob{0}</job-group>", postfix);
            sw.WriteLine("            <!--秒 分 小时 月内日期 月 周内日期 年（可选字段）-->");
            sw.WriteLine("            <!--周一到周五每天的8点到20点，每一分钟触发一次-->");
            sw.WriteLine("            <!--<cron-expression>0 0/1 8-20 ? * MON-FRI</cron-expression>-->");
            sw.WriteLine("            <!--每天21点触发-->");
            sw.WriteLine("            <!--<cron-expression>0 0 21 ? * *</cron-expression>-->");
            sw.WriteLine("            <cron-expression>0 {0} {1} ? * *</cron-expression>", dt.Minute, dt.Hour);
            sw.WriteLine("          </cron>");
            sw.WriteLine("      </trigger>");
            sw.WriteLine("    </job>");
            sw.WriteLine("</quartz>");
            sw.Close();
            fs.Close();

            return true;
        }

        private void _btnRegYK_Click(object sender, EventArgs e)
        {
            ExecuteReg(Path.Combine(TaskJob.GetBatchFilePath(), @"注册表\眼科注册表.reg"));
            Execute(Path.Combine(TaskJob.GetBatchFilePath(), "__copy2svcbin.bat"), 0);
            RestartServices();
        }

        private void _tbnRegSY_Click(object sender, EventArgs e)
        {
            ExecuteReg(Path.Combine(TaskJobA.GetBatchFilePath(), @"注册表\省医注册表.reg"));
            Execute(Path.Combine(TaskJobA.GetBatchFilePath(), "__copy2svcbin.bat"), 0);
            RestartServices();
        }

        private void _btnRegS12_Click(object sender, EventArgs e)
        {
            ExecuteReg(Path.Combine(TaskJobB.GetBatchFilePath(), @"注册表\市十二注册表.reg"));
            Execute(Path.Combine(TaskJobB.GetBatchFilePath(), "__copy2svcbin.bat"), 0);
            RestartServices();
        }

        private void _btnRegGH_Click(object sender, EventArgs e)
        {
            ExecuteReg(Path.Combine(TaskJobC.GetBatchFilePath(), @"注册表\光华注册表.reg"));
            Execute(Path.Combine(TaskJobC.GetBatchFilePath(), "__copy2svcbin.bat"), 0);
            RestartServices();
        }

        private void _btnRegS1_Click(object sender, EventArgs e)
        {
            ExecuteReg(Path.Combine(TaskJobD.GetBatchFilePath(), @"注册表\市一注册表.reg"));
            Execute(Path.Combine(TaskJobD.GetBatchFilePath(), "__copy2svcbin.bat"), 0);
            RestartServices();
        }

        private void _btnRegSGS1_Click(object sender, EventArgs e)
        {
            ExecuteReg(Path.Combine(TaskJobE.GetBatchFilePath(), @"注册表\韶关市一注册表.reg"));
            Execute(Path.Combine(TaskJobE.GetBatchFilePath(), "__copy2svcbin.bat"), 0);
            RestartServices();
        }

        /// <summary>  
        /// 执行注册表导入  
        /// </summary>  
        /// <param name="regFile">注册表文件路径</param>  
        private void ExecuteReg(string regFile)
        {
            if (File.Exists(regFile))
            {
                //如果注册表路径含有空格，则需要使用双引号引起来，不然会报错。 /s：指示不弹出导入注册表对话框
                regFile = @"""" + regFile + @"""";
                Process.Start("regedit", string.Format(" /s {0}", regFile));
                _txtStatus.Text = string.Format("{0:yyyy-MM-dd HH:mm:ss}: 注册表导入成功：{1}。", DateTime.Now, regFile);
            }
        }

        /// <summary>  
        /// 执行DOS命令，返回DOS命令的输出  
        /// </summary>  
        /// <param name="dosCommand">dos命令</param>  
        /// <param name="milliseconds">等待命令执行的时间（单位：毫秒），  
        /// 如果设定为0，则无限等待</param>  
        /// <returns>返回DOS命令的输出</returns>  
        public static string Execute(string command, int seconds)
        {
            string output = ""; //输出字符串  
            if (command != null && !command.Equals(""))
            {
                Process process = new Process();//创建进程对象  
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";//设定需要执行的命令  
                startInfo.Arguments = "/C " + command;//“/C”表示执行完命令后马上退出  
                startInfo.UseShellExecute = true;//使用系统外壳程序启动  
                startInfo.RedirectStandardInput = false;//不重定向输入  
                startInfo.RedirectStandardOutput = false; //不重定向输出  
                startInfo.CreateNoWindow = false;//创建窗口  
                process.StartInfo = startInfo;
                try
                {
                    if (process.Start())//开始进程  
                    {
                        if (seconds == 0)
                        {
                            process.WaitForExit();//这里无限等待进程结束  
                        }
                        else
                        {
                            process.WaitForExit(seconds); //等待进程结束，等待时间为指定的毫秒  
                        }
                        output = process.StandardOutput.ReadToEnd();//读取进程的输出  
                    }
                }
                catch
                {
                }
                finally
                {
                    if (process != null)
                        process.Close();
                }
            }
            return output;
        }

        private void _btnStartServices_Click(object sender, EventArgs e)
        {
            RestartServices();
        }

        private void RestartServices()
        {
            string servicename = "jssvcl";
            string strWql = string.Format("SELECT PathName FROM Win32_Service where Name ='{0}'", servicename);
            string path = GetResultByWql(strWql);
            if (string.IsNullOrWhiteSpace(path))
            {
                System.Windows.Forms.MessageBox.Show("服务已卸载，请重新安装服务");
                //return;
            }

            //string new_path = RegistryHelper.GetValue(RegistryHelper.LocalServiceKey, string.Empty).ToString();
            string new_path = Path.Combine(this.JssvcFilePath, "jssvc.exe");
            string batFilePath = Path.Combine(Environment.CurrentDirectory, "_$RestartLocalService");
            string batchCommand = string.Format("{0} \"{1}\" \"{2}\"", batFilePath, path, new_path);
            Execute(batchCommand, 0);
        }

        private string JssvcFilePath
        {
            get
            {
                switch (this.VersionTag)
                {
                    case "":
                        return TaskJob.GetJSSVCFilePath();
                    case "A":
                        return TaskJobA.GetJSSVCFilePath();
                    case "B":
                        return TaskJobB.GetJSSVCFilePath();
                    case "C":
                        return TaskJobC.GetJSSVCFilePath();
                    case "D":
                        return TaskJobD.GetJSSVCFilePath();
                    case "E":
                        return TaskJobE.GetJSSVCFilePath();
                    default:
                        return TaskJob.GetJSSVCFilePath();
                }
            }
        }

        private static string GetResultByWql(string wql)
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher(wql);
            foreach (ManagementObject mo in mos.Get())
            {
                PropertyDataEnumerator oEnum = (mo.Properties.GetEnumerator() as PropertyDataEnumerator);
                while (oEnum.MoveNext())
                {
                    PropertyData prop = (PropertyData)oEnum.Current;
                    if (prop.Value != null) return prop.Value.ToString();
                }
            }
            return null;
        }

        private RadioButton _currentVersion;
        private void _optCustomer_CheckedChanged(object sender, EventArgs e)
        {
            _currentVersion = sender as RadioButton;
        }
    }
}
