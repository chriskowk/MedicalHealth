using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunarCalendar
{
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
