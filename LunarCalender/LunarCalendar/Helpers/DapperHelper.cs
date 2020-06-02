using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace LunarCalendar.SqlContext
{
    public static class DapperHelper
    {
        private static IDbConnection Connection => ConnectionOption.DbConnection;


        /// <summary>
        /// 单个查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="commandType">command类型</param>
        /// <returns></returns>
        public static T QueryFirst<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            Connection.Open();
            using (IDbTransaction transaction = Connection.BeginTransaction())
            {
                var user = Connection.QueryFirstOrDefault<T>(sql, param, transaction, commandTimeout, commandType);
                transaction.Commit();
                Connection.Close();
                return user;
            }
        }
        /// <summary>
        /// 多个查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="transaction">事务</param>
        /// <param name="buffered">缓冲/缓存</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="commandType">command类型</param>
        /// <returns></returns>
        public static IEnumerable<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static int Execute<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.Execute(sql, param, transaction, commandTimeout, commandType);
        }

        /// <summary>
        /// 获取Model-Key为int类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(int id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            return await Connection.GetAsync<T>(id, transaction, commandTimeout);
        }

        /// <summary>
        /// 获取Model集合（没有Where条件）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> GetAllAsync<T>() where T : class, new()
        {
            return await Connection.GetAllAsync<T>();
        }

        /// <summary>
        /// 插入一个Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="sqlAdapter"></param>
        /// <returns></returns>
        public static async Task<int> InsertAsync<T>(T model, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            return await Connection.InsertAsync<T>(model, transaction, commandTimeout);
        }

        /// <summary>
        /// 更新一个Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entityToUpdate"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static async Task<T> UpdateAsync<T>(T model, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            bool ret = await Connection.UpdateAsync<T>(model, transaction, commandTimeout);
            return ret ? model : null;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <param name="p">动态参数</param>
        /// <param name="sqlTotal">total语句</param>
        /// <param name="p2">Total动态参数</param>
        /// <returns></returns>
        public static async Task<string> PageLoadAsync<T>(string sql, object p = null, string sqlTotal = "", object p2 = null)
        {
            var rows = await Connection.QueryAsync<T>(sql.ToString(), p);
            var total = rows.Count();
            if (!string.IsNullOrWhiteSpace(sqlTotal)) { total = await Connection.ExecuteScalarAsync<int>(sqlTotal, p2); }

            return new { rows, total }.ToJson();
        }
    }
}
