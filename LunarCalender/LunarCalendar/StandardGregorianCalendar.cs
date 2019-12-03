using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace LunarCalendar
{
    class StandardGregorianCalendar : ICalendar
    {
        private Hashtable festivalDate = new Hashtable();

        public StandardGregorianCalendar()
        {
            festivalDate.Add(101, LunarCalendar.Properties.Resources.NewYearsDayText);
            festivalDate.Add(501, LunarCalendar.Properties.Resources.LaborDayText);
            festivalDate.Add(504, LunarCalendar.Properties.Resources.YouthDayText);
            festivalDate.Add(601, LunarCalendar.Properties.Resources.ChildrensDayText);
            festivalDate.Add(801, LunarCalendar.Properties.Resources.ArmyDayText);
            festivalDate.Add(1001, LunarCalendar.Properties.Resources.NationalDayText);
            festivalDate.Add(1225, LunarCalendar.Properties.Resources.ChristmasText);
        }

        //Now the 3rd parameter of constructor of DateEntry is set to 0, that is no handle of it at the moment.
        public List<DateEntry> GetDaysOfMonth(int year, int month)
        {
            int DayNum = DateTime.DaysInMonth(year, month);
            DateEntry tempEntry;
            List<DateEntry> dateList = new List<DateEntry>();

            for (int i = 0; i < (DayNum); i++)
            {
                int date = i + 1;
                string dateString = date.ToString(NumberFormatInfo.CurrentInfo);
                int key = month * 100 + date;
                string festivalName = (string)festivalDate[key];
                bool isFestival = false;
                if (festivalName != null)
                {
                    dateString = festivalName;
                    isFestival = true;
                }

                tempEntry = new DateEntry(date, dateString, isFestival);
                dateList.Add(tempEntry);
            }

            return dateList;
        }
    }
}
