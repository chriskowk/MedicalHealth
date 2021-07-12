using Genersoft.Platform.Core.DataAccess;
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

        private OracleDbContext _context;
        public OracleDbContext Context
        {
            get { return _context; }
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
                Runtimes.ConnectionString = $"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = {host})(PORT = {port}))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = orclbak))); Persist Security Info=True;User ID={userid};Password={password};";
                _context = new OracleDbContext(host, port, service, userid, password);
                //DataTable dt = OracleDbContext.GetDomainSystemTable();
                //int count = OracleDbHelper.ExecuteSql("select * from CORE.DOMAINSYSTEM where SYSTEMKEY = :p0", "cis");
                //DataTable dt1 = OracleDbHelper.ExecuteDataTable("select * from CORE.DOMAINSYSTEM where SYSTEMKEY = :key", new OracleParameter("@key", "emr"));
                string sql1 = "SELECT table_name FROM all_tables WHERE table_name = '" + OracleDbContext.OldTable + "'";
                string sql2 = "SELECT table_name FROM all_tables WHERE table_name = '" + OracleDbContext.NewTable + "'";
                //DataSet ds1 = Context.DB.ExecuteDataSet(sql1);
                //DataSet ds2 = Context.DB.ExecuteDataSet(sql2);
                //if (((ds1 != null) && (ds1.Tables[0].Rows.Count > 0)) && (Convert.ToString(ds1.Tables[0].Rows[0]["table_name"]) == OracleDbContext.OldTable))
                //{
                //    Context.DB.ExecSqlStatement("DROP TABLE " + OracleDbContext.OldTable + " PURGE"); //删除Table不进入Recycle
                //}
                //if (((ds2 != null) && (ds2.Tables[0].Rows.Count > 0)) && (Convert.ToString(ds2.Tables[0].Rows[0]["table_name"]) == OracleDbContext.NewTable))
                //{
                //    Context.DB.ExecSqlStatement("DROP TABLE " + OracleDbContext.NewTable + " PURGE"); //删除Table不进入Recycle
                //}
                string oldtable = OracleDbHelper.GetSingle(sql1)?.ToString();
                string newtable = OracleDbHelper.GetSingle(sql2)?.ToString();
                if (!string.IsNullOrEmpty(oldtable))
                {
                    OracleDbHelper.ExecuteSql("DROP TABLE " + OracleDbContext.OldTable + " PURGE"); //删除Table不进入Recycle
                }
                if (!string.IsNullOrEmpty(newtable))
                {
                    OracleDbHelper.ExecuteSql("DROP TABLE " + OracleDbContext.NewTable + " PURGE"); //删除Table不进入Recycle
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
                string createTableSql = "CREATE TABLE " + OracleDbContext.OldTable + " AS SELECT * FROM v$sql";
                //Context.DB.ExecSqlStatement(createTableSql);
                int count = OracleDbHelper.ExecuteSql(createTableSql);
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
                string createTableSql = "CREATE TABLE " + OracleDbContext.NewTable + " AS SELECT * FROM v$sql";
                //Context.DB.ExecSqlStatement(createTableSql);
                int count = OracleDbHelper.ExecuteSql(createTableSql);

                //string sqlbase = string.Format(@"SELECT a.SQL_ID, a.parsing_schema_name AS SCHEMA, a.module AS MODULE, a.sql_text AS SQL_TEXT 
                //, a.sql_fulltext AS SQL_FULLTEXT 
                //, a.parse_calls - nvl(b.parse_calls, 0) AS PARSE_CALLS 
                //, a.buffer_gets - nvl(b.buffer_gets, 0) AS BUFFER_GETS 
                //, a.disk_reads - nvl(b.disk_reads, 0) AS DISK_READS 
                //, a.executions - nvl(b.executions, 0) AS EXECUTIONS 
                //, round((a.cpu_time - nvl(b.cpu_time, 0)) / 1000000, 2) AS CPU_TIME 
                //, round((a.cpu_time - nvl(b.cpu_time, 0)) / ((a.executions - nvl(b.executions, 0)) * 1000000), 2) AS CPU_TIME_PER_EXE 
                //, round((a.elapsed_time - nvl(b.elapsed_time, 0)) / ((a.executions - nvl(b.executions, 0)) * 1000000), 2) AS ELAPSED_TIME_PER_EXE 
                //, to_date(a.FIRST_LOAD_TIME, 'yyyy-MM-dd hh24:mi:ss') AS FIRST_LOAD_TIME, a.LAST_ACTIVE_TIME 
                //FROM {0} a LEFT JOIN {1} b ON a.hash_value = b.hash_value AND a.address = b.address 
                //WHERE ( to_date(a.FIRST_LOAD_TIME, 'yyyy/MM/dd hh24:mi:ss') > to_date('{2}', 'yyyy/MM/dd hh24:mi:ss') OR a.LAST_ACTIVE_TIME > to_date('{2}', 'yyyy/MM/dd hh24:mi:ss') )
                //AND (a.executions - nvl(b.executions, 0)) > 0 <CRITERIA> 
                //ORDER BY a.LAST_ACTIVE_TIME DESC, a.FIRST_LOAD_TIME DESC", OracleDbContext.NewTable, OracleDbContext.OldTable, this.StartOnText);

                string sqlbase = string.Format(@"SELECT a.SQL_ID, a.parsing_schema_name AS SCHEMA, a.module AS MODULE, a.sql_text AS SQL_TEXT 
                , a.sql_fulltext AS SQL_FULLTEXT 
                , a.parse_calls AS PARSE_CALLS 
                , a.buffer_gets AS BUFFER_GETS 
                , a.disk_reads AS DISK_READS 
                , decode(a.executions,0,1,a.executions) AS EXECUTIONS 
                , round(a.cpu_time / 1000000, 2) AS CPU_TIME 
                , round(a.cpu_time / (decode(a.executions,0,1,a.executions) * 1000000), 2) AS CPU_TIME_PER_EXE 
                , round(a.elapsed_time / (decode(a.executions,0,1,a.executions) * 1000000), 2) AS ELAPSED_TIME_PER_EXE 
                , to_date(a.FIRST_LOAD_TIME, 'yyyy-MM-dd hh24:mi:ss') AS FIRST_LOAD_TIME, a.LAST_ACTIVE_TIME 
                FROM v$sql a 
                WHERE ( to_date(a.FIRST_LOAD_TIME, 'yyyy/MM/dd hh24:mi:ss') > to_date('{0}', 'yyyy/MM/dd hh24:mi:ss') OR a.LAST_ACTIVE_TIME > to_date('{0}', 'yyyy/MM/dd hh24:mi:ss') )
                <CRITERIA> 
                ORDER BY a.LAST_ACTIVE_TIME DESC, a.FIRST_LOAD_TIME DESC", this.StartOnText);

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
                    string sql2 = "SELECT table_name FROM all_tables WHERE table_name='" + tablename + "'";
                    //DataSet ds1 = Context.DB.ExecuteDataSet(sql2);
                    //if (((ds1 != null) && (ds1.Tables[0].Rows.Count > 0)) && (Convert.ToString(ds1.Tables[0].Rows[0]["table_name"]) == tablename))
                    //{
                    //    this.Dispatcher.BeginInvoke(new Action(() => this.tb_log.AppendText("Drop table:" + tablename + " ...\r\n")));
                    //    Context.DB.ExecSqlStatement("DROP TABLE " + tablename + " PURGE");
                    //}
                    string savetable = OracleDbHelper.GetSingle(sql2)?.ToString();
                    if (!string.IsNullOrEmpty(savetable))
                    {
                        this.Dispatcher.BeginInvoke(new Action(() => this.tb_log.AppendText("Drop table:" + tablename + " ...\r\n")));
                        OracleDbHelper.ExecuteSql("DROP TABLE " + tablename + " PURGE");
                    }

                    this.Dispatcher.BeginInvoke(new Action(() => this.tb_log.AppendText("Creating table:" + tablename + " ...\r\n")));
                    string createTable = string.Format("CREATE TABLE {0} AS {1}", tablename, sqlbase);
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
                string sql = $"select b.NAME, b.POSITION, b.DATATYPE_STRING, b.VALUE_STRING, b.LAST_CAPTURED from v$sql_bind_capture b where b.sql_id = '{sql_id}'";
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
