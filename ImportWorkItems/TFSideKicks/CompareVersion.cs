using SharpCompress.Archive;
using SharpCompress.Archive.Zip;
using SharpCompress.Common;
using SharpCompress.Reader;
using SharpCompress.Writer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TFSideKicks
{
    public partial class CompareVersion : Form
    {
        private const string COMPONENT_FILENAME = "component.xls";
        private const string RESOURCE_FILENAME = "resource.rar";
        private const string DBSCRIPT_FILENAME = "dbscript.xls";
        private const string TEMP_PATH = "temp";
        IList<Component> _components = new List<Component>();
        IList<DBScript> _dbscripts = new List<DBScript>();
        IList<string> _resources = new List<string>();
        public bool DevelopMode = false;
        public CompareVersion()
        {
            InitializeComponent();
            txtExecutePath.Text = RegistryHelper.GetExecutablePath();
            lblStatus.Text = string.Empty;
            lblStatus2.Text = string.Empty;
            SetTextBoxBackColor(chkVersion.Checked);
            string rkv = RegistryHelper.GetValue(RegistryHelper.IsServerKey, "false").ToString();
            chkIsWcfServer.Checked = bool.Parse(rkv);
            rkv = RegistryHelper.GetValue(RegistryHelper.IsDebugKey, "false").ToString();
            chkIsWcfServer.Enabled = bool.Parse(rkv);

            ToolTip tip = new ToolTip();
            tip.ShowAlways = true;
            tip.SetToolTip(btnFolder, "使用手工选定路径进行比较");

            ToolTip tip1 = new ToolTip();
            tip1.ShowAlways = true;
            tip1.SetToolTip(btnLocalService, "使用本地服务路径进行比较");

            ToolTip tip2 = new ToolTip();
            tip2.ShowAlways = true;
            tip2.SetToolTip(btnExcel, "根据指定提交版本导出组件和脚本EXCEL文件");

            ToolTip tip3 = new ToolTip();
            tip3.ShowAlways = true;
            tip3.SetToolTip(btnRar, "打包指定文件夹的所有文件");
        }

        private void Desktop_Load(object sender, EventArgs e)
        {
            chkVersion.Visible = this.DevelopMode;
            txtVersionID.Visible = this.DevelopMode;
            btnExcel.Visible = this.DevelopMode;
            btnRar.Visible = this.DevelopMode;
        }

        private void SetTextBoxBackColor(bool enabled)
        {
            txtVersionID.Enabled = enabled;
            txtVersionID.BackColor = enabled ? Color.White : lblStatus.BackColor;
            btnExcel.Enabled = enabled;
        }

        private bool LoadComponents(bool fromDB)
        {
            int versionId;
            if (fromDB == false)
            {
                DataTable dt = GetExcelDataTable(DBSCRIPT_FILENAME);
                if (dt != null) LoadDBScripts(dt);

                dt = GetExcelDataTable(COMPONENT_FILENAME);
                if (dt != null)
                    return LoadComponents(dt);
                else
                    MessageBox.Show(string.Format("请确认组件文件是否存在！\r\n{0}", COMPONENT_FILENAME), "版本比较");
            }
            else
            {
                if (int.TryParse(txtVersionID.Text, out versionId))
                {
                    LoadDBScripts(GetDBScriptDataTable(versionId), true);
                    return LoadComponents(GetComponentDataTable(versionId), true);
                }
                else
                    MessageBox.Show("请输入正确的版本号！", "版本比较", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return false;
        }

        private DataTable GetExcelDataTable(string filename)
        {
            if (!File.Exists(filename)) return null;

            DataTable dt = Runtimes.ReadExcelToTable(filename);
            return dt;
        }

        private DataTable GetComponentDataTable(int versionId)
        {
            string sql = string.Format("select ComponentID, SourcePath, FileSize, CompileDateTime from tSystemVersionComponent where systemversionid = {0}", versionId);
            DataSet ds = SqlDbHelper.Query(sql);

            return ds.Tables[0];
        }

        private DataTable GetDBScriptDataTable(int versionId)
        {
            string sql = string.Format("select distinct l.DBScriptID, e.FullName, s.Description, s.Note, f.Description as BranchVersion, s.LastedModifyDateTime from dbo.tSystemVersionDBScriptList l, dbo.tDBScript s, dbo.tbEmployee e, tsFlagDefine f where l.SystemVersionID={0} and l.DBScriptID = s.DBScriptID and s.CreateEmployeeID=e.EmployeeID and s.BranchFlag=f.Flag and f.FieldName='BranchFlag' order by l.DBScriptID", versionId);
            DataSet ds = SqlDbHelper.Query(sql);

            return ds.Tables[0];
        }

        private bool LoadComponents(DataTable dt, bool isFromDB = false)
        {
            try
            {
                _components.Clear();
                foreach (DataRowView row in dt.DefaultView)
                {
                    string path = row["SourcePath"].ToString();
                    string extension = Path.GetExtension(path);
                    if (extension.Equals(".dll", StringComparison.OrdinalIgnoreCase) || extension.Equals(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        int id = int.Parse(row["ComponentID"].ToString());
                        string filename = Path.GetFileName(path);
                        int filesize = int.Parse(row["FileSize"].ToString());
                        DateTime compiledatetime = DateTime.Parse(row["CompileDateTime"].ToString());
                        _components.Add(new Component() { ComponentID = id, FileName = filename, FileSize = filesize, CompileDateTime = compiledatetime });
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                string sourcetips = isFromDB ? "访问平台数据库失败！" : string.Format("读取{0}文件Sheet1数据失败！\r\n调整工作表标题行的高度保存后再试！", DBSCRIPT_FILENAME);
                MessageBox.Show(string.Format("{0}\r\n{1}", sourcetips, ex.Message), "LoadDBScripts", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool LoadDBScripts(DataTable dt, bool isFromDB = false)
        {
            try
            {
                _dbscripts.Clear();
                foreach (DataRowView row in dt.DefaultView)
                {
                    int id = int.Parse(row["DBScriptID"].ToString());
                    DateTime lastedmodifiedon = DateTime.Parse(row["LastedModifyDateTime"].ToString());
                    _dbscripts.Add(new DBScript() { DBScriptID = id, CreateEmployeeName = row["FullName"].ToString(), Description = row["Description"].ToString(), Note = row["Note"].ToString(), BranchVersion = row["BranchVersion"].ToString(), LastedModifyDateTime = lastedmodifiedon });
                }
                lvwDBScript.Items.Clear();
                foreach (var item in _dbscripts.OrderBy(a => a.DBScriptID))
                {
                    lvwDBScript.Items.Add(new ListViewItem(new string[] { item.DBScriptID.ToString(), item.CreateEmployeeName, item.Description, item.Note, item.BranchVersion, item.LastedModifyDateTime.ToString("yyyy-MM-dd HH:mm:ss") }));
                }
                return true;
            }
            catch (Exception ex)
            {
                string sourcetips = isFromDB ? "访问平台数据库失败！" : string.Format("读取{0}文件Sheet1数据失败！\r\n调整工作表标题行的高度保存后再试！", COMPONENT_FILENAME);
                MessageBox.Show(string.Format("{0}\r\n{1}", sourcetips, ex.Message), "LoadDBScripts", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void CheckFileVersion()
        {
            IList<Component> components;
            lblStatus.Text = string.Empty;
            if (!LoadComponents(chkVersion.Checked)) return;

            string foldername = txtExecutePath.Text;
            if (!Directory.Exists(foldername)) return;

            DirectoryInfo folder = new DirectoryInfo(foldername);
            FileInfo[] fileInfos = folder.GetFiles("*", SearchOption.AllDirectories);
            if (chkIsWcfServer.Checked)
                components = _components;
            else
                components = _components.Where(a => !a.FileName.EndsWith(".ServiceLibrary.dll", StringComparison.OrdinalIgnoreCase)).ToList();

            lvwComponent.Items.Clear();
            if (components.Count == 0)
            {
                lblStatus.ForeColor = Color.OrangeRed;
                lblStatus.Text = "本次实施版本无相关组件！";
                return;
            }

            foreach (Component item in components.OrderBy(a => a.FileName))
            {
                FileInfo fi = fileInfos.FirstOrDefault(a => a.Name == item.FileName);
                if (fi == null)
                {
                    ListViewItem lvi = new ListViewItem(new string[] { item.FileName, "本地缺失！" });
                    lvi.ForeColor = Color.Red;
                    lvwComponent.Items.Add(lvi);
                }
                else
                {
                    if (fi.Length != item.FileSize || Math.Abs((fi.LastWriteTime.Subtract(item.CompileDateTime)).TotalSeconds) > 1)
                    {
                        lvwComponent.Items.Add(new ListViewItem(new string[] { item.FileName, fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"), item.CompileDateTime.ToString("yyyy-MM-dd HH:mm:ss"), fi.Length.ToString(), item.FileSize.ToString() }));
                    }
                }
            }

            lblStatus.ForeColor = (lvwComponent.Items.Count == 0) ? Color.Black : Color.Red;
            lblStatus.Text = (lvwComponent.Items.Count == 0) ? "本次实施版本组件均相同" : "本次实施版本组件有差异，详见表格！";
        }

        private void btnCheckVersion_Click(object sender, EventArgs e)
        {
            btnCheckVersion.Enabled = false;

            CheckFileVersion();
            CompareResources();
            txtSummary.Text = string.Format("组件：{0}/{1}，资源：{2}/{3}，脚本：{4}个", lvwComponent.Items.Count, _components.Count, lvwResource.Items.Count, _resources.Count, _dbscripts.Count);

            btnCheckVersion.Enabled = true;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkVersion_CheckedChanged(object sender, EventArgs e)
        {
            txtVersionID.Text = string.Empty;
            lblStatus.Text = string.Empty;
            lblStatus2.Text = string.Empty;
            SetTextBoxBackColor(chkVersion.Checked);
        }

        private void LoadResources(string filename)
        {
            if (Directory.Exists(TEMP_PATH))
                Directory.Delete(TEMP_PATH, true);

            Directory.CreateDirectory(TEMP_PATH);
            if (!File.Exists(filename)) return;

            string extension = Path.GetExtension(filename);
            if (string.Compare(extension, ".rar", StringComparison.OrdinalIgnoreCase) == 0)
                ExtractRarFiles(filename);
            else if (string.Compare(extension, ".zip", StringComparison.OrdinalIgnoreCase) == 0)
                ExtractZipFiles(filename);
        }

        private void ExtractRarFiles(string filename)
        {
            _resources.Clear();
            using (Stream stream = File.OpenRead(filename))
            {
                var reader = ReaderFactory.Open(stream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        string path = Path.Combine(TEMP_PATH, Path.GetDirectoryName(reader.Entry.FilePath));
                        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                        reader.WriteEntryToDirectory(path);
                        _resources.Add(reader.Entry.FilePath);
                    }
                }
            }
        }

        private void ExtractZipFiles(string filename)
        {
            _resources.Clear();
            var archive = ArchiveFactory.Open(filename);
            foreach (var entry in archive.Entries)
            {
                if (!entry.IsDirectory)
                {
                    string path = Path.Combine(TEMP_PATH, Path.GetDirectoryName(entry.FilePath));
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                    entry.WriteToDirectory(path);
                    _resources.Add(entry.FilePath);
                }
            }
        }

        private void WriteFilesAsZip(string path, string destFilePath)
        {
            try
            {
                if (File.Exists(destFilePath)) File.Delete(destFilePath);

                using (Stream stream = File.OpenWrite(destFilePath))
                using (var writer = WriterFactory.Open(stream, ArchiveType.Zip, CompressionType.None))
                {
                    writer.WriteAll(path, "*", SearchOption.AllDirectories);
                }

                MessageBox.Show(string.Format("资源文件导出成功。\r\n{0}", destFilePath), "导出资源", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("资源文件导出失败！\r\n{0}", ex.Message), "导出资源", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CompareResources()
        {
            LoadResources(RESOURCE_FILENAME);

            DirectoryInfo destfolder = new DirectoryInfo(TEMP_PATH);
            FileInfo[] destfiles = destfolder.GetFiles("*", SearchOption.AllDirectories);

            lvwResource.Items.Clear();
            if (destfiles.Length == 0)
            {
                lblStatus2.ForeColor = Color.OrangeRed;
                lblStatus2.Text = "无相关资源文件！";
                return;
            }

            string foldername = Path.Combine(RegistryHelper.GetExecutablePath(), "..\\Resources");
            if (!Directory.Exists(foldername)) return;

            DirectoryInfo folder = new DirectoryInfo(foldername);
            FileInfo[] fileInfos = folder.GetFiles("*", SearchOption.AllDirectories);

            foreach (FileInfo item in destfiles)
            {
                int pos = item.FullName.LastIndexOf(TEMP_PATH, StringComparison.OrdinalIgnoreCase);
                string sourcefile = item.FullName.Substring(pos + TEMP_PATH.Length);
                sourcefile = sourcefile.Replace("\\Resource\\", "\\Resources\\");
                FileInfo fi = fileInfos.FirstOrDefault(a => a.FullName.EndsWith(sourcefile, StringComparison.OrdinalIgnoreCase));
                if (fi == null)
                {
                    ListViewItem lvi = new ListViewItem(new string[] { sourcefile, "本地缺失！" });
                    lvi.ForeColor = Color.Red;
                    lvwResource.Items.Add(lvi);
                }
                else
                {
                    if (fi.Length != item.Length || Math.Abs((fi.LastWriteTime.Subtract(item.LastWriteTime)).TotalSeconds) > 1)
                    {
                        lvwResource.Items.Add(new ListViewItem(new string[] { sourcefile, fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"), item.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"), fi.Length.ToString(), item.Length.ToString() }));
                    }
                    //bool issame = Utility.CompareFiles(fi.FullName, item.FullName);
                }
            }

            lblStatus2.ForeColor = (lvwResource.Items.Count == 0) ? Color.Black : Color.Red;
            lblStatus2.Text = (lvwResource.Items.Count == 0) ? "资源文件均相同" : "资源文件有差异，详见表格！";
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog() { Description = "请选择将要比较的文件夹：" };
            fbd.ShowNewFolderButton = false;
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            fbd.SelectedPath = txtExecutePath.Text;
            if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                txtExecutePath.Text = fbd.SelectedPath;
            }
        }

        private void btnLocalService_Click(object sender, EventArgs e)
        {
            string jssvcpath = RegistryHelper.GetValue(RegistryHelper.LocalServiceKey, string.Empty).ToString();
            txtExecutePath.Text = Path.GetDirectoryName(jssvcpath);
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            int versionId;
            if (int.TryParse(txtVersionID.Text, out versionId))
            {
                Utility.ExportByMyXls(GetComponentDataTable(versionId), new int[] { 5000, 20000, 5000, 5000 }, "component");
                Utility.ExportByMyXls(GetDBScriptDataTable(versionId), new int[] { 3000, 3000, 10000, 20000, 5000, 6000 }, "dbscript");
            }
        }

        private void btnRar_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog() { Description = "请选择将要打包的文件夹：" };
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            fbd.SelectedPath = Path.Combine(Directory.GetCurrentDirectory(), TEMP_PATH);
            if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                WriteFilesAsZip(fbd.SelectedPath, Path.Combine(Directory.GetCurrentDirectory(), "resource.rar"));
            }
        }
    }
}
