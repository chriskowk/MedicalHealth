using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;
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
using System.Configuration;
using Microsoft.Win32;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using JobController.Configuration;
using System.Data.SqlTypes;
using MyJob;
using Quartz;

namespace JobController
{
    public partial class MainForm : Form
    {
        private HandleMask _batch;
        private bool _isExiting = false;

        private System.Threading.Timer _timer;
        private class Emp
        {
            public Emp(int a) { Age = a; }
            public int Age;
        }

        public MainForm()
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

                foreach (SchedulerElement item in ConfigHelper.SchedulerCollection)
                {
                    SetFilesWritable(Path.Combine(item.BasePath, @"bin\Resources"));
                    SetFilesWritable(Path.Combine(item.BasePath, @"\DataModel\Oracle"));
                }

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
                    Debug.Print(ex.ToString());
                    Environment.Exit(1);
                }
                if (newMutexCreated)
                {
                    InitializeComponent();
                    CreateCustomerRadioButtons();
                    BindControlEvents(Controls);

                    // 注意：this.Width和this.Height只有在InitializeComponent()初始化UI控件之后才能正确获取
                    this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height;
                    this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;

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
            OnReceivedTaskFinishedMessage();
            GlobalEventManager.JobWasExecuted += GlobalEventManager_JobWasExecuted;

            _timer = new System.Threading.Timer(new TimerCallback(ResetTrayIcon));
            _timer.Change(0, 1000);
        }

        private void GlobalEventManager_JobWasExecuted(object sender, EventArgs e)
        {
            JobKey jobKey = sender as JobKey;
            if (jobKey != null)
                ShowTaskFinishedMessage(jobKey.Name);
        }

        private static ConnectionFactory _connectionFactory;
        private static ConnectionFactory ConnectionFactory
        {
            get
            {
                if (_connectionFactory == null)
                    _connectionFactory = new ConnectionFactory
                    {
                        HostName = "localhost",//RabbitMQ服务在本地运行
                        UserName = "guest",//用户名
                        Password = "guest"//密码 
                    };

                return _connectionFactory;
            }
        }

        private static readonly IConnection _connection = ConnectionFactory.CreateConnection();
        private static readonly IModel _channel = _connection.CreateModel();
        private static EventingBasicConsumer _consumer = null;
        private void OnReceivedTaskFinishedMessage()
        {
            _channel.QueueDeclare(queue: "TaskFinishedMessage", durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.ExchangeDeclare(exchange: "TaskFinishedMessageExchange", type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: "TaskFinishedMessage", exchange: "TaskFinishedMessageExchange", routingKey: string.Empty, arguments: null);
            try
            {
                if (_consumer == null) { _consumer = new EventingBasicConsumer(_channel); }
                _consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    message = $"【{ConfigHelper.GetCustomerName(message)}】版本编译已完成";
                    string errMsg = string.Empty;
                    string path = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "MessageBox.exe");
                    JobHelper.ExecBatch(path, false, false, false, $"{message.Replace(" ", "{SPACE}")}", ref errMsg);
                };
                _channel.BasicConsume(queue: "TaskFinishedMessage",
                                     autoAck: true,
                                     consumer: _consumer);
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter(@"E:\err_msg.log", true, Encoding.UTF8))
                {
                    sw.WriteLine($"接收消息队列异常:{ex.Message}-{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
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

        private void ShowTaskFinishedMessage(string jobname)
        {
            System.Windows.Forms.MessageBox.Show(string.Format("【{0}】 编译任务刚结束！", ConfigHelper.GetCustomerName(jobname)), "检查任务状态", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.ServiceNotification);
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
            _btnResetJobConfig.Enabled = false;
            _starting = DateTime.Now;
            int i = 19;
            foreach (SchedulerElement item in ConfigHelper.SchedulerCollection)
            {
                WriteJobConfig(item.SchedulerFile, item.JobName, item.TypeName, item.CustomerName, DateTime.Now.Date.AddHours(i++));
            }

            Delay(10000);
            Restart();
            _txtStatus.Text = string.Format("{0:yyyy-MM-dd HH:mm:ss}: 作业调度计划已重置。", DateTime.Now);
            _btnResetJobConfig.Enabled = true;
        }

        public static void Delay(int mm)
        {
            DateTime current = DateTime.Now;
            while (current.AddMilliseconds(mm) > DateTime.Now)
            {
                System.Windows.Forms.Application.DoEvents();
            }
            return;
        }

        private DateTime _starting;
        private void tscbStart_Click(object sender, EventArgs e)
        {
            _starting = DateTime.Now;
            if (VersionSelected.Tag is SchedulerElement se)
            {
                _dtpRestart.Value = _starting.AddMinutes(1);
                WriteJobConfig(se.SchedulerFile, se.JobName, se.TypeName, se.CustomerName, _dtpRestart.Value);
                Restart();
            }
        }

        private void tscbStop_Click(object sender, EventArgs e)
        {
            _batch.Stop();
            tscbStart.Enabled = true;
            tscbStop.Enabled = false;
            tscbExit.Enabled = true;
            lblCurState.ForeColor = Color.Red;
            lblCurState.Text = "作业已暂停，停止时间：" + DateTime.Now;
        }

        private void tscbExit_Click(object sender, EventArgs e)
        {
            _batch.Stop();
            _isExiting = true;
            System.Windows.Forms.Application.Exit();
        }

        private RadioButton VersionSelected
        {
            get
            {
                foreach (Control item in panCustomers.Controls)
                {
                    if (item is RadioButton rdo && rdo.Checked) { return rdo; }
                }
                return null;
            }
        }

        private RadioButton DefaultCustomer
        {
            get
            {
                foreach (Control item in panCustomers.Controls)
                {
                    if (item is RadioButton rdo) { return rdo; }
                }
                return null;
            }
        }

        private void Restart()
        {
            _batch.Stop();
            _batch.Start();
            tscbStart.Enabled = false;
            tscbStop.Enabled = true;
            tscbExit.Enabled = true;

            lblCurState.ForeColor = Color.Black;
            lblCurState.Text = "作业已启动，启动时间：" + _starting;
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.MinimizedToNormal();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isExiting)
            {
                e.Cancel = true;
                NormalToMinimized();
                return;
            }

            if (_timer != null) _timer.Dispose();
        }

        private void MainForm_Activated(object sender, EventArgs e)
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
                DefaultCustomer.Checked = true;
            else
                _currentVersion.Checked = true;
        }

        private void RefreshDurationText()
        {
            try
            {
                TimeSpan ts = DateTime.Now.Subtract(_starting);
                this.Invoke(new Action(() => { lblDuration.Text = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}"; }));
            }
            catch (Exception)
            {
            }
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
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

        private void WriteJobConfig(string fileName, string jobname, string typename, string customer, DateTime dt)
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
            sw.WriteLine($"      <name>{jobname}</name>");
            sw.WriteLine("      <!--group(选填) 任务所属分组，用于标识任务所属分组-->");
            sw.WriteLine($"      <group>{jobname}Group</group>");
            sw.WriteLine($"      <description>{customer}版本编译任务</description>");
            sw.WriteLine("      <!--job-type(必填)任务的具体类型及所属程序集，格式：实现了IJob接口的包含完整命名空间的类名,程序集名称-->");
            sw.WriteLine($"      <job-type>MyJob.{typename}, MyJob</job-type>");
            sw.WriteLine("      <durable>true</durable>");
            sw.WriteLine("      <recover>false</recover>");
            sw.WriteLine("    </job>");
            sw.WriteLine("    <trigger>");
            sw.WriteLine("      <cron>");
            sw.WriteLine("        <!--name(必填) 触发器名称，同一个分组中的名称必须不同-->");
            sw.WriteLine($"        <name>{jobname}Trigger</name>");
            sw.WriteLine("        <!--group(选填) 触发器组-->");
            sw.WriteLine($"        <group>{jobname}TriggerGroup</group>");
            sw.WriteLine("        <!--job-name(必填) 要调度的任务名称，该job-name必须和对应job节点中的name完全相同-->");
            sw.WriteLine($"        <job-name>{jobname}</job-name>");
            sw.WriteLine("        <!--job-group(选填) 调度任务(job)所属分组，该值必须和job中的group完全相同-->");
            sw.WriteLine($"        <job-group>{jobname}Group</job-group>");
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
            if (VersionSelected.Tag is SchedulerElement se)
            {
                string bat1 = Path.Combine(se.BasePath, $"BatchFiles\\注册表\\{se.CustomerName}注册表.reg");
                string bat2 = Path.Combine(se.BasePath, $"BatchFiles\\__copy2svcbin.bat");
                ExecuteReg(bat1);
                Execute(bat2, 0);
                RestartServices();
            }
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
        public static void Execute(string command, int seconds)
        {
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
                            process.WaitForExit();//这里无限等待进程结束  
                        else
                            process.WaitForExit(seconds); //等待进程结束，等待时间为指定的毫秒  
                    }
                }
                catch { }
                finally
                {
                    if (process != null)
                        process.Close();
                }
            }
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
                System.Windows.Forms.MessageBox.Show("服务已卸载，请重新安装服务");

            if (VersionSelected.Tag is SchedulerElement se)
            {
                string new_path = Path.Combine(se.BasePath, "Lib\\jssvc.exe");
                string batFilePath = Path.Combine(Environment.CurrentDirectory, "_$RestartLocalService");
                string batchCommand = string.Format("{0} \"{1}\" \"{2}\"", batFilePath, path, new_path);
                Execute(batchCommand, 0);
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
        private void CustomerRadio_CheckedChanged(object sender, EventArgs e)
        {
            _currentVersion = sender as RadioButton;
        }

        private void CreateCustomerRadioButtons()
        {
            var xStep = 0;
            var yStep = 0;
            int width = lblRef.Width * 2;
            int height = (int)(lblRef.Height * 1.5);
            int px = 10;
            int py = 0;
            int i = 0;
            foreach (SchedulerElement item in ConfigHelper.SchedulerCollection)
            {
                if (i % 3 == 0)
                {
                    xStep = 0;
                    yStep += lblRef.Height * 2;
                }
                var rdo = new RadioButton
                {
                    Text = item.CustomerName,
                    Name = "rdoCustomer" + i,
                    Tag = item,
                    Size = new System.Drawing.Size(width, height),
                    Location = new System.Drawing.Point(px + width * xStep, py + yStep)
                };
                panCustomers.Controls.Add(rdo);
                xStep++;
                i++;
            }
        }

        /// <summary>
        /// 为控件绑定事件
        /// </summary>
        private void BindControlEvents(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control.HasChildren)
                {
                    BindControlEvents(control.Controls);
                }
                else
                {
                    if (control is RadioButton rdo)
                    {
                        rdo.CheckedChanged -= CustomerRadio_CheckedChanged;
                        rdo.CheckedChanged += CustomerRadio_CheckedChanged;
                    }
                }
            }
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
            foreach (DataRowView item in ds.Tables[0].DefaultView)
            {
                _txtWorkItemIDs.AppendText(item["BugIDs"].ToString());
            }
        }

        private bool IsNumberic(string num)
        {
            bool result = double.TryParse(num, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out _);
            return result;
        }
    }
}
