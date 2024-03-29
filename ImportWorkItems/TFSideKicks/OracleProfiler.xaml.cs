﻿using Genersoft.Platform.Core.DataAccess;
using Genersoft.Platform.Core.DataAccess.Configuration;
using Genersoft.Platform.Core.DataAccess.Oracle;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using TFSideKicks.Controls;
using TFSideKicks.Datas;

namespace TFSideKicks
{
    /// <summary>
    /// OracleProfiler.xaml 的交互逻辑
    /// </summary>
    public partial class OracleProfiler : Window
    {
        private string StartOnText;
        private bool IsCurrUser;
        private bool IsSaveOracle;
        public OracleProfiler()
        {
            InitializeComponent();

            this.StartOnText = "";
            this.IsCurrUser = false;
            this.IsSaveOracle = false;
            this.tb_oraname.IsEnabled = false;
            this.mcbo_module.ItemsSource = TraceModuleList.Instance.TraceModuleListData;
        }

        private void cb_save_Click(object sender, RoutedEventArgs e)
        {
            if (this.cb_save.IsChecked.HasValue && this.cb_save.IsChecked.Value)
            {
                this.IsSaveOracle = true;
                this.tb_oraname.IsEnabled = true;
                this.tb_oraname.Focus();
            }
            else
            {
                this.IsSaveOracle = false;
                this.tb_oraname.IsEnabled = false;
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            tb_host.Focus();
        }

        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            this.button1.IsEnabled = false;
            this.button2.IsEnabled = false;
            this.tb_log.Clear();
            this.tb_Status.Clear();
            this.dg_SQLlines.DataContext = null;

            this.tb_log.AppendText("-----------------------------------------------\r\n");
            this.tb_log.AppendText("Getting ready ...\r\n");
            this.tb_log.AppendText("Clearing data ...\r\n");

            string host = tb_host.Text.Trim();
            string port = tb_port.Text.Trim();
            string service = tb_service.Text.Trim();
            string userid = tb_user.Text;
            string password = tb_password.Password;
            await Task.Run(() => DropTables(host, port, service, userid, password));
            this.tb_log.AppendText("Clear data succeed.\r\n");

            this.tb_log.AppendText("Creating data table ...\r\n");
            bool onlycurruser = this.cb_curruser.IsChecked.Value;
            await Task.Run(() => CreateTables(onlycurruser));
            this.tb_log.AppendText(string.Format("Success! starting on {0}\r\n", this.StartOnText));
            this.tb_log.AppendText("-----------------------------------------------\r\n");
            this.button2.IsEnabled = true;
        }

        private void DropTables(string host, string port, string service, string userid, string password)
        {
            try
            {
                Runtimes.ConnectionString = $"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = {host})(PORT = {port}))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = {service}))); Persist Security Info=True;User ID={userid};Password={password};";
                OracleDbContext.Initialize(host, port, service, userid, password);
                //DataTable dt = OracleDbContext.GetDomainSystemTable();
                //int count = OracleDbHelper.ExecuteSql("select * from CORE.DOMAINSYSTEM where SYSTEMKEY = :p0", "cis");
                //DataTable dt1 = OracleDbHelper.ExecuteDataTable("select * from CORE.DOMAINSYSTEM where SYSTEMKEY = :key", new OracleParameter("@key", "emr"));
                
                //StringBuilder sb = new StringBuilder();
                //IList<OracleParameter> paras = new List<OracleParameter>();
                //IList<string> uniqueIds = new List<string> { "c5331527bcc249909742c1f961df3e0d,672a1eb4bedd414c9baa36051f63d120", "5d09f9f161d04202895d18233d56886b,8d531688e23b4d96964f852921925e16", "e32db5191dec4c91a5a176d33a98236d,28273471d69644518225fcc4ce2a86f1" };
                //sb.AppendLine("BEGIN");
                //for (int i = 0; i < 3; i++)
                //{
                //    sb.AppendLine($"update DATA.bjcasignitem set SignStatusCodeId=:p{i * 3},RowVersion=:p{i * 3 + 1} where UniqueId in (SELECT CAST(column_value AS VARCHAR2(128)) ItemId FROM TABLE(poor.fnIDInString(:p{i * 3 + 2},',')));");
                //    paras.Add(new OracleParameter($"@p{i * 3}", OracleDbType.Int32, i, ParameterDirection.Input));
                //    paras.Add(new OracleParameter($"@p{i * 3 + 1}", OracleDbType.Date, DateTime.Now, ParameterDirection.Input));
                //    paras.Add(new OracleParameter($"@p{i * 3 + 2}", OracleDbType.Varchar2, uniqueIds[i], ParameterDirection.Input));
                //}
                //sb.Append("END;");
                //int ret = OracleDbHelper.ExecuteSql(sb.ToString(), paras.ToArray());

                //sb = new StringBuilder();
                //sb.AppendLine("BEGIN");
                //sb.AppendLine($"UPDATE POOR.ORDERREQUEST s set S.ISDELETED=1 where s.encounterid =:EncounterId and s.ISDELETED=0;");
                //sb.AppendLine($"UPDATE POOR.PERFORMREQUEST s SET S.ISDELETED = 1 where s.ENCOUNTERID =:EncounterId and s.ISDELETED = 0;");
                //sb.AppendLine($"UPDATE POOR.PERFORMREQUESTBILL s SET S.ISDELETED= 1 where s.ENCOUNTERID =:EncounterId and s.ISDELETED=0;");
                //sb.AppendLine($"UPDATE FIAB.OTHERBILLING s SET S.ISDELETED= 1 where s.ENCOUNTERID =:EncounterId and s.ISDELETED=0;");
                //sb.AppendLine("COMMIT;");
                //sb.AppendLine("END;");
                //paras.Add(new OracleParameter($"@EncounterId", OracleDbType.Int32, 11961, ParameterDirection.Input));
                //ret = OracleDbHelper.ExecuteSql(sb.ToString(), paras.ToArray());

                string sql = "SELECT table_name FROM all_tables WHERE table_name = :p0";
                IList<string> tns = new List<string> { OracleDbContext.OldTable, OracleDbContext.NewTable };
                foreach (var tablename in tns)
                {
                    if (!string.IsNullOrEmpty(OracleDbHelper.GetSingle(sql, new OracleParameter("@p0", tablename))?.ToString()))
                    {
                        OracleDbHelper.ExecuteSql($"DROP TABLE {tablename} PURGE"); //删除Table不进入Recycle
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "异常", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void CreateTables(bool onlycurruser)
        {
            try
            {
                string createTableSql = $"CREATE TABLE {OracleDbContext.OldTable} AS SELECT * FROM v$sqlarea";
                //Context.DB.ExecSqlStatement(createTableSql);
                OracleDbHelper.ExecuteSql(createTableSql);
                this.Dispatcher.BeginInvoke(new Action(() => this.tb_log.AppendText("Create data table succeed.\r\n")));

                string getDateSql = "SELECT to_char(SYSDATE,'yyyy/MM/dd hh24:mi:ss') AS currdate FROM dual";
                //DataSet ds = Context.DB.ExecuteDataSet(getDateSql);
                //if (ds != null) { this.StartOnText = Convert.ToString(ds.Tables[0].Rows[0]["currdate"]); }
                string currdate = OracleDbHelper.GetSingle(getDateSql)?.ToString();
                if (!string.IsNullOrEmpty(currdate)) this.StartOnText = currdate;
                if (onlycurruser) { this.IsCurrUser = true; }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "异常", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        DataView _dataView;
        private async void button2_Click(object sender, RoutedEventArgs e)
        {
            this.button1.IsEnabled = false;
            this.button2.IsEnabled = false;
            this.tb_log.AppendText("Processing data ...\r\n");
            string tablename = tb_oraname.Text.ToUpper();
            string module = mcbo_module.Text.ToUpper();
            string sql = string.Empty;
            bool ret = await Task.Run(() => ProcessData(tablename, module, out sql));
            if (!ret || string.IsNullOrEmpty(sql)) return;

            if (this.IsSaveOracle)
                this.tb_log.AppendText("Save data to table:" + this.tb_oraname.Text.ToString() + "\r\n");

            DataSet ds_result = await Task.Run(() => LoadData(sql));
            if (ds_result != null)
            {
                _dataView = ds_result.Tables[0].DefaultView;
                this.dg_SQLlines.DataContext = _dataView;
            }

            this.tb_log.AppendText("Success!");
            this.button1.IsEnabled = true;
        }

        private bool ProcessData(string tablename, string module, out string sql)
        {
            try
            {
                string createTableSql = $"CREATE TABLE {OracleDbContext.NewTable} AS SELECT * FROM v$sqlarea";
                //Context.DB.ExecSqlStatement(createTableSql);
                OracleDbHelper.ExecuteSql(createTableSql);

                //string sqlbase = string.Format(@"SELECT a.SQL_ID, a.parsing_schema_name AS SCHEMA, a.module AS MODULE, a.sql_text AS SQL_TEXT 
                //, a.sql_fulltext AS SQL_FULLTEXT 
                //, a.parse_calls - nvl(b.parse_calls, 0) AS PARSE_CALLS 
                //, a.buffer_gets - nvl(b.buffer_gets, 0) AS BUFFER_GETS 
                //, a.disk_reads - nvl(b.disk_reads, 0) AS DISK_READS 
                //, a.executions - nvl(b.executions, 0) AS EXECUTIONS 
                //, round((a.cpu_time - nvl(b.cpu_time, 0)) / 1000000, 2) AS CPU_TIME 
                //, round((a.cpu_time - nvl(b.cpu_time, 0)) / ((a.executions - nvl(b.executions, 0)) * 1000000), 2) AS CPU_TIME_PER_EXE 
                //, round((a.elapsed_time - nvl(b.elapsed_time, 0)) / ((a.executions - nvl(b.executions, 0)) * 1000000), 2) AS ELAPSED_TIME_PER_EXE 
                //, a.LAST_LOAD_TIME, a.LAST_ACTIVE_TIME 
                //FROM {0} a LEFT JOIN {1} b ON a.hash_value = b.hash_value AND a.address = b.address 
                //WHERE ( a.LAST_LOAD_TIME > to_date('{2}', 'yyyy/MM/dd hh24:mi:ss') OR a.LAST_ACTIVE_TIME > to_date('{2}', 'yyyy/MM/dd hh24:mi:ss') )
                //AND (a.executions - nvl(b.executions, 0)) > 0 <CRITERIA> 
                //ORDER BY a.LAST_ACTIVE_TIME DESC, a.LAST_LOAD_TIME DESC", OracleDbContext.NewTable, OracleDbContext.OldTable, this.StartOnText);

                string sqlbase = string.Format(@"SELECT a.SQL_ID, a.parsing_schema_name AS SCHEMA, a.module AS MODULE, a.sql_text AS SQL_TEXT 
                , a.sql_fulltext AS SQL_FULLTEXT 
                , a.OPTIMIZER_COST AS OPTIMIZER_COST
                , a.parse_calls AS PARSE_CALLS 
                , a.buffer_gets AS BUFFER_GETS 
                , a.disk_reads AS DISK_READS 
                , decode(a.executions,0,1,a.executions) AS EXECUTIONS 
                , round(a.cpu_time / 1000000, 2) AS CPU_TIME 
                , round(a.cpu_time / (decode(a.executions,0,1,a.executions) * 1000000), 2) AS CPU_TIME_PER_EXE 
                , round(a.elapsed_time / (decode(a.executions,0,1,a.executions) * 1000000), 2) AS ELAPSED_TIME_PER_EXE 
                , a.LAST_LOAD_TIME, a.LAST_ACTIVE_TIME 
                FROM gv$sqlarea a 
                WHERE ( a.LAST_LOAD_TIME > to_date('{0}', 'yyyy/MM/dd hh24:mi:ss') OR a.LAST_ACTIVE_TIME > to_date('{0}', 'yyyy/MM/dd hh24:mi:ss') )
                <CRITERIA> 
                ORDER BY a.LAST_ACTIVE_TIME DESC, a.LAST_LOAD_TIME DESC", this.StartOnText);

                string criteria = "";
                string criteria2 = "";
                if (!string.IsNullOrWhiteSpace(module))
                {
                    IList<string> ms = module.Split(';').Select(a => string.Format("'{0}'", a.ToUpper())).ToList();
                    string modules = string.Join(",", ms);
                    criteria2 = string.Format("AND UPPER(a.module) IN ({0})", modules);
                }

                criteria = criteria2;
                if (this.IsCurrUser)
                {
                    string user = "";
                    string usersql = "SELECT user FROM dual";
                    //DataSet ds = Context.DB.ExecuteDataSet(usersql);
                    //if ((ds != null) && (ds.Tables[0].Rows.Count > 0))
                    //    user = Convert.ToString(ds.Tables[0].Rows[0]["user"]);
                    user = OracleDbHelper.GetSingle(usersql).ToString();

                    criteria = string.Format("AND a.parsing_schema_name = '{0}'", user);
                    if (!string.IsNullOrWhiteSpace(criteria2))
                    {
                        criteria = string.Format("{0} {1}", criteria, criteria2);
                    }
                }

                sqlbase = sqlbase.Replace("<CRITERIA>", criteria);
                if (this.IsSaveOracle && !string.IsNullOrWhiteSpace(tablename))
                {
                    this.Dispatcher.BeginInvoke(new Action(() => this.tb_log.AppendText("Checking table:" + tablename + " ...\r\n")));
                    string sql2 = "SELECT table_name FROM all_tables WHERE table_name = :p0";
                    if (!string.IsNullOrEmpty(OracleDbHelper.GetSingle(sql2, new OracleParameter("@p0", tablename))?.ToString()))
                    {
                        this.Dispatcher.BeginInvoke(new Action(() => this.tb_log.AppendText("Drop table:" + tablename + " ...\r\n")));
                        OracleDbHelper.ExecuteSql($"DROP TABLE {tablename} PURGE");
                    }

                    this.Dispatcher.BeginInvoke(new Action(() => this.tb_log.AppendText("Creating table:" + tablename + " ...\r\n")));
                    string createTable = $"CREATE TABLE {tablename} AS {sqlbase}";
                    //Context.DB.ExecSqlStatement(createTable);
                    OracleDbHelper.ExecuteSql(createTable);
                }

                sql = string.Format("SELECT ROWNUM, t.* FROM ({0}) t WHERE ROWNUM <= 500", sqlbase);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "异常", MessageBoxButton.OK, MessageBoxImage.Error);
                sql = string.Empty;
                return false;
            }
        }

        private DataSet LoadData(string sql)
        {
            //return Context.DB.ExecuteDataSet(sql);
            return OracleDbHelper.Query(sql);
        }

        private async void _dgSQLlines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tb_Status.Clear();
            DataRowView dr = dg_SQLlines.CurrentItem as DataRowView;
            if (dr != null)
            {
                tb_Status.Text = dr["SQL_FULLTEXT"].ToString();

                string sql_id = dr["SQL_ID"].ToString();
                string sql = $"select b.NAME, b.POSITION, b.DATATYPE_STRING, b.VALUE_STRING, b.LAST_CAPTURED, b.WAS_CAPTURED from v$sql_bind_capture b where b.sql_id = '{sql_id}'";
                DataSet ds = await Task.Run(() => LoadData(sql));
                if (ds != null)
                    this.dg_SQLParameters.DataContext = ds.Tables[0];
            }
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void tb_searchtext_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.Enter) // <CTRL>+<ENTER>
            if (e.Key == Key.Enter) // <ENTER>
            {
                if (_dataView == null) return;

                if (string.IsNullOrWhiteSpace(tb_searchtext.Text))
                    _dataView.RowFilter = null;
                else
                {
                    string st = tb_searchtext.Text.Trim();
                    _dataView.RowFilter = $"SQL_FULLTEXT LIKE '%{st}%'";
                }
                this.dg_SQLlines.DataContext = _dataView;
            }
        }

        public static string EscapeLikeValue(string valueWithoutWildcards)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < valueWithoutWildcards.Length; i++)
            {
                char c = valueWithoutWildcards[i];
                if (c == '*' || c == '%' || c == '[' || c == ']')
                    sb.Append("[").Append(c).Append("]");
                else if (c == '\'')
                    sb.Append("''");
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
