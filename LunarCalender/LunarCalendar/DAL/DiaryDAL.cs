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
        DapperExHelper<Diary> dapperExHelper = new DapperExHelper<Diary>();
        public IList<Diary> GetDiaries()
        {
            return dapperExHelper.GetAll().ToList();
        }
        public Diary GetDiary(int id)
        {
            return dapperExHelper.Get(id);
        }
        public long Insert(Diary dto)
        {
            return dapperExHelper.Insert(dto);
        }
    }
}
