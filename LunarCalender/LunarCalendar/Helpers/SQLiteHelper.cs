using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunarCalendar
{
    public class SQLiteHelper
    {
        public static string ConnectionString = @"Data Source=F:\GitHub\MedicalHealth\LunarCalender\LunarCalendar\bin\Debug\calendar.db; Initial Catalog=sqlite; Integrated Security=True; Max Pool Size=10";

        /// <summary> 
        /// 执行SQL语句，返回影响的记录数 
        /// </summary> 
        /// <param name="SQLString">SQL语句</param> 
        /// <returns>影响的记录数</returns> 
        public static int ExecuteSql(string SQLString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                //事务 
                using (SQLiteTransaction tx = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(SQLString, connection))
                    {
                        try
                        {
                            connection.Open();
                            cmd.Transaction = tx;
                            int rows = cmd.ExecuteNonQuery();
                            tx.Commit();
                            return rows;
                        }
                        catch (System.Data.SqlClient.SqlException e)
                        {
                            connection.Close();
                            tx.Rollback();
                            throw e;
                        }
                    }
                }
            }
        }

        /// <summary> 
        /// 执行查询语句，返回DataTable 
        /// </summary> 
        /// <param name = "SQLString" > 查询语句 </ param > 
        /// < returns > DataSet </ returns > 
        public static DataTable Query(string SQLString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SQLiteDataAdapter command = new SQLiteDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                    return ds.Tables[0];
                }
                catch (Exception ex)
                {
                    connection.Close();
                    throw ex;
                }
            }
        }

        /// <summary> 
        /// 执行存储过程，返回影响的记录数 
        /// </summary> 
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns> 
        public static int ExecuteProc(string procName, SQLiteParameter[] paras)
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                //事务 
                using (SQLiteTransaction tx = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(connection))
                    {
                        try
                        {
                            connection.Open();
                            for (int i = 0; i < paras.Length; i++)
                            {
                                cmd.Parameters.Add(paras[i]);
                            }
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = procName;
                            cmd.Transaction = tx;
                            int rows = cmd.ExecuteNonQuery();
                            tx.Commit();
                            return rows;
                        }
                        catch (System.Data.SqlClient.SqlException e)
                        {
                            connection.Close();
                            tx.Rollback();
                            throw e;
                        }
                    }
                }
            }
        }
         
        /// <summary> 
        /// 执行带参数的SQL语句，返回影响的记录数
        /// </summary> 
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns> 
        public static int ExecuteSqlWithParas(string SQLString, SQLiteParameter[] paras)
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                //事务
                using (SQLiteTransaction tx = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(connection))
                    {
                        try
                        {
                            connection.Open();
                            for (int i = 0; i < paras.Length; i++)
                            {
                                cmd.Parameters.Add(paras[i]);
                            }
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = SQLString;
                            cmd.Transaction = tx;
                            int rows = cmd.ExecuteNonQuery();
                            tx.Commit();
                            return rows;
                        }
                        catch (System.Data.SqlClient.SqlException e)
                        {
                            connection.Close();
                            tx.Rollback();
                            throw e;
                        }
                    }
                }
            }
        }
    }
}
