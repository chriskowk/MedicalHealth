using System;
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
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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
            LoadExecutablePath();
            //OnReceivedTaskFinishedMessage();

            _timer = new System.Threading.Timer(new TimerCallback(ResetTrayIcon));
            _timer.Change(0, 1000);

            GlobalEventManager.JobWasExecuted += GlobalEventManager_JobWasExecuted;
        }

        private void GlobalEventManager_JobWasExecuted(object sender, EventArgs e)
        {
            ShowTaskFinishedMessage(sender.ToString());
        }

        private void OnReceivedTaskFinishedMessage()
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";//RabbitMQ服务在本地运行
            factory.UserName = "guest";//用户名
            factory.Password = "guest";//密码 
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "TaskFinishedMessage", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.ExchangeDeclare(exchange: "TaskFinishedMessageExchange", type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);
                    channel.QueueBind(queue: "TaskFinishedMessage", exchange: "TaskFinishedMessageExchange", routingKey: string.Empty, arguments: null);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body.ToArray());
                        ShowTaskFinishedMessage(message);
                    };
                    channel.BasicConsume(queue: "TaskFinishedMessage",
                                 autoAck: true,
                                 consumer: consumer);
                }
            }
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
            _index = (_index + 1) % 7;
            this.notifyIcon1.Icon = _icons[string.Format("{0}.Tray{1}.ico", _trayIconPath, _index)] as Icon;
            RefreshDurationText();
            RewriteConfigAndRestartJob();
        }

        private void ShowTaskFinishedMessage(string jobType)
        {
            ResetExecutePath();
            System.Windows.Forms.MessageBox.Show(string.Format("【{0}】 编译任务刚结束！", GetAppConfig(jobType)), "检查任务状态", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.ServiceNotification);
        }

        private string _executePath;
        private void LoadExecutablePath()
        {
            object path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\JetSun\3.0\", "ExecutablePath", "");
            _executePath = path == null ? "" : path.ToString();
            _txtStatus.Text = string.Format("{0}注册表项 ExecutablePath: {1}", string.IsNullOrEmpty(_txtStatus.Text) ? string.Empty : _txtStatus.Text + "\r\n", _executePath);
        }

        private void ResetExecutePath()
        {
            if (_executePath.Contains(@"\MedicalHealth\"))
                ExecuteReg(Path.Combine(TaskJob.GetBatchFilePath(), @"注册表\眼科注册表.reg"));
            else if (_executePath.Contains(@"\MedicalHealthSY\"))
                ExecuteReg(Path.Combine(TaskJobA.GetBatchFilePath(), @"注册表\省医注册表.reg"));
            else if (_executePath.Contains(@"\MedicalHealthBasicRC\"))
                ExecuteReg(Path.Combine(TaskJobB.GetBatchFilePath(), @"注册表\市十二注册表.reg"));
            else if (_executePath.Contains(@"\MedicalHealthGH\"))
                ExecuteReg(Path.Combine(TaskJobC.GetBatchFilePath(), @"注册表\光华注册表.reg"));
            else if (_executePath.Contains(@"\MedicalHealthS1\"))
                ExecuteReg(Path.Combine(TaskJobD.GetBatchFilePath(), @"注册表\市一注册表.reg"));
            else if (_executePath.Contains(@"\MedicalHealthSGS1\"))
                ExecuteReg(Path.Combine(TaskJobE.GetBatchFilePath(), @"注册表\韶关市一注册表.reg"));
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
            Stream imgStream;
            Icon icon;
            int pos;

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

            string vt = this.VersionTag.Replace("MyJob.TaskJob", string.Empty);
            string filename = string.Format("Job{0}Config.xml", vt);
            WriteJobConfig(filename, vt, _dtpRestart.Value);

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
            TimeSpan ts = DateTime.Now.Subtract(_starting);
            this.Invoke(new Action(() => { lblDuration.Text = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}"; }));
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
                _txtStatus.Text = string.Format("{0:HH:mm:ss}: 工作项（{1}）导入成功：{2}。", DateTime.Now, ids, builtWorkItemIDs);
            }
        }

        private void WriteJobConfig(string fileName, string tag, DateTime dt)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sw.WriteLine("<job-scheduling-data xmlns=\"http://quartznet.sourceforge.net/JobSchedulingData\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" version=\"2.0\">");
            sw.WriteLine("  <processing-directives>");
            sw.WriteLine("    <overwrite-existing-data>true</overwrite-existing-data>");
            sw.WriteLine("  </processing-directives>");
            sw.WriteLine("  <schedule>");
            sw.WriteLine("    <job>");
            sw.WriteLine("      <!--name(必填)同一个group中多个job的name不能相同，若未设置group则所有未设置group的job为同一个分组-->");
            sw.WriteLine($"      <name>TaskJob{tag}</name>");
            sw.WriteLine("      <!--group(选填) 任务所属分组，用于标识任务所属分组-->");
            sw.WriteLine($"      <group>TaskJob{tag}Group</group>");
            sw.WriteLine("      <description>省医编译任务</description>");
            sw.WriteLine("      <!--job-type(必填)任务的具体类型及所属程序集，格式：实现了IJob接口的包含完整命名空间的类名,程序集名称-->");
            sw.WriteLine($"      <job-type>MyJob.TaskJob{tag}, MyJob</job-type>");
            sw.WriteLine("      <durable>true</durable>");
            sw.WriteLine("      <recover>false</recover>");
            sw.WriteLine("    </job>");
            sw.WriteLine("    <trigger>");
            sw.WriteLine("      <cron>");
            sw.WriteLine("        <!--name(必填) 触发器名称，同一个分组中的名称必须不同-->");
            sw.WriteLine($"        <name>TaskJob{tag}Trigger</name>");
            sw.WriteLine("        <!--group(选填) 触发器组-->");
            sw.WriteLine($"        <group>TaskJob{tag}TriggerGroup</group>");
            sw.WriteLine("        <!--job-name(必填) 要调度的任务名称，该job-name必须和对应job节点中的name完全相同-->");
            sw.WriteLine($"        <job-name>TaskJob{tag}</job-name>");
            sw.WriteLine("        <!--job-group(选填) 调度任务(job)所属分组，该值必须和job中的group完全相同-->");
            sw.WriteLine($"        <job-group>TaskJob{tag}Group</job-group>");
            sw.WriteLine("        <misfire-instruction>DoNothing</misfire-instruction>");
            sw.WriteLine("        <!--秒 分 小时 月内日期 月 周内日期 年（可选字段）-->");
            sw.WriteLine("        <!--周一到周五每天的8点到20点，每一分钟触发一次-->");
            sw.WriteLine("        <!--<cron-expression>0 0/1 8-20 ? * MON-FRI</cron-expression>-->");
            sw.WriteLine("        <!--周一到周五每天的16点45分触发-->");
            sw.WriteLine("        <!--<cron-expression>0 45 16 ? * MON-FRI</cron-expression>-->");
            sw.WriteLine("        <!--每天16点45分触发-->");
            sw.WriteLine("        <!--<cron-expression>0 45 16 ? * *</cron-expression>-->");
            sw.WriteLine("        <!-- 每隔5秒执行一次：*/5 * * * * ?");
            sw.WriteLine("        每隔1分钟执行一次：0 */1 * * * ?");
            sw.WriteLine("        每天23点执行一次：0 0 23 * * ?");
            sw.WriteLine("        每天凌晨1点执行一次：0 0 1 * * ?");
            sw.WriteLine("        每月1号凌晨1点执行一次：0 0 1 1 * ?");
            sw.WriteLine("        每月最后一天23点执行一次：0 0 23 L * ?");
            sw.WriteLine("        每周星期天凌晨1点实行一次：0 0 1 ? * L");
            sw.WriteLine("        在26分、29分、33分执行一次：0 26,29,33 * * * ?");
            sw.WriteLine("        每天的0点、13点、18点、21点都执行一次：0 0 0,13,18,21 * * ? -->");
            sw.WriteLine($"        <cron-expression>0 {dt.Minute} {dt.Hour} * * ?</cron-expression>");
            sw.WriteLine("      </cron>");
            sw.WriteLine("    </trigger>");
            sw.WriteLine("  </schedule>");
            sw.WriteLine("</job-scheduling-data>");
            sw.Close();
            fs.Close();
        }

        private void _btnRegConfig_Click(object sender, EventArgs e)
        {
            switch (this.VersionTag)
            {
                case "MyJob.TaskJob":
                    ExecuteReg(Path.Combine(TaskJob.GetBatchFilePath(), @"注册表\眼科注册表.reg"));
                    Execute(Path.Combine(TaskJob.GetBatchFilePath(), "__copy2svcbin.bat"), 0);
                    break;
                case "MyJob.TaskJobA":
                    ExecuteReg(Path.Combine(TaskJobA.GetBatchFilePath(), @"注册表\省医注册表.reg"));
                    Execute(Path.Combine(TaskJobA.GetBatchFilePath(), "__copy2svcbin.bat"), 0);
                    break;
                case "MyJob.TaskJobB":
                    ExecuteReg(Path.Combine(TaskJobB.GetBatchFilePath(), @"注册表\市十二注册表.reg"));
                    Execute(Path.Combine(TaskJobB.GetBatchFilePath(), "__copy2svcbin.bat"), 0);
                    break;
                case "MyJob.TaskJobC":
                    ExecuteReg(Path.Combine(TaskJobC.GetBatchFilePath(), @"注册表\光华注册表.reg"));
                    Execute(Path.Combine(TaskJobC.GetBatchFilePath(), "__copy2svcbin.bat"), 0);
                    break;
                case "MyJob.TaskJobD":
                    ExecuteReg(Path.Combine(TaskJobD.GetBatchFilePath(), @"注册表\市一注册表.reg"));
                    Execute(Path.Combine(TaskJobD.GetBatchFilePath(), "__copy2svcbin.bat"), 0);
                    break;
                case "MyJob.TaskJobE":
                    ExecuteReg(Path.Combine(TaskJobE.GetBatchFilePath(), @"注册表\韶关市一注册表.reg"));
                    Execute(Path.Combine(TaskJobE.GetBatchFilePath(), "__copy2svcbin.bat"), 0);
                    break;
            }
            LoadExecutablePath();
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
                this.Invoke(new Action(() => { _txtStatus.Text = string.Format("{0:HH:mm:ss}: 注册表导入成功：{1}。", DateTime.Now, regFile); }));
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
                    case "MyJob.TaskJob":
                        return TaskJob.GetJSSVCFilePath();
                    case "MyJob.TaskJobA":
                        return TaskJobA.GetJSSVCFilePath();
                    case "MyJob.TaskJobB":
                        return TaskJobB.GetJSSVCFilePath();
                    case "MyJob.TaskJobC":
                        return TaskJobC.GetJSSVCFilePath();
                    case "MyJob.TaskJobD":
                        return TaskJobD.GetJSSVCFilePath();
                    case "MyJob.TaskJobE":
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
