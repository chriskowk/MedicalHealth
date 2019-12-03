using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;


namespace LunarCalendar
{
    class ChineseLunarCalendar : ICalendar
    {
        private ChineseLunisolarCalendar chineseCalendar = new ChineseLunisolarCalendar();

        private string lowOrderDigitOfLunarDate = LunarCalendar.Properties.Resources.ChineseNumberText;
        private string highOrderDigitOfLunarDate = LunarCalendar.Properties.Resources.HighOrderText;

        private string[] LunarMonth = new string[]{ 
            LunarCalendar.Properties.Resources.LunarJan,
            LunarCalendar.Properties.Resources.LunarFeb, 
            LunarCalendar.Properties.Resources.LunarMar,
            LunarCalendar.Properties.Resources.LunarApr, 
            LunarCalendar.Properties.Resources.LunarMay,
            LunarCalendar.Properties.Resources.LunarJun, 
            LunarCalendar.Properties.Resources.LunarJul,
            LunarCalendar.Properties.Resources.LunarAug, 
            LunarCalendar.Properties.Resources.LunarSep,
            LunarCalendar.Properties.Resources.LunarOct, 
            LunarCalendar.Properties.Resources.LunarNov,
            LunarCalendar.Properties.Resources.LunarDec 
        };

        private Hashtable festivalDate = new Hashtable();

        public ChineseLunarCalendar()
        {
            festivalDate.Add(101, LunarCalendar.Properties.Resources.SpringFestivalText);
            festivalDate.Add(115, LunarCalendar.Properties.Resources.YuanxiaoText);
            festivalDate.Add(505, LunarCalendar.Properties.Resources.DuanwuText);
            festivalDate.Add(815, LunarCalendar.Properties.Resources.MidautumnText);
            festivalDate.Add(909, LunarCalendar.Properties.Resources.ChongyangText);
            festivalDate.Add(1230, LunarCalendar.Properties.Resources.ChuxiText);
        }

        public List<DateEntry> GetDaysOfMonth(int year, int month)
        {
            DateTime dateTime = new DateTime(year, month, 1);

            int highOrderIndex;
            int lowOrderIndex;
            String highOrderString;
            String lowOrderString;
            string lunarStrName;
            DateEntry tempEntry;
            List<DateEntry> dateList = new List<DateEntry>();

            //The index of Jan is 1.
            //The lunar year may span two Gregorian years, so we need to record the 
            //current lunar year for the later calculation the leap month of this lunar year.
            int lunarYear = chineseCalendar.GetYear(dateTime);
            int lunarMonth = chineseCalendar.GetMonth(dateTime);
            int lunarDay = chineseCalendar.GetDayOfMonth(dateTime);
            int leapMonth = chineseCalendar.GetLeapMonth(lunarYear);
            int lunarMonthIndex = lunarMonth;
            int lunarMonthDayNum = chineseCalendar.GetDaysInMonth(lunarYear, lunarMonth);
            int gregorianMonthDayNum = DateTime.DaysInMonth(year, month);

            if ((leapMonth != 0) && (lunarMonth > leapMonth))
            {
                lunarMonth--;
            }

            for (int i = 0; i < gregorianMonthDayNum; i++)
            {
                switch (lunarDay)
                {
                    case 1:
                        {
                            int index = lunarMonth - 1;
                            lunarStrName = LunarMonth[index];
                            if (lunarMonthIndex == leapMonth)
                            {
                                lunarStrName = LunarCalendar.Properties.Resources.LeapText + lunarStrName;
                            }
                            break;
                        }
                    case 10:
                        {
                            lunarStrName = LunarCalendar.Properties.Resources.ChushiText;
                            break;
                        }
                    case 20:
                        {
                            highOrderString = new String(lowOrderDigitOfLunarDate[1], 1);
                            lowOrderString = new String(lowOrderDigitOfLunarDate[9], 1);
                            lunarStrName = highOrderString.Normalize() + lowOrderString.Normalize();
                            break;
                        }
                    default:
                        {
                            lowOrderIndex = (lunarDay - 1) % 10;
                            highOrderIndex = lunarDay / 10;

                            highOrderString = new String(highOrderDigitOfLunarDate[highOrderIndex], 1);
                            lowOrderString = new String(lowOrderDigitOfLunarDate[lowOrderIndex], 1);
                            lunarStrName = highOrderString.Normalize() + lowOrderString.Normalize();
                            break;
                        }

                }

                if (lunarMonthIndex == leapMonth)
                {
                    tempEntry = new DateEntry(lunarDay, lunarStrName, false);
                }
                else
                {
                    int key = lunarMonth * 100 + lunarDay;
                    string festivalName = (string)festivalDate[key];
                    if (festivalName != null)
                    {
                        tempEntry = new DateEntry(lunarDay, festivalName, true);
                    }
                    else if (lunarDay == lunarMonthDayNum && lunarMonth == 12)
                    {
			key = 1230;
			festivalName = (string)festivalDate[key];
                        tempEntry = new DateEntry(lunarDay, festivalName, true);
                    }
                    else
                    {
                        tempEntry = new DateEntry(lunarDay, lunarStrName, false);
                    }
                }
                dateList.Add(tempEntry);

                //Update the lunar status
                dateTime = dateTime.AddDays(1);
                if (lunarDay == lunarMonthDayNum)
                {
                    lunarDay = 1;
                    lunarMonth = chineseCalendar.GetMonth(dateTime);
                    lunarMonthIndex = lunarMonth;
                    lunarYear = chineseCalendar.GetYear(dateTime);
                    leapMonth = chineseCalendar.GetLeapMonth(lunarYear);
                    lunarMonthDayNum = chineseCalendar.GetDaysInMonth(lunarYear, lunarMonth);
                    if ((leapMonth != 0) && (lunarMonth >= leapMonth))
                    {
                        lunarMonth--;
                    }
                }
                else
                {
                    lunarDay++;
                }
            }
            return dateList;
        }
    }
}
