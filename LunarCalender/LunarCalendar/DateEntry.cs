using System;

namespace LunarCalendar
{
    class DateEntry
    {
        public int DateOfMonth;
        public string Text;
        public bool IsFestival;

        public DateEntry(int date, string text, bool isFestival)
        {
            DateOfMonth = date;
            Text = text;
            IsFestival = isFestival;
        }
    }
}
