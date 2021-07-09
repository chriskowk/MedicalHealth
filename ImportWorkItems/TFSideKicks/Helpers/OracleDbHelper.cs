using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Configuration;
using Oracle.ManagedDataAccess;
using Oracle.ManagedDataAccess.Types;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Reflection;
using System.Data.Linq.Mapping;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TFSideKicks
{
    /// <summary>
    /// 数据访问基础类(基于Oracle) Copyright (C) Maticsoft
    /// 可以用户可以修改满足自己项目的需要。
    /// </summary>
    public abstract class OracleDbHelper
    {
        //数据库连接字符串(web.config来配置)，可以动态更改connectionString支持多数据库.		
        private static string _userid = "apps";
        private static string _password = "jetsun";
        public static string _connectionString = $"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = 172.18.99.243)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = orclbak))); Persist Security Info=True;User ID={_userid};Password={_password};";
        public OracleDbHelper()
        {
        }

        public static int GetMaxID(string FieldName, string TableName)
        {
            string strsql = "select max(" + FieldName + ")+1 from " + TableName;
            object obj = GetSingle(strsql);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }
        public static bool Exists(string strSql)
        {
            object obj = GetSingle(strSql);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool Exists(string strSql, params OracleParameter[] cmdParms)
        {
            object obj = GetSingle(strSql, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString)
        {
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                using (OracleCommand cmd = new OracleCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (OracleException E)
                    {
                        connection.Close();
                        throw new Exception(E.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>		
        public static void ExecuteSqlTran(ArrayList SQLStringList)
        {
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                OracleTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                }
                catch (OracleException E)
                {
                    tx.Rollback();
                    throw new Exception(E.Message);
                }
            }
        }

        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, string content)
        {
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                OracleCommand cmd = new OracleCommand(SQLString, connection);
                OracleParameter myParameter = new OracleParameter("@content", OracleDbType.Varchar2, ParameterDirection.Input);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    //OracleDataAdapter oda = new OracleDataAdapter(cmd);
                    //DataSet ds = new DataSet();
                    //oda.Fill(ds);
                    //DataTable dt = ds.Tables[0];

                    return rows;
                }
                catch (OracleException e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
        /// <summary>
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSqlInsertImg(string strSQL, byte[] fs)
        {
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                OracleCommand cmd = new OracleCommand(strSQL, connection);
                OracleParameter myParameter = new OracleParameter("@fs", OracleDbType.LongRaw);
                myParameter.Value = fs;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (OracleException E)
                {
                    throw new Exception(E.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString)
        {
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                using (OracleCommand cmd = new OracleCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (OracleException e)
                    {
                        connection.Close();
                        throw new Exception(e.Message);
                    }
                }
            }
        }
        /// <summary>
        /// 执行查询语句，返回OracleDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>OracleDataReader</returns>
        public static OracleDataReader ExecuteReader(string strSQL)
        {
            OracleConnection connection = new OracleConnection(_connectionString);
            OracleCommand cmd = new OracleCommand(strSQL, connection);
            try
            {
                connection.Open();
                OracleDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch (OracleException e)
            {
                throw new Exception(e.Message);
            }

        }
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString)
        {
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    OracleDataAdapter command = new OracleDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (OracleException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, params OracleParameter[] cmdParms)
        {
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (OracleException E)
                    {
                        throw new Exception(E.Message);
                    }
                }
            }
        }


        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的OracleParameter[]）</param>
        public static void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                conn.Open();
                using (OracleTransaction trans = conn.BeginTransaction())
                {
                    OracleCommand cmd = new OracleCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            OracleParameter[] cmdParms = (OracleParameter[])myDE.Value;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                            trans.Commit();
                        }
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }


        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString, params OracleParameter[] cmdParms)
        {
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (OracleException e)
                    {
                        throw new Exception(e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回OracleDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>OracleDataReader</returns>
        public static OracleDataReader ExecuteReader(string SQLString, params OracleParameter[] cmdParms)
        {
            OracleConnection connection = new OracleConnection(_connectionString);
            OracleCommand cmd = new OracleCommand();
            try
            {
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                OracleDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (OracleException e)
            {
                throw new Exception(e.Message);
            }

        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString, params OracleParameter[] cmdParms)
        {
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (OracleException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }


        private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, string cmdText, OracleParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (OracleParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        /// <summary>
        /// 执行存储过程 返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OracleDataReader</returns>
        public static OracleDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            OracleConnection connection = new OracleConnection(_connectionString);
            OracleDataReader returnReader;
            connection.Open();
            OracleCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.CommandType = CommandType.StoredProcedure;
            returnReader = command.ExecuteReader(CommandBehavior.CloseConnection);
            return returnReader;
        }


        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                OracleDataAdapter sqlDA = new OracleDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }


        /// <summary>
        /// 构建 OracleCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OracleCommand</returns>
        private static OracleCommand BuildQueryCommand(OracleConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            OracleCommand command = new OracleCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (OracleParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            return command;
        }

        /// <summary>
        /// 执行存储过程，返回影响的行数		
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public static int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                int result;
                connection.Open();
                OracleCommand command = BuildIntCommand(connection, storedProcName, parameters);
                rowsAffected = command.ExecuteNonQuery();
                result = (int)command.Parameters["ReturnValue"].Value;
                //Connection.Close();
                return result;
            }
        }

        /// <summary>
        /// 创建 OracleCommand 对象实例(用来返回一个整数值)	
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OracleCommand 对象实例</returns>
        private static OracleCommand BuildIntCommand(OracleConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            OracleCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.Parameters.Add(new OracleParameter("ReturnValue",
                 OracleDbType.Int32, 4, ParameterDirection.ReturnValue,
                false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }

        public static int ExecuteNonQuery(string sql, params OracleParameter[] parameters)
        {
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                conn.Open();
                using (OracleCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 执行SQL语句,返回DataTable;只用来执行查询结果比较少的情况
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(string sql, params OracleParameter[] parameters)
        {
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                conn.Open();
                using (OracleCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.AddRange(parameters);
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable datatable = new DataTable();
                    adapter.Fill(datatable);
                    return datatable;
                }
            }
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sSQL">SQL语句</param>
        /// <param name="Params">参数列表</param>
        /// <returns>查询到的数据DataSet</returns>
        public static DataSet ExecuteSQL(string sSQL, object[][] Params)
        {
            try
            {
                DataSet ds = new DataSet();
                OracleDataAdapter adapter = new OracleDataAdapter(sSQL, _connectionString);
                if (Params != null)
                {
                    for (int i = 0; i <= Params.Length - 1; i++)
                    {
                        OracleParameter oraclePar = new OracleParameter();
                        oraclePar.Direction = ParameterDirection.Input;
                        try
                        {
                            oraclePar.OracleDbType = (OracleDbType)Params[i][1];
                        }
                        catch
                        {
                            switch (Params[i][1].ToString())
                            {
                                case "1":
                                    oraclePar.OracleDbType = OracleDbType.Varchar2;
                                    break;
                                case "2":
                                    oraclePar.OracleDbType = OracleDbType.Int32;
                                    break;
                                case "3":
                                    oraclePar.OracleDbType = OracleDbType.Date;
                                    break;
                                default:
                                    oraclePar.OracleDbType = OracleDbType.Varchar2;
                                    break;
                            }
                        }
                        oraclePar.ParameterName = Params[i][2].ToString().ToUpper();
                        oraclePar.Value = Params[i][3];
                        adapter.SelectCommand.Parameters.Add(oraclePar);
                    }
                }
                try
                {
                    adapter.Fill(ds);
                    return ds;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    ds.Dispose();
                    adapter.Dispose();
                }
            }
            finally
            {
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="sProcName">存储过程名称</param>
        /// <param name="Params">参数列表</param>
        /// <returns>返回的数据</returns>
        public static DataSet ExecuteProc(string sProcName, object[][] Params)
        {
            try
            {
                OracleConnection OraConnection = new OracleConnection(_connectionString);
                OracleCommand oraCmd = new OracleCommand("", OraConnection);
                oraCmd.CommandText = sProcName;

                oraCmd.CommandType = CommandType.StoredProcedure;
                DataSet ds = new DataSet();
                DataTable dsTable = new DataTable();
                DataRow dr = dsTable.NewRow();
                OracleDataAdapter adapter = null;
                for (int i = 0; i <= Params.Length - 1; i++)
                {
                    OracleParameter oraclePar = new OracleParameter();
                    try
                    {
                        oraclePar.Direction = (ParameterDirection)Params[i][0];
                    }
                    catch
                    {
                        switch (Params[i][0].ToString().ToUpper())
                        {
                            case "INPUT":
                                oraclePar.Direction = ParameterDirection.Input;
                                break;
                            case "OUTPUT":
                                oraclePar.Direction = ParameterDirection.Output;
                                break;
                            default:
                                oraclePar.Direction = ParameterDirection.Input;
                                break;
                        }
                    }

                    try
                    {
                        oraclePar.OracleDbType = (OracleDbType)Params[i][1];
                    }
                    catch
                    {
                        switch (Params[i][1].ToString())
                        {
                            case "1":
                                oraclePar.OracleDbType = OracleDbType.Varchar2;
                                break;
                            case "2":
                                oraclePar.OracleDbType = OracleDbType.Int32;
                                break;
                            case "3":
                                oraclePar.OracleDbType = OracleDbType.Date;
                                break;
                            default:
                                oraclePar.OracleDbType = OracleDbType.Varchar2;
                                break;
                        }
                    }
                    oraclePar.ParameterName = Params[i][2].ToString().ToUpper();
                    oraclePar.Size = Params[i][3].ToString().Length;
                    if (oraclePar.Direction == ParameterDirection.Output)
                    {
                        oraclePar.Size = 1000;
                    }
                    oraclePar.Value = Params[i][3].ToString();
                    oraCmd.Parameters.Add(oraclePar);
                }

                adapter = new OracleDataAdapter(oraCmd);
                try
                {
                    adapter.Fill(ds);
                    for (int i = 0; i <= adapter.SelectCommand.Parameters.Count - 1; i++)
                    {
                        if (adapter.SelectCommand.Parameters[i].Direction == ParameterDirection.Output)
                        {
                            dsTable.Columns.Add(adapter.SelectCommand.Parameters[i].ParameterName, typeof(string));
                            dr[adapter.SelectCommand.Parameters[i].ParameterName] =
                                adapter.SelectCommand.Parameters[i].Value;
                        }
                    }
                    dsTable.Rows.Add(dr);
                    ds.Tables.Clear();
                    ds.Tables.Add(dsTable);
                    return ds;
                }
                finally
                {
                    OraConnection.Dispose();
                    oraCmd.Dispose();
                    ds.Dispose();
                    dsTable.Dispose();
                    adapter.Dispose();
                }
            }
            finally
            {
            }
        }

        /// <summary>
        /// 示例1 ：执行SQL语句 使用ExecuteSQL 附加参数列表Params为空
        /// 获取服务器时间，
        /// </summary>
        /// <returns>返回服务器时间</returns>
        public static DateTime GetServerDate()
        {
            DateTime dtNow;
            string sSQL = @" SELECT SYSDATE FROM DUAL ";
            dtNow = (DateTime)(ExecuteSQL(sSQL, null).Tables[0].Rows[0][0]); //
            return dtNow;
        }

        /// <summary>
        /// 示例2： 执行SQL语句 使用ExecuteSQL 附加参数列表Params不为空
        /// 更新数据 
        /// </summary>
        /// <param name="sTable"></param>
        /// <param name="sRackLoc"></param>
        /// <param name="sStatus"></param>
        /// <returns></returns>
        public static DataSet UpdateStatus(string sTable, string sLocation, string sStatus)
        {
            object[][] Params = new object[3][];
            string sSQL = string.Format(@" UPDATE {0} 
                                     SET STATUS = :STATUS
                                       WHERE ID = :ID 
                                        AND LOC = :LOC ", sTable);
            Params[0] = new object[] { ParameterDirection.Input, OracleDbType.Varchar2, "RACK_STATUS", sStatus };
            Params[1] = new object[] { ParameterDirection.Input, OracleDbType.Varchar2, "PDLINE_ID", "1" };
            Params[2] = new object[] { ParameterDirection.Input, OracleDbType.Varchar2, "CRANE_LOC", sLocation };
            return ExecuteSQL(sSQL, Params);
        }

        /// <summary>
        /// 执行返回一行一列的数据库操作
        /// </summary>
        /// <param name="commandText">Oracle语句或存储过程名</param>
        /// <param name="commandType">Oracle命令类型</param>
        /// <param name="param">Oracle命令参数数组</param>
        /// <returns>第一行第一列的记录</returns>
        public static int ExecuteScalar(string commandText, CommandType commandType, params OracleParameter[] param)
        {
            int count = 0;
            using (var conn = new OracleConnection(_connectionString))
            {
                using (var cmd = new OracleCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(param);
                    conn.Open();
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return count;
        }

        /// <summary>
        /// 执行不查询的数据库操作
        /// </summary>
        /// <param name="commandText">Oracle语句或存储过程名</param>
        /// <param name="commandType">Oracle命令类型</param>
        /// <param name="param">Oracle命令参数数组</param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(string commandText, CommandType commandType, params OracleParameter[] param)
        {
            int result = 0;
            using (var conn = new OracleConnection(_connectionString))
            {
                using (var cmd = new OracleCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(param);
                    conn.Open();
                    result = cmd.ExecuteNonQuery();
                }
            }
            return result;
        }

        /// <summary>
        /// 执行返回一条记录的泛型对象
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="reader">只进只读对象</param>
        /// <returns>泛型对象</returns>
        private static T ExecuteDataReader<T>(IDataReader reader)
        {
            T obj = default(T);

            Type type = typeof(T);
            obj = (T)Activator.CreateInstance(type); //从当前程序集里面通过反射的方式创建指定类型的对象   
            //obj = (T)Assembly.Load(OracleHelper._assemblyName).CreateInstance(OracleHelper._assemblyName + "." + type.Name);//从另一个程序集里面通过反射的方式创建指定类型的对象 
            var propertyInfos = type.GetProperties(); //获取指定类型里面的所有属性
            foreach (var propertyInfo in propertyInfos)
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var fieldName = reader.GetName(i);
                    if (fieldName.ToLower() == propertyInfo.Name.ToLower())
                    {
                        var val = reader[propertyInfo.Name.ToUpper()]; //读取表中某一条记录里面的某一列信息
                        if (val != null && val != DBNull.Value)
                            propertyInfo.SetValue(obj,
                                Convert.ChangeType(val, propertyInfo.PropertyType.IsGenericType
                                    ? propertyInfo.PropertyType.GetGenericArguments()[0]
                                    : propertyInfo.PropertyType), null); //给对象的某一个属性赋值
                        break;
                    }
                }
            }
            return obj;
        }

        /// <summary>
        /// 执行返回一条记录的泛型对象
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="commandText">Oracle语句或存储过程名</param>
        /// <param name="commandType">Oracle命令类型</param>
        /// <param name="param">Oracle命令参数数组</param>
        /// <returns>实体对象</returns>
        public static T ExecuteEntity<T>(string commandText, CommandType commandType, params OracleParameter[] param)
        {
            T obj = default(T);
            using (var conn = new OracleConnection(_connectionString))
            {
                using (var cmd = new OracleCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    if (param != null && param.Length > 0)
                    {
                        cmd.Parameters.AddRange(param);
                    }
                    conn.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        obj = OracleDbHelper.ExecuteDataReader<T>(reader);
                    }
                }
            }
            return obj;
        }

        /// <summary>
        /// 执行返回多条记录的泛型集合对象
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="commandText">Oracle语句或存储过程名</param>
        /// <param name="commandType">Oracle命令类型</param>
        /// <param name="param">Oracle命令参数数组</param>
        /// <returns>泛型集合对象</returns>
        public static IList<T> ExecuteList<T>(string commandText, CommandType commandType, params OracleParameter[] param)
        {
            IList<T> list = new List<T>();
            using (var conn = new OracleConnection(_connectionString))
            {
                using (var cmd = new OracleCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    if (param != null && param.Length > 0)
                    {
                        cmd.Parameters.AddRange(param);
                    }
                    conn.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        T obj = OracleDbHelper.ExecuteDataReader<T>(reader);
                        list.Add(obj);
                    }
                    reader.Close();
                }
            }
            return list;
        }

        /// <summary>
        /// 执行返回数据的第一行第一列
        /// </summary>
        /// <param name="commandText">查询语句</param>
        /// <returns></returns>
        public static int ExecuteScalar(string commandText)
        {
            return ExecuteScalar(commandText, CommandType.Text);
        }

        /// <summary>
        /// 执行查询返回一个泛型集合
        /// </summary>
        /// <typeparam name="T">单条数据模型</typeparam>
        /// <param name="commandText">查询命令</param>
        /// <returns>查询结果集</returns>
        public static IList<T> ExecuteList<T>(string commandText)
        {
            return ExecuteList<T>(commandText, CommandType.Text);
        }

        /// <summary>
        /// 执行查询以返回单条记录
        /// </summary>
        /// <typeparam name="T">数据模型</typeparam>
        /// <param name="commandText">查询命令</param>
        /// <returns>查询结果对象</returns>
        public static T ExecuteEntity<T>(string commandText)
        {
            return ExecuteEntity<T>(commandText, CommandType.Text);
        }

        /// <summary>
        /// 执行不返回结果集的查询
        /// </summary>
        /// <param name="commandText">查询命令</param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(commandText, CommandType.Text);
        }

        /// <summary>
        /// CodeFirst 数据检查和自行
        /// </summary>
        /// <param name="autoExcute">是否自动执行数据迁移</param>
        /// <returns></returns>
        public static bool CodeFirstVerify(bool autoExcute)
        {
            return CodeFirstMigrate();
        }

        /// <summary>
        /// CodeFirst 数据迁移
        /// </summary>
        /// <returns></returns>
        public static bool CodeFirstMigrate()
        {
            var asm = Assembly.GetExecutingAssembly();
            var types = asm.GetExportedTypes().ToList();
            try
            {
                // App_Code 文件夹
                var asm1 = Assembly.Load("__code");
                types.AddRange(asm1.GetExportedTypes());
            }
            catch
            {
            }
            // 表类型判断委托
            Func<Attribute[], bool> IsTable = o =>
            {
                foreach (Attribute a in o)
                {
                    if (a is System.ComponentModel.DataAnnotations.Schema.TableAttribute)
                        return true;
                }
                return false;
            };

            var CosType = types.Where(o =>
            {
                var attri = Attribute.GetCustomAttributes(o, false);
                return IsTable(attri);
            });

            foreach (var type in CosType)
            {
                //OracleTransaction trans = new OracleTransaction();
                // 迁移符合条件的类型
                var table = GetObjectFromAttribute<System.ComponentModel.DataAnnotations.Schema.TableAttribute>(type);
                if (!string.IsNullOrWhiteSpace(table.Name))
                {
                    // 检查模型和数据库以进行迁移
                    var tableName = table.Name.ToUpper();
                    // 查询数据库是否存在此表
                    var count = ExecuteScalar(
                        "SELECT COUNT(*) FROM USER_TABLES WHERE TABLE_NAME = '" + tableName + "'");
                    if (count == 0)
                    {
                        // 创建数据表
                        var createTable = new StringBuilder();
                        var createSequence = new StringBuilder();
                        var createTrigger = new StringBuilder();
                        var enableTrigger = "";

                        createTable.AppendLine("CREATE TABLE \"" + tableName + "\" (");
                        // 表格模型成员列表
                        var props = type.GetProperties();
                        var priKeys = new List<string>();
                        var identis = new List<string>();
                        foreach (var prop in props)
                        {
                            bool isKey; // 是否主键标志
                            bool isIdent; // 是否标识列
                            createTable.AppendLine("\"" + prop.Name.ToUpper() + "\" "
                                                   + ConvertToOracleType(prop, out isKey, out isIdent) + ",");
                            if (isKey)
                                priKeys.Add(prop.Name.ToUpper());
                            if (isIdent)
                                identis.Add(prop.Name.ToUpper());
                        }
                        if (priKeys.Count != 0)
                            createTable.AppendLine(" CONSTRAINT \"" + tableName + "_PK\" PRIMARY KEY (\"" + priKeys[0] +
                                                   "\") ENABLE");
                        createTable.AppendLine(") ");
                        var resultCount = ExecuteNonQuery(createTable.ToString());
                        //createTable.AppendLine("/");
                        createSequence.AppendLine("CREATE SEQUENCE \"" + tableName + "_SEQ\" MINVALUE 1 " +
                                                  "MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 1 NOCACHE ORDER NOCYCLE");
                        resultCount = ExecuteNonQuery(createSequence.ToString());
                        //createTable.AppendLine("/");
                        if (identis.Count != 0)
                        {
                            createTrigger.AppendLine("CREATE OR REPLACE TRIGGER  \"BI_" + tableName + "\" ");
                            createTrigger.AppendLine("  before insert on \"" + tableName + "\" ");
                            createTrigger.AppendLine("  for each row");
                            createTrigger.AppendLine("begin");
                            createTrigger.AppendLine("  if :NEW.\"" + identis[0] + "\" is null then");
                            createTrigger.AppendLine("    select \"" + tableName + "_SEQ\".nextval into :NEW.\"" +
                                                     identis[0] + "\" from sys.dual;");
                            createTrigger.AppendLine("  end if;");
                            createTrigger.AppendLine("end;");
                            resultCount = ExecuteNonQuery(createTrigger.ToString());
                            //createTable.AppendLine("/");
                            enableTrigger = "ALTER TRIGGER \"BI_" + tableName + "\" ENABLE";
                            resultCount = ExecuteNonQuery(enableTrigger);
                            //createTable.AppendLine("/");
                        }
                        //resultCount = ExecuteNonQuery(createTable.ToString());
                    }
                    else
                    {
                        // 表结构修改
                        // 查询现有表列
                        var cols = ExecuteList<ColumnModel>(
                            "SELECT * FROM USER_TAB_COLUMNS WHERE TABLE_NAME = '" + tableName + "' ORDER BY COLUMN_ID");
                        // 获取模型属性名集合
                        var propNames = new List<string>();
                        var colNames = new List<string>();
                        foreach (var prop in type.GetProperties())
                        {
                            propNames.Add(prop.Name.ToUpper());
                        }
                        foreach (var col in cols)
                        {
                            colNames.Add(col.Column_Name.ToUpper());
                        }
                        // 表格字段和屬性列表的差集
                        var excepts = propNames.Except(colNames).ToList();
                        foreach (var prop in type.GetProperties())
                        {
                            var propName = prop.Name.ToUpper();
                            bool isKey; // 是否主键标志
                            bool isIdent; // 是否标识列
                            var colType = ConvertToOracleType(prop, out isKey, out isIdent);

                            if (excepts.Contains(propName))
                                // 添加数据字段
                                ExecuteNonQuery("alter table \"" + tableName
                                                + "\" add (\"" + propName + "\" " + colType + ")");
                        }
                        excepts = colNames.Except(propNames).ToList();
                        foreach (var col in cols)
                        {
                            if (excepts.Contains(col.Column_Name))
                                // 删除数据字段
                                ExecuteNonQuery("alter table \"" + tableName
                                                + "\" drop column \"" + col.Column_Name + "\"");
                            else
                            {
                                bool isKey; // 是否主键标志
                                bool isIdent; // 是否标识列
                                var props = type.GetProperties();
                                var colType = ConvertToOracleType(
                                    props.Where(a => a.Name.ToUpper() == col.Column_Name).First(), out isKey,
                                    out isIdent);
                                if (colType != col.Data_Type)
                                {
                                    if (col.Data_Type == "NUMBER" && col.Data_Scale != null)
                                    {
                                        // Decimal 类型比较
                                        if (colType != "NUMBER(38," + col.Data_Scale + ")")
                                            ExecuteNonQuery("alter table \"" + tableName
                                                            + "\" modify (\"" + col.Column_Name + "\" " + colType + ")");
                                    }
                                    else
                                    {
                                        try
                                        {
                                            // 修改字段类型
                                            ExecuteNonQuery("alter table \"" + tableName
                                                            + "\" modify (\"" + col.Column_Name + "\" " + colType + ")");
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                    throw new Exception("Oracle 数据表模型 " + type.Name + " 未设置表名！");
            }

            return true;
        }

        /// <summary>
        /// 将常用数据类型转换为 Oracle 数据类型
        /// </summary>
        /// <param name="type">系统数据类型</param>
        /// <param name="isKey">获取成员是否主键</param>
        /// <param name="isIdent">是否标识列</param>
        /// <returns></returns>
        static string ConvertToOracleType(PropertyInfo propInfo, out bool isKey, out bool isIdent)
        {
            isKey = false;
            isIdent = false;

            var type = propInfo.PropertyType;
            switch (type.Name)
            {
                case "Int32":
                    // 主键检查
                    var key = GetObjectFromAttribute<KeyAttribute>(propInfo);
                    if (key != null)
                    {
                        isKey = true;
                        // 标识列
                        var identity = GetObjectFromAttribute<DatabaseGeneratedAttribute>(propInfo);
                        if (identity != null && identity.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
                            isIdent = true;
                        return "NUMBER";
                    }
                    else
                        return "NUMBER NOT NULL ENABLE";
                case "String":
                    var returnStr = "";
                    // 字符串长度
                    var lenAttr = GetObjectFromAttribute<StringLengthAttribute>(propInfo);
                    if (lenAttr == null)
                        returnStr += "NVARCHAR2(2000)";
                    else
                        returnStr += "NVARCHAR2(" + lenAttr.MaximumLength + ")";
                    // 是否必须
                    var Required = GetObjectFromAttribute<RequiredAttribute>(propInfo);
                    if (Required != null && !Required.AllowEmptyStrings)
                        returnStr += " NOT NULL ENABLE";
                    else
                        returnStr += "";
                    return returnStr;
                case "DateTime":
                    return "TIMESTAMP (6) WITH LOCAL TIME ZONE NOT NULL ENABLE";
                case "Decimal":
                    return "NUMBER(38,2) NOT NULL ENABLE";
                case "Boolean":
                    return "CHAR(1) NOT NULL ENABLE";
                case "Nullable`1":
                    // 可为空值类型
                    switch (type.GetGenericArguments()[0].Name)
                    {
                        case "Int32":
                            return "NUMBER";
                        case "DateTime":
                            return "TIMESTAMP (6) WITH LOCAL TIME ZONE";
                        case "Decimal":
                            return "NUMBER(38,2)";
                        case "Boolean":
                            return "CHAR(1)";
                    }
                    return "";
            }
            return string.Empty;
        }

        /// <summary>
        /// 从特性中获取该特性类型的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        static T GetObjectFromAttribute<T>(MemberInfo element) where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(element, typeof(T));
        }
    }


    /// <summary>
    /// 查询表结构模型
    /// </summary>
    public class ColumnModel
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string Table_Name { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string Column_Name { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public string Data_Type { get; set; }

        /// <summary>
        /// 字段可否为空
        /// </summary>
        public string Nullable { get; set; }

        /// <summary>
        /// 字符长度
        /// </summary>
        public int Char_Length { get; set; }

        /// <summary>
        /// 小数
        /// </summary>
        public int? Data_Scale { get; set; }
    }
}

