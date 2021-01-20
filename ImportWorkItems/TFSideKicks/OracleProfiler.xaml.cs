using Genersoft.Platform.Core.DataAccess;
using Genersoft.Platform.Core.DataAccess.Configuration;
using Genersoft.Platform.Core.DataAccess.Oracle;
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
        private string Sysdate;
        private bool IsCurrUser;
        private bool IsSaveOracle;
        public OracleProfiler()
        {
            InitializeComponent();

            this.Sysdate = "";
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
            tb_source.Focus();
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

            string source = tb_source.Text;
            string userid = tb_user.Text;
            string password = tb_password.Password;
            await Task.Run(() => DropTables(source, userid, password));
            this.tb_log.AppendText("Clear data succeed.\r\n");

            this.tb_log.AppendText("Creating data table ...\r\n");
            bool onlycurruser = this.cb_curruser.IsChecked.Value;
            await Task.Run(() => CreateTables(onlycurruser));
            this.tb_log.AppendText("Success!\r\n");
            this.tb_log.AppendText("-----------------------------------------------\r\n");
            this.button2.IsEnabled = true;
        }

        private void DropTables(string source, string userid, string password)
        {
            try
            {
                _context = new OracleDbContext(source, userid, password);
                string sql1 = "SELECT table_name FROM user_tables WHERE table_name='" + OracleDbContext.OldTable + "'";
                string sql2 = "SELECT table_name FROM user_tables WHERE table_name='" + OracleDbContext.NewTable + "'";
                DataSet ds1 = Context.DB.ExecuteDataSet(sql1);
                DataSet ds2 = Context.DB.ExecuteDataSet(sql2);
                if (((ds1 != null) && (ds1.Tables[0].Rows.Count > 0)) && (Convert.ToString(ds1.Tables[0].Rows[0]["table_name"]) == OracleDbContext.OldTable))
                {
                    Context.DB.ExecSqlStatement("DROP TABLE " + OracleDbContext.OldTable + " PURGE");
                }
                if (((ds2 != null) && (ds2.Tables[0].Rows.Count > 0)) && (Convert.ToString(ds2.Tables[0].Rows[0]["table_name"]) == OracleDbContext.NewTable))
                {
                    Context.DB.ExecSqlStatement("DROP TABLE " + OracleDbContext.NewTable + " PURGE");
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
                Context.DB.ExecSqlStatement(createTableSql);
                this.Dispatcher.BeginInvoke(new Action(() => this.tb_log.AppendText("Create data table succeed.\r\n")));

                string getDateSql = "SELECT to_char(SYSDATE,'yyyy/MM/dd hh24:mi:ss') AS currdate FROM dual";
                DataSet ds = Context.DB.ExecuteDataSet(getDateSql);
                if (ds != null) { this.Sysdate = Convert.ToString(ds.Tables[0].Rows[0]["currdate"]); }
                if (onlycurruser) { this.IsCurrUser = true; }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "异常", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private async void button2_Click(object sender, RoutedEventArgs e)
        {
            this.button1.IsEnabled = false;
            this.button2.IsEnabled = false;
            this.tb_log.AppendText("Processing data ...\r\n");
            string tablename = tb_oraname.Text.ToUpper();
            string module = tb_module.Text.ToUpper();
            string sql = string.Empty;
            bool ret = await Task.Run(() => ProcessData(tablename, module, out sql));
            if (!ret || string.IsNullOrEmpty(sql)) return;

            if (this.IsSaveOracle)
                this.tb_log.AppendText("Save data to table:" + this.tb_oraname.Text.ToString() + "\r\n");

            DataSet ds_result = await Task.Run(() => LoadData(sql));
            if (ds_result != null)
                this.dg_SQLlines.DataContext = ds_result.Tables[0];

            this.tb_log.AppendText("Success!");
            this.button1.IsEnabled = true;
        }

        private bool ProcessData(string tablename, string module, out string sql)
        {
            try
            {
                string createTableSql = "CREATE TABLE " + OracleDbContext.NewTable + " AS SELECT * FROM v$sql";
                Context.DB.ExecSqlStatement(createTableSql);

                string sqlbase = string.Format(@"SELECT n.parsing_schema_name AS SCHEMA, n.module AS MODULE, n.sql_text AS SQL_TEXT 
                , DBMS_LOB.SUBSTR(n.sql_fulltext, 4000, 1) AS SQL_FULLTEXT 
                , n.parse_calls - nvl(o.parse_calls, 0) AS PARSE_CALLS 
                , n.buffer_gets - nvl(o.buffer_gets, 0) AS BUFFER_GETS 
                , n.disk_reads - nvl(o.disk_reads, 0) AS DISK_READS 
                , n.executions - nvl(o.executions, 0) AS EXECUTIONS 
                , round((n.cpu_time - nvl(o.cpu_time, 0)) / 1000000, 2) AS CPU_TIME 
                , round((n.cpu_time - nvl(o.cpu_time, 0)) / ((n.executions - nvl(o.executions, 0)) * 1000000), 2) AS CPU_TIME_PER_EXE 
                , round((n.elapsed_time - nvl(o.elapsed_time, 0)) / ((n.executions - nvl(o.executions, 0)) * 1000000), 2) AS ELAPSED_TIME_PER_EXE 
                , to_date(n.FIRST_LOAD_TIME, 'yyyy-MM-dd hh24:mi:ss') AS FIRST_LOAD_TIME, n.LAST_ACTIVE_TIME 
                FROM {0} n LEFT JOIN {1} o ON o.hash_value = n.hash_value AND o.address = n.address 
                WHERE ( to_date(n.FIRST_LOAD_TIME, 'yyyy/MM/dd hh24:mi:ss') > to_date('{2}', 'yyyy/MM/dd hh24:mi:ss') OR n.LAST_ACTIVE_TIME > to_date('{2}', 'yyyy/MM/dd hh24:mi:ss') )
                AND (n.executions - nvl(o.executions, 0)) > 0 <CRITERIA> 
                ORDER BY n.LAST_ACTIVE_TIME DESC, n.FIRST_LOAD_TIME DESC", OracleDbContext.NewTable, OracleDbContext.OldTable, this.Sysdate);

                string criteria = "";
                string criteria2 = "";
                if (!string.IsNullOrWhiteSpace(module))
                {
                    IList<string> ms = module.Split(';').Select(a => string.Format("'{0}'", a.ToUpper())).ToList();
                    string modules = string.Join(",", ms);
                    criteria2 = string.Format("AND UPPER(n.module) IN ({0})", modules);
                }

                criteria = criteria2;
                if (this.IsCurrUser)
                {
                    string user = "";
                    string usersql = "SELECT user FROM dual";
                    DataSet ds = Context.DB.ExecuteDataSet(usersql);
                    if ((ds != null) && (ds.Tables[0].Rows.Count > 0))
                        user = Convert.ToString(ds.Tables[0].Rows[0]["user"]);

                    criteria = string.Format("AND n.parsing_schema_name = '{0}'", user);
                    if (!string.IsNullOrWhiteSpace(criteria2))
                    {
                        criteria = string.Format("{0} {1}", criteria, criteria2);
                    }
                }

                sqlbase = sqlbase.Replace("<CRITERIA>", criteria);
                if (this.IsSaveOracle && !string.IsNullOrWhiteSpace(tablename))
                {
                    this.Dispatcher.BeginInvoke(new Action(() => this.tb_log.AppendText("Checking table:" + tablename + " ...\r\n")));
                    string sql1 = "SELECT table_name FROM user_tables WHERE table_name='" + tablename + "'";
                    DataSet ds1 = Context.DB.ExecuteDataSet(sql1);
                    if (((ds1 != null) && (ds1.Tables[0].Rows.Count > 0)) && (Convert.ToString(ds1.Tables[0].Rows[0]["table_name"]) == tablename))
                    {
                        this.Dispatcher.BeginInvoke(new Action(() => this.tb_log.AppendText("Drop table:" + tablename + " ...\r\n")));
                        Context.DB.ExecSqlStatement("DROP TABLE " + tablename + " PURGE");
                    }
                    this.Dispatcher.BeginInvoke(new Action(() => this.tb_log.AppendText("Creating table:" + tablename + " ...\r\n")));
                    string createTable = string.Format("CREATE TABLE {0} AS {1}", tablename, sqlbase);
                    Context.DB.ExecSqlStatement(createTable);
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
            return Context.DB.ExecuteDataSet(sql);
        }

        private void _dgSQLlines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tb_Status.Clear();
            DataRowView dr = dg_SQLlines.CurrentItem as DataRowView;
            if (dr != null) tb_Status.Text = dr["SQL_FULLTEXT"].ToString();
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void mcbo_module_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ObservableCollection<MultiComboBox.MultiCbxBaseData> context = mcbo_module.ItemsSource as ObservableCollection<MultiComboBox.MultiCbxBaseData>;
            string s = string.Join(";", context.Where(a => a.IsChecked));
            this.tb_module.Text = s;
        }
    }
}
