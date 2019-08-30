using Genersoft.Platform.Core.DataAccess;
using Genersoft.Platform.Core.DataAccess.Configuration;
using Genersoft.Platform.Core.DataAccess.Oracle;
using System;
using System.Collections.Generic;
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

namespace TFSideKicks
{
    /// <summary>
    /// ProfilerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProfilerWindow : Window
    {
        private string Sysdate;
        private bool IsCurrUser;
        private bool IsSaveOracle;
        public ProfilerWindow()
        {
            InitializeComponent();

            this.Sysdate = "";
            this.IsCurrUser = false;
            this.IsSaveOracle = false;
            this.tb_oraname.IsEnabled = false;
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
            this.tb_log.AppendText("Getting ready...\r\n");
            this.tb_log.AppendText("Clearing data...\r\n");

            string source = tb_source.Text;
            string userid = tb_user.Text;
            string password = tb_password.Password;
            await Task.Run(() => DropTables(source, userid, password));
            this.tb_log.AppendText("Clear data succeed.\r\n");

            this.tb_log.AppendText("Creating data table...\r\n");
            bool onlycurruser = this.cb_curruser.IsChecked.Value;
            await Task.Run(() => CreateTables(onlycurruser));
            this.tb_log.AppendText("Success!\r\n");
            this.tb_log.AppendText("-----------------------------------------------\r\n\r\n");
            this.button2.IsEnabled = true;
        }

        private void DropTables(string source, string userid, string password)
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

        private void CreateTables(bool onlycurruser)
        {
            string createTableSql = "CREATE TABLE " + OracleDbContext.OldTable + " AS SELECT * FROM v$sqlarea";
            Context.DB.ExecSqlStatement(createTableSql);
            this.Dispatcher.BeginInvoke(new Action(() => this.tb_log.AppendText("Create data table succeed.\r\n")));

            string getDateSql = "SELECT to_char(SYSDATE,'yyyy/MM/dd hh24:mi:ss') AS currdate FROM dual";
            DataSet ds = Context.DB.ExecuteDataSet(getDateSql);
            if (ds != null) { this.Sysdate = Convert.ToString(ds.Tables[0].Rows[0]["currdate"]); }
            if (onlycurruser) { this.IsCurrUser = true; }
        }

        private async void button2_Click(object sender, RoutedEventArgs e)
        {
            this.button1.IsEnabled = false;
            this.button2.IsEnabled = false;
            this.tb_log.AppendText("Processing data...\r\n");
            string tablename = tb_oraname.Text.ToUpper();
            string module = tb_module.Text.ToUpper();
            string sql = "";
            bool ret = await Task.Run(() => ProcessData(tablename, module, ref sql));
            if (!ret || string.IsNullOrEmpty(sql)) return;

            if (this.IsSaveOracle)
                this.tb_log.AppendText("Save data to table:" + this.tb_oraname.Text.ToString() + "\r\n");

            DataSet ds_result = await Task.Run(() => LoadData(sql));
            if (ds_result != null)
                this.dg_SQLlines.DataContext = ds_result.Tables[0];

            this.tb_log.AppendText("Success!\r\n");
            this.button1.IsEnabled = true;
        }

        private bool ProcessData(string tablename, string module, ref string sql)
        {
            string createTableSql = "CREATE TABLE " + OracleDbContext.NewTable + " AS SELECT * FROM v$sqlarea";
            Context.DB.ExecSqlStatement(createTableSql);
            string sqlbase = string.Format("SELECT n.parsing_schema_name AS SCHEMA, n.module AS MODULE, n.sql_text AS SQL_TEXT, DBMS_LOB.SUBSTR(n.sql_fulltext, 4000, 1) AS SQL_FULLTEXT, n.parse_calls - nvl(o.parse_calls, 0) AS PARSE_CALLS, n.buffer_gets - nvl(o.buffer_gets, 0) AS BUFFER_GETS, n.disk_reads - nvl(o.disk_reads, 0) AS DISK_READS, n.executions - nvl(o.executions, 0) AS EXECUTIONS, round((n.cpu_time - nvl(o.cpu_time, 0)) / 1000000, 2) AS CPU_TIME, round((n.cpu_time - nvl(o.cpu_time, 0)) / ((n.executions - nvl(o.executions, 0)) * 1000000), 2) AS CPU_TIME_PER_EXE, round((n.elapsed_time - nvl(o.elapsed_time, 0)) / ((n.executions - nvl(o.executions, 0)) * 1000000), 2) AS ELAPSED_TIME_PER_EXE FROM {0} n LEFT JOIN {1} o ON o.hash_value = n.hash_value AND o.address = n.address WHERE n.last_active_time > to_date('{2}', 'yyyy/MM/dd hh24:mi:ss') AND(n.executions - nvl(o.executions, 0)) > 0 <CRITERIA> ORDER BY ELAPSED_TIME_PER_EXE DESC", OracleDbContext.NewTable, OracleDbContext.OldTable, this.Sysdate);

            string criteria = "";
            if (!this.IsCurrUser)
            {
                if (!string.IsNullOrWhiteSpace(module)) { criteria = string.Format("AND n.module = '{0}'", module); }
            }
            else
            {
                string user = "";
                string usersql = "SELECT user FROM dual";
                DataSet ds = Context.DB.ExecuteDataSet(usersql);
                if ((ds != null) && (ds.Tables[0].Rows.Count > 0))
                    user = Convert.ToString(ds.Tables[0].Rows[0]["user"]);

                criteria = string.Format("AND n.parsing_schema_name = '{0}'", user);
                if (!string.IsNullOrWhiteSpace(module)) { criteria = string.Format("{0} AND n.module = '{1}'", criteria, module); }
            }

            sqlbase = sqlbase.Replace("<CRITERIA>", criteria);
            if (this.IsSaveOracle && !string.IsNullOrWhiteSpace(tablename))
            {
                this.Dispatcher.BeginInvoke(new Action(() => this.tb_log.AppendText("Checking table:" + tablename + "...\r\n")));
                string sql1 = "SELECT table_name FROM user_tables WHERE table_name='" + tablename + "'";
                DataSet ds1 = Context.DB.ExecuteDataSet(sql1);
                if (((ds1 != null) && (ds1.Tables[0].Rows.Count > 0)) && (Convert.ToString(ds1.Tables[0].Rows[0]["table_name"]) == tablename))
                {
                    this.Dispatcher.BeginInvoke(new Action(() => this.tb_log.AppendText("Drop table:" + tablename + "...\r\n")));
                    Context.DB.ExecSqlStatement("DROP TABLE " + tablename + " PURGE");
                }
                this.Dispatcher.BeginInvoke(new Action(() => this.tb_log.AppendText("Creating table:" + tablename + "...\r\n")));
                string createTable = string.Format("CREATE TABLE {0} AS {1}", tablename, sqlbase);
                Context.DB.ExecSqlStatement(createTable);
            }

            sql = string.Format("SELECT * FROM ({0}) WHERE ROWNUM <= 100", sqlbase);
            return true;
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
    }
}
