using System;
using System.Collections.Generic;

namespace LunarCalendar
{
    interface ICalendar
    {
        List<DateEntry> GetDaysOfMonth(int year, int month);
    }
}
