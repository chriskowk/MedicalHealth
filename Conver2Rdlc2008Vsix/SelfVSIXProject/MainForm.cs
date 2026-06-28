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
using System.Xml;
using System.Windows.Forms;
using Microsoft.Win32;
using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio.Shell;
using EnvDTE;

namespace SelfVSIXProject
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.Activated -= MainForm_Activated;
            this.Activated += MainForm_Activated;
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (Package.GetGlobalService(typeof(DTE)) is DTE dte && dte.ActiveDocument != null)
            {
                string fullPath = dte.ActiveDocument.FullName;
                _txtFileName.Text = fullPath;
            }
        }

        private void _btnConvert_Click(object sender, EventArgs e)
        {
            DoConvert2Rdlc2008(_txtFileName.Text);
        }

        private void DoConvert2Rdlc2008(string fullfilename)
        {
            string destinationFilePath = $@"C:\Temp\output{DateTime.Now:yyyyMMddHHmmss}.rdlc";
            int startline = 0;
            int idx = 0;
            try
            {
                if (!String.IsNullOrEmpty(fullfilename))
                {
                    if (File.Exists(fullfilename))
                    {
                        string extension = Path.GetExtension(fullfilename);
                        if (!extension.Equals(".rdlc", StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBox.Show("不能转换非 RDLC 文件！", "DoConvert2Rdlc2008");
                            return;
                        }

                        using (StreamWriter writer = new StreamWriter(destinationFilePath))
                        {
                            // 读取源文件
                            using (StreamReader reader = new StreamReader(fullfilename))
                            {
                                string line;
                                while ((line = reader.ReadLine()) != null)
                                {
                                    idx++;

                                    // 判断行是否包含特定字符
                                    if (line.Contains("/reporting/2016/01/"))
                                    {
                                        // 2016替换为2008
                                        writer.WriteLine(line.Replace("/reporting/2016/01/", "/reporting/2008/01/"));
                                    }
                                    else if (line.Contains("ReportSection"))
                                    {
                                        //不复制报表参数SECTION标记行<ReportSections>, <ReportSection>, </ReportSection>, </ReportSections>
                                    }
                                    else if (line.Contains("<ReportParametersLayout>"))
                                    {
                                        startline = idx;
                                    }
                                    else if (line.Contains("</ReportParametersLayout>"))
                                    {
                                        startline = 0;
                                    }
                                    else if (startline > 0)
                                    {
                                        //不复制报表参数布局块
                                    }
                                    else
                                    {
                                        //复制文本行
                                        writer.WriteLine(line);
                                    }
                                }
                            }
                        }

                        MessageBox.Show("Successfully convert to Rdlc2008!", "Convert2Rdlc2008 Successful");
                        System.Diagnostics.Process.Start("notepad.exe", destinationFilePath);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a RDLC file", "DoConvert2Rdlc2008");
                }
            }
            catch (Exception)
            {
            }
            finally
            {
            }
        }

        private void _btnClose_Click(object sender, EventArgs e)
        {
            //Application.Exit();
            //Environment.Exit(0);
            //只能关闭窗口，不能使用上面语句（退出进程），否则会卡死
            if (this.InvokeRequired)
                this.Invoke((MethodInvoker)delegate
                {
                    this.Close();
                });
            else
            {
                this.Close();
            }
        }
    }
}
