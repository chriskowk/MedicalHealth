using Dapper.Contrib.Extensions;
using LunarCalendar.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LunarCalendar.SqlContext
{
    public static class DapperExHelper<T> where T : BaseEntity
    {
        public static T Get(int id)
        {
            return ConnectionOption.DbConnection.Get<T>(id);
        }
        public static IEnumerable<T> GetAll()
        {
            return ConnectionOption.DbConnection.GetAll<T>();
        }
        public static IEnumerable<T> Query(string sql)
        {
            return DapperHelper.Query<T>(sql);
        }
        public static long Insert(T t)
        {
            return ConnectionOption.DbConnection.Insert(t);
        }
        public static bool Update(T t)
        {
            return ConnectionOption.DbConnection.Update(t);
        }
        public static bool Delete(T t)
        {
            return ConnectionOption.DbConnection.Delete(t);
        }
        public static bool DeleteAll()
        {
            return ConnectionOption.DbConnection.DeleteAll<T>();
        }
    }
}
