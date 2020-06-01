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

        public bool EraseOnceRemind(DateTime recordOn, DateTime remindOn)
        {
            string sql = $"UPDATE Diary SET RemindFlag = 0 WHERE RemindFlag = 1 AND DATE(RecordOn) = '{recordOn:yyyy-MM-dd}' AND STRFTIME('%Y-%m-%d %H:%M', RemindOn, 'localtime') = '{remindOn:yyyy-MM-dd HH:mm}'";
            if (DapperHelper.Execute<Diary>(sql) > 0)
            {
                GlobalParams.IsDiaryUpdated = true;
                return true;
            }

            return false;
        }

        public bool EraseExpiredOnceRemind(DateTime recordOn, DateTime remindOn, bool deleted = false)
        {
            string sql = deleted ? "DELETE FROM Diary" : "UPDATE Diary SET RemindFlag = 0";
            sql += $" WHERE RemindFlag = 1 AND (RecordOn < '{recordOn:yyyy-MM-dd}' OR (RecordOn = '{recordOn:yyyy-MM-dd}' AND RemindOn < '{remindOn:yyyy-MM-dd HH:mm}'))";

            if (DapperHelper.Execute<Diary>(sql) > 0)
            {
                GlobalParams.IsDiaryUpdated = true;
                return true;
            }

            return false;
        }
    }
}
