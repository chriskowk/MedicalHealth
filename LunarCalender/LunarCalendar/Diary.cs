using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Data.SQLite;

namespace LunarCalendar
{

    [Table(Name = "Diary")]
    public class Diary
    {

        [Column(IsPrimaryKey = true)]
        public int ID { get; set; }

        [Column]
        public string Title { get; set; }

        [Column]
        public string Keywords { get; set; }

        [Column]
        public string Content { get; set; }

        [Column]
        public DateTime RecordOn { get; set; }

        [Column]
        public int RemindFlag { get; set; }

        [Column]
        public DateTime RemindOn { get; set; }
    }

    public abstract class DiaryBase
    {
        public abstract int ID { get; }
        public abstract DateTime RecordOn { get; }
        public abstract string Content { get; }
        public abstract string Title { get; }
        public abstract string Keywords { get; }
        public abstract int RemindFlag { get; }
        public abstract DateTime RemindOn { get; }
        public abstract int RecordCount { get; }
        public abstract DateTime RecordDateFrom { get; }
        public abstract DateTime RecordDateTo { get; }
        public abstract IList<Diary> Diaries { get; }
        public abstract bool GetLists(Expression<Func<Diary, bool>> expr);
        public abstract bool MoveFirst();
        public abstract bool MoveLast();
        public abstract bool MoveNext();
        public abstract bool Save(Diary diary);
        public abstract bool Save(DateTime RecordOn, string Title, string Keywords, string Content, int RemindFlag, DateTime RemindOn);
        public abstract bool Delete(DateTime RecordOn);
        public abstract bool Delete(int ID);
        public abstract bool EraseOnceRemind(DateTime RecordOn, DateTime RemindOn);
        public abstract bool EraseExpiredOnceRemind(DateTime RecordOn, DateTime RemindOn, bool Deleted);

        public override string ToString()
        {
            return Content;
        }
    }


    public class DiaryMDB : DiaryBase
    {
        private Connection _conn;
        private DataContext _ctx;
        private Diary[] _diaries = new Diary[] { };
        private int _recordCount = 0;
        private int _rowIndex = 0;
        private DateTime _recordDateFrom = DateTime.Parse("1900-1-1");
        private DateTime _recordDateTo = DateTime.Parse("1900-1-1");

        public DiaryMDB()
        {

            _conn = Connection.getInstance();
            if (_conn == null) return;

            _ctx = _conn.ctx;
        }

        public override IList<Diary> Diaries
        {
            get { return _diaries.ToList(); }
        }

        public override bool GetLists(Expression<Func<Diary, bool>> expr)
        {
            _diaries = _ctx.GetTable<Diary>().Where(expr.Compile()).OrderBy(a => a.RecordOn).ToArray();
            //var dt = SQLiteHelper.Query("SELECT * FROM Diary");
            FillProperties();

            return true;
        }

        private void FillProperties()
        {
            _recordCount = _diaries.Length;
            _rowIndex = 0;
            if (_recordCount > 0)
            {
                _recordDateTo = _diaries.Max(a => a.RecordOn);
                _recordDateFrom = _diaries.Min(a => a.RecordOn);
            }
        }

        public override bool MoveFirst()
        {
            if (_recordCount > 0)
            {
                _rowIndex = 0;
                return true;
            }
            else
                return false;
        }

        public override bool MoveLast()
        {
            if (_recordCount > 0)
            {
                _rowIndex = _recordCount - 1;
                return true;
            }
            else
                return false;
        }

        public override bool MoveNext()
        {
            if (_rowIndex >= 0 && _rowIndex < _recordCount - 1)
            {
                _rowIndex++;
                return true;
            }
            else
                return false;
        }

        public override int RecordCount
        {
            get { return _recordCount; }
        }

        public override int ID
        {
            get
            {
                if (_recordCount > 0 && _rowIndex >= 0)
                    return _diaries[_rowIndex].ID;
                else
                    return -1;
            }
        }

        public override string Content
        {
            get
            {
                if (_recordCount > 0 && _rowIndex >= 0)
                    return _diaries[_rowIndex].Content;
                else
                    return "";
            }
        }

        public override DateTime RecordOn
        {
            get
            {
                if (_recordCount > 0 && _rowIndex >= 0)
                    return _diaries[_rowIndex].RecordOn;
                else
                    return DateTime.Parse("1900-1-1");
            }
        }

        public override string Title
        {
            get
            {
                if (_recordCount > 0 && _rowIndex >= 0)
                    return _diaries[_rowIndex].Title;
                else
                    return "";
            }
        }

        public override string Keywords
        {
            get
            {
                if (_recordCount > 0 && _rowIndex >= 0)
                    return _diaries[_rowIndex].Keywords;
                else
                    return "";
            }
        }

        public override int RemindFlag
        {
            get
            {
                if (_recordCount > 0 && _rowIndex >= 0)
                    return _diaries[_rowIndex].RemindFlag;
                else
                    return 0;
            }
        }

        public override DateTime RemindOn
        {
            get
            {
                if (_recordCount > 0 && _rowIndex >= 0)
                    return _diaries[_rowIndex].RemindOn;
                else
                    return DateTime.Parse("00:00");
            }
        }

        public override DateTime RecordDateFrom
        {
            get { return _recordDateFrom; }
        }

        public override DateTime RecordDateTo
        {
            get { return _recordDateTo; }
        }

        public override bool Save(Diary diary)
        {
            return Save(diary.RecordOn, diary.Title, diary.Keywords, diary.Content, diary.RemindFlag, diary.RemindOn);
        }

        public override bool Save(DateTime RecordOn, string Title, string Keywords, string Content, int RemindFlag, DateTime RemindOn)
        {
            string sql;

            //IList<Diary> dtos = (from a in _ctx.GetTable<Diary>()
            //                     where a.RecordOn == RecordOn.Date
            //                     select a).ToList();
            IList<Diary> dtos = _ctx.ExecuteQuery<Diary>("SELECT * FROM Diary WHERE RecordOn = {0}", RecordOn.Date).ToList();
            if (dtos.Count > 0)
            {
                sql = "UPDATE Diary SET [Title] = {0}, [Keywords] = {1}, [Content] = {2}, [RemindFlag] = {3}, [RemindOn] = {4}"
                    + "  WHERE RecordOn = {5}";
                _ctx.ExecuteCommand(sql, Title, Keywords, Content, RemindFlag, GlobalParams.OriginalDate.AddHours(RemindOn.Hour).AddMinutes(RemindOn.Minute), RecordOn.Date);
            }
            else
            {
                sql = "INSERT INTO Diary([RecordOn], [Title], [Keywords], [Content], [RemindFlag], [RemindOn] ) VALUES( {0}, {1}, {2}, {3}, {4}, {5})";
                _ctx.ExecuteCommand(sql, RecordOn.Date, Title, Keywords, Content, RemindFlag, GlobalParams.OriginalDate.AddHours(RemindOn.Hour).AddMinutes(RemindOn.Minute));
            }

            GlobalParams.IsDiaryUpdated = true;
            return true;
        }

        public override bool Delete(DateTime RecordOn)
        {
            string sql;

            IList<Diary> dtos = _ctx.ExecuteQuery<Diary>("SELECT * FROM Diary WHERE RecordOn = {0}", RecordOn.Date).ToList();
            if (dtos.Count > 0)
            {
                sql = "DELETE Diary WHERE RecordOn = {0}";
                _ctx.ExecuteCommand(sql, RecordOn.Date);
            }
            GlobalParams.IsDiaryUpdated = true;
            return true;
        }

        public override bool Delete(int ID)
        {
            string sql;

            IList<Diary> dtos = _ctx.ExecuteQuery<Diary>("SELECT * FROM Diary WHERE ID = {0}", ID).ToList();
            if (dtos.Count > 0)
            {
                sql = "DELETE Diary WHERE ID = {0}";
                _ctx.ExecuteCommand(sql, ID);
            }
            GlobalParams.IsDiaryUpdated = true;
            return true;
        }

        public override bool EraseOnceRemind(DateTime RecordOn, DateTime RemindOn)
        {
            string sql;

            sql = "UPDATE Diary SET RemindFlag = 0 WHERE RemindFlag = 1 AND RecordOn = {0} AND RemindOn = {1}";
            _ctx.ExecuteCommand(sql, RecordOn.Date, GlobalParams.OriginalDate.AddHours(RemindOn.Hour).AddMinutes(RemindOn.Minute));
            GlobalParams.IsDiaryUpdated = true;
            return true;
        }

        public override bool EraseExpiredOnceRemind(DateTime RecordOn, DateTime RemindOn, bool Deleted)
        {
            string sql;

            if (Deleted)
                sql = "DELETE FROM Diary";
            else
                sql = "UPDATE Diary SET RemindFlag = 0";

            sql = sql + " WHERE RemindFlag = 1 AND (RecordOn < {0} OR (RecordOn = {0} AND RemindOn < {1}))";

            _ctx.ExecuteCommand(sql, RecordOn.Date, GlobalParams.OriginalDate.AddHours(RemindOn.Hour).AddMinutes(RemindOn.Minute));
            GlobalParams.IsDiaryUpdated = true;
            return true;
        }

        private string FormatTextSQL(string sql)
        {
            return FormatTextSQL(sql, true);
        }

        private string FormatTextSQL(string sql, bool Bracketed)
        {
            string strBracket;

            strBracket = Bracketed ? "'" : string.Empty;
            return strBracket + sql.Replace("'", "''") + strBracket;
        }
    }

    public class Connection
    {
        private static string connectstring;
        private static DataContext _ctx = null;
        private static SQLiteConnection _cn = null;
        private static Connection _instance = null;

        //不允许外部调用NEW创建实例
        private Connection() { }

        public SQLiteConnection cn
        {
            get
            {
                if (_instance == null) getInstance();
                return _cn;
            }
        }

        public DataContext ctx
        {
            get
            {
                if (_instance == null) getInstance();
                return _ctx;
            }
        }

        public static Connection getInstance()
        {
            if (_instance == null) _instance = new Connection();

            if (_cn == null)
            {
#if USINGPROJECTSYSTEM
                string strDBFileName = "..\\..\\CalendarDatas.mdb";
#else
                string strDBFileName = @"CalendarDatas.mdb";
#endif

                // Checks to see if the options.txt file exists.
                if (!File.Exists(strDBFileName))
                {
                    Console.WriteLine("Error: {0} does not exist.", strDBFileName);
                    GlobalParams.IsDatabaseOnline = false;
                    return null;
                }

                //connectstring = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strDBFileName + ";User ID=Admin;Jet OLEDB:Database Password=721123;Mode=ReadWrite|Share Deny None;Persist Security Info=False";
                connectstring = SQLiteHelper.ConnectionString;
                try
                {
                    _cn = new SQLiteConnection(connectstring);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: Failed to create a database connection. \n{0}", ex.Message);
                    GlobalParams.IsDatabaseOnline = false;
                    return null;
                }

                _ctx = new DataContext(_cn);
            }

            GlobalParams.IsDatabaseOnline = true;
            return _instance;
        }
    }

    public static class GlobalParams
    {
        public const int FIRSTYEAR = 1902;
        public const int LASTYEAR = 2100;
        public static DateTime OriginalDate = new DateTime(1899, 12, 30);
        public static bool IsFormLoaded;
        public static bool IsDatabaseOnline;
        public static bool IsDiaryUpdated;
        public static bool IsBeepOnClock;
        public static bool IsShowBackPicture;
    }
}
