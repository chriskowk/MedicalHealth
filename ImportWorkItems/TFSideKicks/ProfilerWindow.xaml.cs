using Genersoft.Platform.Core.DataAccess;
using Genersoft.Platform.Core.DataAccess.Configuration;
using Genersoft.Platform.Core.DataAccess.Oracle;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TFSideKicks
{
    /// <summary>
    /// ProfilerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProfilerWindow : Window
    {
        private string OldTable;
        private string NewTable;
        private string Sysdate;
        private bool IsCurrUser;
        private bool IsSaveOracle;
        public ProfilerWindow()
        {
            InitializeComponent();
            this.OldTable = "TEMP_ORACLESQLOLD";
            this.NewTable = "TEMP_ORACLESQLNEW";
            this.Sysdate = "";
            this.IsCurrUser = false;
            this.IsSaveOracle = false;
            this.tb_oraname.IsEnabled = false;
        }



        private IGSPDatabase _db;
        public IGSPDatabase DB
        {
            get
            {
                if (_db == null)
                {
                    _db = GetDatabase(this.ConfigData);
                }
                return _db;
            }
        }

        private GSPDbConfigData _configData = null;
        private GSPDbConfigData ConfigData
        {
            get
            {
                if (_configData == null)
                {
                    _configData = new GSPDbConfigData();
                    _configData.DbType = GSPDbType.Oracle;
                    _configData.ConnectionString = string.Format("DATA SOURCE = {0}; USER ID = {1}; PASSWORD = {2};", tb_source.Text, tb_user.Text, tb_password.Password);
                    _configData.Source = tb_source.Text;
                    _configData.UserId = tb_user.Text;
                    _configData.Password = tb_password.Password;
                }

                return _configData;
            }
        }

        private IGSPDatabase GetDatabase(GSPDbConfigData configData)
        {
            return new OracleDatabase(configData);
        }

        private void cb_save_Checked(object sender, RoutedEventArgs e)
        {
            if (this.cb_save.IsChecked.Value)
            {
                this.IsSaveOracle = true;
                this.tb_oraname.IsEnabled = true;
            }
            else
            {
                this.IsSaveOracle = false;
                this.tb_oraname.IsEnabled = false;
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            tb_source.Focus();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.button1.IsEnabled = false;
            this.button2.IsEnabled = true;
            this.tb_log.Clear();
            this.tb_log.AppendText("------------------------------\r\n");
            this.tb_log.AppendText("Getting ready...\r\n");
            this.tb_log.AppendText("Clear data...\r\n");
            string sql1 = "select table_name from user_tables where table_name='" + this.OldTable + "'";
            string sql2 = "select table_name from user_tables where table_name='" + this.NewTable + "'";
            DataSet ds1 = this.DB.ExecuteDataSet(sql1);
            DataSet ds2 = this.DB.ExecuteDataSet(sql2);
            if (((ds1 != null) && (ds1.Tables[0].Rows.Count > 0)) && (Convert.ToString(ds1.Tables[0].Rows[0]["table_name"]) == this.OldTable))
            {
                this.DB.ExecSqlStatement("drop table " + this.OldTable + " purge");
            }
            if (((ds2 != null) && (ds2.Tables[0].Rows.Count > 0)) && (Convert.ToString(ds2.Tables[0].Rows[0]["table_name"]) == this.NewTable))
            {
                this.DB.ExecSqlStatement("drop table " + this.NewTable + " purge");
            }
            this.tb_log.AppendText("Clear data succeed.\r\n");
            this.tb_log.AppendText("Create dataTable...\r\n");
            string createTableSql = "create table " + this.OldTable + " as select * from v$sqlarea";
            this.DB.ExecSqlStatement(createTableSql);
            this.tb_log.AppendText("Create tableTable succeed.\r\n");
            string getDateSql = "SELECT to_char(SYSDATE,'yyyy/MM/dd hh24:mi:ss') as currdate FROM dual";
            DataSet ds = this.DB.ExecuteDataSet(getDateSql);
            if (ds != null)
            {
                this.Sysdate = Convert.ToString(ds.Tables[0].Rows[0]["currdate"]);
            }
            if (this.cb_curruser.IsChecked.Value)
            {
                this.IsCurrUser = true;
            }
            this.tb_log.AppendText("Success!\r\n");
            this.tb_log.AppendText("------------------------------\r\n\r\n");
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.button1.IsEnabled = true;
            this.button2.IsEnabled = false;
            this.tb_log.AppendText("Processing data...\r\n");
            string createTableSql = "create table " + this.NewTable + " as select * from v$sqlarea";
            this.DB.ExecSqlStatement(createTableSql);
            string sql = "";
            if (this.IsCurrUser)
            {
                string user = "";
                string usersql = "select user from dual";
                DataSet ds = this.DB.ExecuteDataSet(usersql);
                if ((ds != null) && (ds.Tables[0].Rows.Count > 0))
                {
                    user = Convert.ToString(ds.Tables[0].Rows[0]["user"]);
                }
                string[] textArray1 = new string[] { "SELECT * FROM (SELECT n.parsing_schema_name AS SCHEMA,n.module AS MODULE, DBMS_LOB.SUBSTR(n.Sql_Fulltext,2000,1) AS SQL_TEXT,n.parse_calls-nvl(o.parse_calls,0) AS PARSE_CALLS,n.buffer_gets-nvl(o.buffer_gets,0) AS BUFFER_GETS,n.disk_reads-nvl(o.disk_reads,0) AS DISK_READS,n.executions-nvl(o.executions,0) AS EXECUTIONS,round((n.cpu_time-nvl(o.cpu_time,0))/1000000,2) AS CPU_TIME,round((n.cpu_time-nvl(o.cpu_time,0))/((n.executions-nvl(o.executions,0))*1000000),2) AS CPU_TIME_PER_EXE,round((n.elapsed_time-nvl(o.elapsed_time,0))/((n.executions-nvl(o.executions,0))*1000000),2) AS ELAPSED_TIME_PER_EXE FROM ", this.NewTable, " n LEFT JOIN ", this.OldTable, " o ON o.hash_value = n.hash_value AND o.address = n.address WHERE n.last_active_time > to_date('", this.Sysdate, "','yyyy/MM/dd hh24:mi:ss') AND(n.executions - nvl(o.executions,0)) > 0 AND n.parsing_schema_name = '", user, "' ORDER BY ELAPSED_TIME_PER_EXE DESC) WHERE ROWNUM<=100" };
                sql = string.Concat(textArray1);
                if (this.IsSaveOracle)
                {
                    string oracleTableName = this.tb_oraname.Text.ToString().ToUpper();
                    this.tb_log.AppendText("Checking table:" + oracleTableName + "...\r\n");
                    string sql1 = "select table_name from user_tables where table_name='" + oracleTableName + "'";
                    DataSet ds1 = this.DB.ExecuteDataSet(sql1);
                    if (((ds1 != null) && (ds1.Tables[0].Rows.Count > 0)) && (Convert.ToString(ds1.Tables[0].Rows[0]["table_name"]) == oracleTableName))
                    {
                        this.tb_log.AppendText("Drop table:" + oracleTableName + "...\r\n");
                        this.DB.ExecSqlStatement("drop table " + oracleTableName + " purge");
                    }
                    string[] textArray2 = new string[] { "create table ", oracleTableName, " as SELECT n.parsing_schema_name AS SCHEMA,n.module AS MODULE, DBMS_LOB.SUBSTR(n.Sql_Fulltext,2000,1) AS SQL_TEXT,n.parse_calls-nvl(o.parse_calls,0) AS PARSE_CALLS,n.buffer_gets-nvl(o.buffer_gets,0) AS BUFFER_GETS,n.disk_reads-nvl(o.disk_reads,0) AS DISK_READS,n.executions-nvl(o.executions,0) AS EXECUTIONS,round((n.cpu_time-nvl(o.cpu_time,0))/1000000,2) AS CPU_TIME,round((n.cpu_time-nvl(o.cpu_time,0))/((n.executions-nvl(o.executions,0))*1000000),2) AS CPU_TIME_PER_EXE,round((n.elapsed_time-nvl(o.elapsed_time,0))/((n.executions-nvl(o.executions,0))*1000000),2) AS ELAPSED_TIME_PER_EXE FROM ", this.NewTable, " n LEFT JOIN ", this.OldTable, " o ON o.hash_value = n.hash_value AND o.address = n.address WHERE n.last_active_time > to_date('", this.Sysdate, "','yyyy/MM/dd hh24:mi:ss') AND(n.executions - nvl(o.executions,0)) > 0 AND n.parsing_schema_name = '", user, "' ORDER BY ELAPSED_TIME_PER_EXE DESC" };
                    string createTable = string.Concat(textArray2);
                    this.tb_log.AppendText("Creating table:" + oracleTableName + "...\r\n");
                    this.DB.ExecSqlStatement(createTable);
                }
            }
            else
            {
                string[] textArray3 = new string[] { "SELECT * FROM (SELECT n.parsing_schema_name AS SCHEMA,n.module AS MODULE, DBMS_LOB.SUBSTR(n.Sql_Fulltext,2000,1) AS SQL_TEXT,n.parse_calls-nvl(o.parse_calls,0) AS PARSE_CALLS,n.buffer_gets-nvl(o.buffer_gets,0) AS BUFFER_GETS,n.disk_reads-nvl(o.disk_reads,0) AS DISK_READS,n.executions-nvl(o.executions,0) AS EXECUTIONS,round((n.cpu_time-nvl(o.cpu_time,0))/1000000,2) AS CPU_TIME,round((n.cpu_time-nvl(o.cpu_time,0))/((n.executions-nvl(o.executions,0))*1000000),2) AS CPU_TIME_PER_EXE,round((n.elapsed_time-nvl(o.elapsed_time,0))/((n.executions-nvl(o.executions,0))*1000000),2) AS ELAPSED_TIME_PER_EXE FROM ", this.NewTable, " n LEFT JOIN ", this.OldTable, " o ON o.hash_value = n.hash_value AND o.address = n.address WHERE n.last_active_time > to_date('", this.Sysdate, "','yyyy/MM/dd hh24:mi:ss') AND(n.executions - nvl(o.executions,0)) > 0 ORDER BY ELAPSED_TIME_PER_EXE DESC) WHERE ROWNUM<=100" };
                sql = string.Concat(textArray3);
                if (this.IsSaveOracle)
                {
                    string oracleTableName = this.tb_oraname.Text.ToString().ToUpper();
                    this.tb_log.AppendText("Checking table:" + oracleTableName + "...\r\n");
                    string sql1 = "select table_name from user_tables where table_name='" + oracleTableName + "'";
                    DataSet ds1 = this.DB.ExecuteDataSet(sql1);
                    if (((ds1 != null) && (ds1.Tables[0].Rows.Count > 0)) && (Convert.ToString(ds1.Tables[0].Rows[0]["table_name"]) == oracleTableName))
                    {
                        this.tb_log.AppendText("Drop table:" + oracleTableName + "...\r\n");
                        this.DB.ExecSqlStatement("drop table " + oracleTableName + " purge");
                    }
                    string[] textArray4 = new string[] { "create table ", oracleTableName, " as SELECT n.parsing_schema_name AS SCHEMA,n.module AS MODULE, DBMS_LOB.SUBSTR(n.Sql_Fulltext,2000,1) AS SQL_TEXT,n.parse_calls-nvl(o.parse_calls,0) AS PARSE_CALLS,n.buffer_gets-nvl(o.buffer_gets,0) AS BUFFER_GETS,n.disk_reads-nvl(o.disk_reads,0) AS DISK_READS,n.executions-nvl(o.executions,0) AS EXECUTIONS,round((n.cpu_time-nvl(o.cpu_time,0))/1000000,2) AS CPU_TIME,round((n.cpu_time-nvl(o.cpu_time,0))/((n.executions-nvl(o.executions,0))*1000000),2) AS CPU_TIME_PER_EXE,round((n.elapsed_time-nvl(o.elapsed_time,0))/((n.executions-nvl(o.executions,0))*1000000),2) AS ELAPSED_TIME_PER_EXE FROM ", this.NewTable, " n LEFT JOIN ", this.OldTable, " o ON o.hash_value = n.hash_value AND o.address = n.address WHERE n.last_active_time > to_date('", this.Sysdate, "','yyyy/MM/dd hh24:mi:ss') AND(n.executions - nvl(o.executions,0)) > 0 ORDER BY ELAPSED_TIME_PER_EXE DESC" };
                    string createTable = string.Concat(textArray4);
                    this.DB.ExecSqlStatement(createTable);
                }
            }
            if (this.IsSaveOracle)
            {
                this.tb_log.AppendText("Save data to table:" + this.tb_oraname.Text.ToString() + "\r\n");
            }
            DataSet ds_result = this.DB.ExecuteDataSet(sql);
            if (ds_result != null)
            {
                this._dgSQLlines.DataContext = ds_result.Tables[0];
            }
            this.tb_log.AppendText("Success!\r\n");
        }
    }
}
