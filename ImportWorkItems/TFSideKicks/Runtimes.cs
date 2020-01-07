using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.Linq.Expressions;
using System.Windows;

namespace TFSideKicks
{
    public static class Runtimes
    {
        //根据excle的路径把第一个sheel中的内容放入datatable
        public static DataTable ReadExcelToTable(string path)//excel存放的路径
        {
            DataTable ret;
            // HDR=YES 有两个值:YES/NO,表示第一行是否字段名,默认是YES,第一行是字段名
            // IMEX=1 表示是否强制转换为文本,解决数字与字符混合时,识别不正常的情况.
            // PS:IMEX=0---输出模式;IMEX=1---输入模式;IMEX=2----链接模式(完全更新能力)
            string connstring1 = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1';"; // Office 7及以上版本 不能出现多余的空格 而且分号注意
            string connstring2 = "Provider=Microsoft.JET.OLEDB.4.0;Data Source=" + path + ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1';";  //Office 7以下版本 
            try
            {
                ret = LoadExcelData(connstring1);
            }
            catch (Exception)
            {
                ret = LoadExcelData(connstring2);
            }
            return ret;
        }

        private static DataTable LoadExcelData(string connstring)
        {
            using (OleDbConnection conn = new OleDbConnection(connstring))
            {
                conn.Open();
                DataTable sheetsName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" }); //得到所有sheet的名字
                string firstSheetName = sheetsName.Rows[0][2].ToString(); //得到第一个sheet的名字
                string sql = string.Format("SELECT * FROM [{0}]", firstSheetName); //查询字符串
                //string sql = "SELECT * FROM [Sheet1$A:D]";
                //string sql = string.Format("SELECT [ComponentID], [SourcePath], [FileSize], [CompileDateTime] FROM [{0}]", firstSheetName); //查询字符串

                OleDbDataAdapter ada = new OleDbDataAdapter(sql, connstring);
                DataSet set = new DataSet();
                ada.Fill(set);
                return set.Tables[0];
            }
        }
    }


}
