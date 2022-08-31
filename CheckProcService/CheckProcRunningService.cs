using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CheckProcService
{
    public partial class CheckProcRunningService : ServiceBase
    {
        public CheckProcRunningService()
        {
            InitializeComponent();
        }

        private const int _MIN_INTERVAL = 60000;
        protected override void OnStart(string[] args)
        {
            CommandLineArguments arguments = new CommandLineArguments(args);
            ConfigHelper.Arguments = arguments;

            System.Threading.ThreadStart job = new System.Threading.ThreadStart(StartJob);
            System.Threading.Thread thread = new System.Threading.Thread(job);
            thread.Start();
            EventLog.WriteEntry(this.ServiceName, string.Format("CheckProcRunningService -period:{0} -procfilename:{1}", this.Period.ToString(), this.ProcFileName), EventLogEntryType.Information);
            return;
        }

        private static int? _period;
        public int Period
        {
            get
            {
                if (!_period.HasValue)
                {
                    int period = _MIN_INTERVAL;
                    //EventLog.WriteEntry(this.ServiceName, string.Format("-period:{0}", ConfigHelper.Arguments.Parameters.ContainsKey("Period")), EventLogEntryType.Warning);
                    if (ConfigHelper.Arguments.Parameters.ContainsKey("Period"))
                        int.TryParse(ConfigHelper.Arguments.Parameters["Period"], out period);
                    _period = period;
                }
                return _period.Value;
            }
        }

        private static string _procfilename;
        public string ProcFileName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_procfilename))
                {
                    string fn = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "trayicondemo.exe");
                    if (ConfigHelper.Arguments.Parameters.ContainsKey("ProcFileName"))
                        fn = ConfigHelper.Arguments.Parameters["ProcFileName"];
                    _procfilename = fn;
                }
                return _procfilename;
            }
        }

        protected override void OnStop()
        {
            if (_timer != null) _timer.Dispose();
        }

        private Timer _timer;
        private void StartJob()
        {
            try
            {
                EventLog.WriteEntry(this.ServiceName, string.Format("Starting on {0} Paramters[Period: {1} ProcFileName:{2}]", DateTime.Now, this.Period.ToString(), this.ProcFileName), EventLogEntryType.Information);

                _timer = new Timer(new TimerCallback(ExecCheckRunJob));
                _timer.Change(0, this.Period);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(this.ServiceName, ex.Message, EventLogEntryType.Warning);
            }
        }

        private void ExecCheckRunJob(object state)
        {
            //string errMsg = string.Empty;
            //ProcessHelper.ExecBatch(fn, false, false, false, "", ref errMsg);

            // the name of the application to launch;
            // to launch an application using the full command path simply escape
            // the path with quotes, for example to launch firefox.exe:
            //      String applicationName = "\"C:\\Program Files (x86)\\Mozilla Firefox\\firefox.exe\"";
            //string fn = "cmd.exe";

            // launch the application
            ApplicationLoader.PROCESS_INFORMATION procInfo;
            ApplicationLoader.StartProcessAndBypassUAC(this.ProcFileName, out procInfo);
        }
    }
}
