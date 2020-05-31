using Dapper.Contrib.Extensions;
using LunarCalendar.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LunarCalendar.SqlContext
{
    public class DapperExHelper<T> where T : BaseEntity
    {
        public T Get(int id)
        {
            return ConnectionOption.DbConnection.Get<T>(id);
        }
        public IEnumerable<T> GetAll()
        {
            return ConnectionOption.DbConnection.GetAll<T>();
        }
        public long Insert(T t)
        {
            return ConnectionOption.DbConnection.Insert(t);
        }
        public bool Update(T t)
        {
            return ConnectionOption.DbConnection.Update(t);
        }
        public bool Delete(T t)
        {
            return ConnectionOption.DbConnection.Delete(t);
        }
        public bool DeleteAll()
        {
            return ConnectionOption.DbConnection.DeleteAll<T>();
        }
    }
}
