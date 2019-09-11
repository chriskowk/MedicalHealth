using System;
using System.Collections.Generic;
using System.Text;

using Quartz;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace MyJob
{
    public interface IExecWithEvent
    {
        /// <summary>
        /// 
        /// </summary>
        event EventHandler<EventArgs> JobStarting;
        /// <summary>
        /// 
        /// </summary>
        event EventHandler<EventArgs> JobFinished;
    }

    public abstract class TaskJobBase : IJob, IExecWithEvent
    {
        public abstract string BasePath { get; }
        public abstract string BatchFilesPath { get; }
        public abstract string JSSVCFilePath { get; }

        public void Execute(JobExecutionContext context)
        {
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\JetSun\3.0\Quartz\Job\" + this.GetType().ToString(), "State", "STATE_START");
            if (JobStarting != null) JobStarting(this, new EventArgs());

            //todo:此处为执行的任务
            TFGetLatestVersion();
            BuildAll();
            RebuildDataModels();

            if (JobFinished != null) JobFinished(this, new EventArgs());
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\JetSun\3.0\Quartz\Job\" + this.GetType().ToString(), "State", "STATE_COMPLETE");
        }

        private void TFGetLatestVersion()
        {
            //string path = string.Format(@"{0}\TF_GET_MedicalHealth.bat", BatchFilesPath);
            string path = Path.Combine(BatchFilesPath, "TF_GET_MedicalHealth.bat");
            if (!File.Exists(path)) return;

            string errMsg = string.Empty;
            string output = JobHelper.ExecBatch(path, true, true, 60000, ref errMsg);

            FileStream fs = new FileStream(string.Format(@"{0}\Log\TFGetLog{1}.txt", BatchFilesPath, DateTime.Now.ToString("yyyyMMddHHmmss")), FileMode.Append);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write(string.Format("Output: {0}\r\nErrorMsg:\r\n{1}", output, errMsg));
            sw.Close();
            fs.Close();
        }

        private void BuildAll()
        {
            //string path = string.Format(@"{0}\全编译Upload.bat", BatchFilesPath);
            string path = Path.Combine(BatchFilesPath, "全编译Upload.bat");
            if (!File.Exists(path)) return;

            string errMsg = string.Empty;
            string output = JobHelper.ExecBatch(path, false, false, ref errMsg);
        }

        public virtual void RebuildDataModels()
        {
        }

        public event EventHandler<EventArgs> JobStarting;

        public event EventHandler<EventArgs> JobFinished;
    }

    public class TaskJob : TaskJobBase
    {
        public static string BASE_PATH;
        public override string BasePath
        {
            get { return BASE_PATH; }
        }
        public static string GetBatchFilePath()
        {
            return Path.Combine(BASE_PATH, "BatchFiles"); ;
        }
        public static string GetJSSVCFilePath()
        {
            return Path.Combine(BASE_PATH, "Lib");
        }

        public override string BatchFilesPath
        {
            get { return GetBatchFilePath(); }
        }

        public override string JSSVCFilePath
        {
            get { return GetJSSVCFilePath(); }
        }

        public override void RebuildDataModels()
        {
            base.RebuildDataModels();
            string path = Path.Combine(BatchFilesPath, "tempcmd.bat");
            if (!File.Exists(path)) return;

            string errMsg = string.Empty;
            string output = JobHelper.ExecBatch(path, false, false, ref errMsg);
        }
    }

    public class TaskJobA : TaskJobBase
    {
        public static string BASE_PATH;
        public override string BasePath
        {
            get { return BASE_PATH; }
        }
        public static string GetBatchFilePath()
        {
            return Path.Combine(BASE_PATH, "BatchFiles"); ;
        }
        public static string GetJSSVCFilePath()
        {
            return Path.Combine(BASE_PATH, "Lib");
        }

        public override string BatchFilesPath
        {
            get { return GetBatchFilePath(); }
        }

        public override string JSSVCFilePath
        {
            get { return GetJSSVCFilePath(); }
        }
    }

    public class TaskJobB : TaskJobBase
    {
        public static string BASE_PATH;
        public override string BasePath
        {
            get { return BASE_PATH; }
        }
        public static string GetBatchFilePath()
        {
            return Path.Combine(BASE_PATH, "BatchFiles"); ;
        }
        public static string GetJSSVCFilePath()
        {
            return Path.Combine(BASE_PATH, "Lib");
        }

        public override string BatchFilesPath
        {
            get { return GetBatchFilePath(); }
        }

        public override string JSSVCFilePath
        {
            get { return GetJSSVCFilePath(); }
        }
    }


    public class TaskJobC : TaskJobBase
    {
        public static string BASE_PATH;
        public override string BasePath
        {
            get { return BASE_PATH; }
        }
        public static string GetBatchFilePath()
        {
            return Path.Combine(BASE_PATH, "BatchFiles"); ;
        }
        public static string GetJSSVCFilePath()
        {
            return Path.Combine(BASE_PATH, "Lib");
        }

        public override string BatchFilesPath
        {
            get { return GetBatchFilePath(); }
        }

        public override string JSSVCFilePath
        {
            get { return GetJSSVCFilePath(); }
        }
    }

    public class TaskJobD : TaskJobBase
    {
        public static string BASE_PATH;
        public override string BasePath
        {
            get { return BASE_PATH; }
        }
        public static string GetBatchFilePath()
        {
            return Path.Combine(BASE_PATH, "BatchFiles"); ;
        }
        public static string GetJSSVCFilePath()
        {
            return Path.Combine(BASE_PATH, "Lib");
        }

        public override string BatchFilesPath
        {
            get { return GetBatchFilePath(); }
        }

        public override string JSSVCFilePath
        {
            get { return GetJSSVCFilePath(); }
        }
    }

    public class TaskJobE : TaskJobBase
    {
        public static string BASE_PATH;
        public override string BasePath
        {
            get { return BASE_PATH; }
        }
        public static string GetBatchFilePath()
        {
            return Path.Combine(BASE_PATH, "BatchFiles"); ;
        }
        public static string GetJSSVCFilePath()
        {
            return Path.Combine(BASE_PATH, "Lib");
        }

        public override string BatchFilesPath
        {
            get { return GetBatchFilePath(); }
        }

        public override string JSSVCFilePath
        {
            get { return GetJSSVCFilePath(); }
        }
    }

    public static class JobHelper
    {
        public static string ExecBatch(string batPath, bool redirectStandardOutput, bool redirectStandardError, ref string errMsg)
        {
            return ExecBatch(batPath, redirectStandardOutput, redirectStandardError, 0, ref errMsg);
        }

        public static string ExecBatch(string batPath, bool redirectStandardOutput, bool redirectStandardError, int milliseconds, ref string errMsg)
        {
            string outputString = string.Empty;
            using (Process pro = new Process())
            {
                FileInfo file = new FileInfo(batPath);
                pro.StartInfo.WorkingDirectory = file.Directory.FullName;
                pro.StartInfo.FileName = batPath;
                pro.StartInfo.CreateNoWindow = false;
                pro.StartInfo.RedirectStandardOutput = redirectStandardOutput;
                pro.StartInfo.RedirectStandardError = redirectStandardError;
                pro.StartInfo.UseShellExecute = false;

                pro.Start();
                if (milliseconds <= 0)
                    pro.WaitForExit();
                else
                    pro.WaitForExit(milliseconds);

                outputString = pro.StartInfo.RedirectStandardOutput ? pro.StandardOutput.ReadToEnd() : string.Empty;
                errMsg = pro.StartInfo.RedirectStandardError ? pro.StandardError.ReadToEnd() : string.Empty;
            }
            return outputString;
        }
    }
}
