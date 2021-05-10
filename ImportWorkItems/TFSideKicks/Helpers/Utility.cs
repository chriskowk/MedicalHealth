using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Windows;
using System.Collections;
using System.Windows.Input;
using System.Linq.Expressions;
using System.IO;
using Microsoft.Win32;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using MyXls = Core.MyXls;
using System.Diagnostics;

namespace TFSideKicks
{
    public static class Utility
    {
        public static void Export2XmlExcel(DataTable table)
        {
            if (table == null || table.Rows.Count == 0) return;

            SaveFileDialog sfd = new SaveFileDialog() { DefaultExt = "xls", Filter = "Excel文件(*.xls)|*.xls|CSV文件(*.csv)|*.csv|All files (*.*)|*.*", FilterIndex = 1 };
            if (sfd.ShowDialog() == true)
            {
                string format = sfd.SafeFileName.Substring(sfd.SafeFileName.IndexOf('.') + 1).ToUpper();
                StringBuilder sb = new StringBuilder();
                List<string> fields = new List<string>();
                //headers
                foreach (DataColumn col in table.Columns)
                {
                    fields.Add(FormatField(col.ColumnName, format));
                }
                BuildStringOfRow(sb, fields, format);
                //rows
                foreach (DataRow row in table.Rows)
                {
                    fields.Clear();
                    foreach (DataColumn col in table.Columns)
                    {
                        fields.Add(FormatField(row[col].ToString(), format));
                    }
                    BuildStringOfRow(sb, fields, format);
                }

                WriteEndingFile(sfd.OpenFile(), sb.ToString(), format);
            }
        }

        private static void WriteEndingFile(Stream stream, string content, string format)
        {
            if (null == stream || string.IsNullOrEmpty(content)) return;

            bool isXls = GetFileType(format) == FileType.xls;

            Encoding encoding = isXls ? Encoding.UTF8 : Encoding.Unicode;
            StreamWriter sw = new StreamWriter(stream, encoding);
            if (isXls)
            {
                //write the headers for the Excel XML
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                sw.WriteLine("<?mso-application progid=\"Excel.Sheet\"?>");
                sw.WriteLine("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\">");
                sw.WriteLine("<DocumentProperties xmlns=\"urn:schemas-microsoft-com:office:office\">");
                sw.WriteLine("<Author>Arasu Elango</Author>");
                sw.WriteLine("<Created>" + DateTime.Now.ToLocalTime().ToLongDateString() + "</Created>");
                sw.WriteLine("<LastSaved>" + DateTime.Now.ToLocalTime().ToLongDateString() + "</LastSaved>");
                sw.WriteLine("<Company>Guangzhou Jetsun Information System Co,Ltd.</Company>");
                sw.WriteLine("<Version>12.00</Version>");
                sw.WriteLine("</DocumentProperties>");
                sw.WriteLine("<Worksheet ss:Name=\"Export Sheet\" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\">");
                sw.WriteLine("<Table>");
            }

            //write the bodies 
            sw.Write(content);

            if (isXls)
            {
                //write the footers for the Excel XML
                sw.WriteLine("</Table>");
                sw.WriteLine("</Worksheet>");
                sw.WriteLine("</Workbook>");
            }

            sw.Close();
        }

        private enum FileType { xls, csv, other }
        static FileType GetFileType(string format)
        {
            if (string.Equals(format, "xls", StringComparison.CurrentCultureIgnoreCase))
                return FileType.xls;

            if (string.Equals(format, "csv", StringComparison.CurrentCultureIgnoreCase))
                return FileType.csv;

            return FileType.other;
        }

        private static void BuildStringOfRow(StringBuilder sb, IList<string> fields, string format)
        {
            switch (GetFileType(format))
            {
                case FileType.xls:
                    sb.AppendLine("<Row>");
                    sb.AppendLine(String.Join("\r\n", fields.ToArray()));
                    sb.AppendLine("</Row>");
                    break;
                case FileType.csv:
                    sb.AppendLine(String.Join("\t", fields.ToArray()));
                    break;
            }
        }

        private static string FormatField(string data, string format)
        {
            switch (GetFileType(format))
            {
                case FileType.xls:
                    return String.Format("<Cell><Data ss:Type=\"String\">{0}</Data></Cell>", data);
                case FileType.csv:
                    return String.Format("\"{0}\"", data.Replace("\"", "\"\"").Replace("\n", "").Replace("\r", ""));
            }
            return data;
        }
        private static string FormatColumn(string width, bool isAuto, string format)
        {
            switch (GetFileType(format))
            {
                case FileType.xls:
                    return isAuto ? String.Format("<Column ss:AutoFitWidth=\"0\" />") : String.Format("<Column ss:AutoFitWidth=\"0\" ss:Width=\"{0}\"/>", width);
            }
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dg"></param>
        /// <param name="filename"></param>
        public static void ExportByMyXls(DataTable table, int[] colWidths, string filename = "", bool withDatePosfix = false)
        {
            if (table == null || table.Rows.Count == 0)
            {
                MessageBox.Show("没有数据导出！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            int state = 0;
            string filePath = string.Empty;

            if (string.IsNullOrWhiteSpace(filename) || withDatePosfix)
                filename = (filename ?? "").Replace("/", "") + DateTime.Now.ToString("yyyyMMdd").Replace("-", "").Replace(":", "").Replace(" ", "") + ".xls";

            SaveFileDialog sfd = new SaveFileDialog() { DefaultExt = "xls", Filter = "Excel文件(*.xls)|*.xls|CSV文件(*.csv)|*.csv|All files (*.*)|*.*", FilterIndex = 1 };
            sfd.FileName = Path.Combine(sfd.FileName, filename);
            if (sfd.ShowDialog() != true)
                return;

            MyXls.XlsDocument doc = new MyXls.XlsDocument();
            try
            {
                //弹出保存窗口后用户可能会修改保存的文件名
                filename = Path.GetFileName(sfd.FileName);
                doc.FileName = filename;//excel文件名称
                filePath = sfd.FileName;

                MyXls.Worksheet sheet = doc.Workbook.Worksheets.Add("Sheet1");//Excel工作表名称
                int columnCount = table.Columns.Count;

                // 列标题样式
                MyXls.XF columnTitleXF = doc.NewXF();
                columnTitleXF.HorizontalAlignment = MyXls.HorizontalAlignments.Left;
                columnTitleXF.VerticalAlignment = MyXls.VerticalAlignments.Centered;
                columnTitleXF.UseBorder = true;
                columnTitleXF.TopLineStyle = 1;
                columnTitleXF.TopLineColor = MyXls.Colors.Black;
                columnTitleXF.BottomLineStyle = 1;
                columnTitleXF.BottomLineColor = MyXls.Colors.Black;
                columnTitleXF.LeftLineStyle = 1;
                columnTitleXF.LeftLineColor = MyXls.Colors.Black;
                columnTitleXF.Font.FontName = "宋体"; // 字体
                columnTitleXF.Font.Bold = true;
                columnTitleXF.Font.Height = 10 * 20; // 设定字大小（字体大小是以 1/20 point 为单位的）
                columnTitleXF.Pattern = 1; // 单元格填充风格。如果设定为0，则是纯色填充(无色)，1代表没有间隙的实色 
                columnTitleXF.PatternBackgroundColor = MyXls.Colors.Red; // 填充的底色 
                columnTitleXF.PatternColor = MyXls.Colors.Default2F; // 填充背景色

                // 数据单元格样式
                MyXls.XF dataXF = doc.NewXF(); // 为xls生成一个XF实例，XF是单元格格式对象
                dataXF.HorizontalAlignment = MyXls.HorizontalAlignments.Left; // 设定文字居左
                dataXF.VerticalAlignment = MyXls.VerticalAlignments.Centered; // 垂直居中
                dataXF.UseBorder = true; // 使用边框 
                dataXF.LeftLineStyle = 1; // 左边框样式
                dataXF.LeftLineColor = MyXls.Colors.Black; // 左边框颜色
                dataXF.BottomLineStyle = 1;  // 下边框样式
                dataXF.BottomLineColor = MyXls.Colors.Black;  // 下边框颜色
                dataXF.Font.FontName = "宋体";
                dataXF.Font.Height = 10 * 20; // 设定字大小（字体大小是以 1/20 point 为单位的）
                dataXF.UseProtection = false; // 默认的就是受保护的，导出后需要启用编辑才可修改
                dataXF.TextWrapRight = true; // 自动换行

                MyXls.Cells cells = sheet.Cells;
                MyXls.ColumnInfo colInfo;
                int rowIndex = 1;
                int rowCount = table.Rows.Count;

                foreach (DataColumn col in table.Columns)
                {
                    colInfo = new MyXls.ColumnInfo(doc, sheet); // 列对象 
                    colInfo.ColumnIndexStart = (ushort)col.Ordinal; // 起始列为第1列，索引从0开始
                    colInfo.ColumnIndexEnd = (ushort)col.Ordinal;

                    int colWidth = colWidths[col.Ordinal];
                    colInfo.Width = (ushort)colWidth; // 列的宽度计量单位为 1/256 字符宽
                    sheet.AddColumnInfo(colInfo); // 把格式附加到sheet页上

                    cells.Add(rowIndex, col.Ordinal + 1, col.ColumnName, columnTitleXF);
                }
                foreach (DataRow row in table.Rows)
                {
                    rowIndex++;
                    foreach (DataColumn col in table.Columns)
                    {
                        string cellValue = row[col].ToString();
                        cells.Add(rowIndex, col.Ordinal + 1, cellValue, dataXF);
                    }
                }
                doc.Save(Path.GetDirectoryName(filePath), true);  //保存到指定位置
                state = 1;
                MessageBox.Show(string.Format("导出成功。\r\n{0}", filePath), "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                System.Diagnostics.Process.Start(filePath);
            }
            catch (Exception ex)
            {
                if (state == 1)
                    MessageBox.Show(string.Format("打开文件失败！\r\n{0}", ex.Message), "系统提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                else
                    MessageBox.Show(string.Format("导出失败！请确认文件是否已打开。\r\n{0} ", ex.Message), "系统提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        /// <summary>
        /// 根据路径比较目录下的文件是否都相同
        /// </summary>
        /// <param name="SCRATCH_FILES_PATH"></param>
        /// <param name="ORIGINAL_FILES_PATH"></param>
        /// <returns></returns>
        public static bool VerifyFilesByPath(string SCRATCH_FILES_PATH, string ORIGINAL_FILES_PATH)
        {
            bool ret = true;
            var extracted =
                Directory.EnumerateFiles(SCRATCH_FILES_PATH, "*.*", SearchOption.AllDirectories)
                .ToLookup(path => path.Substring(SCRATCH_FILES_PATH.Length));
            var original =
                Directory.EnumerateFiles(ORIGINAL_FILES_PATH, "*.*", SearchOption.AllDirectories)
                .ToLookup(path => path.Substring(ORIGINAL_FILES_PATH.Length));

            ret = extracted.Count == original.Count;
            if (!ret) return false;

            foreach (var orig in original)
            {
                ret = ret && extracted.Contains(orig.Key);
                if (!ret) return false;

                ret = ret && CompareFiles(orig.Single(), extracted[orig.Key].Single());
                if (!ret) return false;
            }
            return ret;
        }

        /// <summary>
        /// 比较两个文件内容是否一致
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        public static bool CompareFiles(string file1, string file2)
        {
            bool ret = true;
            using (var file1Stream = File.OpenRead(file1))
            using (var file2Stream = File.OpenRead(file2))
            {
                ret = file1Stream.Length == file2Stream.Length;
                if (!ret) return false;

                int byte1 = 0;
                int byte2 = 0;
                while (byte1 != -1)
                {
                    byte1 = file1Stream.ReadByte();
                    byte2 = file2Stream.ReadByte();
                    ret = ret && byte1 == byte2;
                    if (!ret) return false;
                }
            }
            return ret;
        }

        public static string Execute(string filefullname, int seconds, bool redirectStandardOutput, bool redirectStandardError, ref string errMsg)
        {
            string output = ""; //输出字符串  
            if (filefullname != null && !filefullname.Equals(""))
            {
                FileInfo file = new FileInfo(filefullname);
                Process process = new Process();//创建进程对象  
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";//设定需要执行的命令  
                startInfo.WorkingDirectory = file.Directory.FullName;
                startInfo.Arguments = "/C " + filefullname;//“/C”表示执行完命令后马上退出  
                startInfo.UseShellExecute = false;//使用系统外壳程序启动  
                startInfo.RedirectStandardInput = false;//不重定向输入  
                startInfo.RedirectStandardOutput = redirectStandardOutput; //重定向输出  
                startInfo.RedirectStandardError = redirectStandardError;
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
                            process.WaitForExit(seconds * 1000); //等待进程结束，等待时间为指定的毫秒  
                        }
                        output = process.StartInfo.RedirectStandardOutput ? process.StandardOutput.ReadToEnd() : string.Empty;
                        errMsg = process.StartInfo.RedirectStandardError ? process.StandardError.ReadToEnd() : string.Empty;
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

    }
}
