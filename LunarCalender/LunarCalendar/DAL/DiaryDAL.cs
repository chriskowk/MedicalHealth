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

        public bool EraseOnceRemind(DateTime recordDate, DateTime runningStart)
        {
            string sql = $"UPDATE Diary SET IsRemindRequired = 0 WHERE IsRemindRequired = 1 AND DATE(RecordDate) = '{recordDate:yyyy-MM-dd}' AND STRFTIME('%Y-%m-%d %H:%M', RunningStart, 'localtime') = '{runningStart:yyyy-MM-dd HH:mm}'";
            if (DapperHelper.Execute<Diary>(sql) > 0)
            {
                GlobalParams.IsDiaryUpdated = true;
                return true;
            }

            return false;
        }

        public bool EraseExpiredOnceRemind(DateTime recordDate, DateTime runningStart, bool deleted = false)
        {
            string sql = deleted ? "DELETE FROM Diary" : "UPDATE Diary SET IsRemindRequired = 0";
            sql += $" WHERE IsRemindRequired = 1 AND (RecordDate < '{recordDate:yyyy-MM-dd}' OR (RecordDate = '{recordDate:yyyy-MM-dd}' AND RunningStart < '{runningStart:yyyy-MM-dd HH:mm}'))";

            if (DapperHelper.Execute<Diary>(sql) > 0)
            {
                GlobalParams.IsDiaryUpdated = true;
                return true;
            }

            return false;
        }
    }
}
