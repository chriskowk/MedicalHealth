using LunarCalendar.Entities;
using LunarCalendar.SqlContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LunarCalendar.DAL
{
    public class DiaryDAL
    {
        public IList<Diary> GetDiaries()
        {
            return DapperExHelper<Diary>.GetAll().ToList();
        }
        public IList<Diary> GetDiaries(string sql)
        {
            return DapperExHelper<Diary>.Query(sql).ToList();
        }
        public Diary GetDiary(int id)
        {
            return DapperExHelper<Diary>.Get(id);
        }
    }
}
