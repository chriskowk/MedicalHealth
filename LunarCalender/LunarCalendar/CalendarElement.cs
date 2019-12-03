using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LunarCalendar
{
    public struct CalendarElement
    {
        private string solarDay;
        private string lunarDesc;
        private byte elementType;
        private int weekday;
        private byte remindFlag;
        private DateTime remindTime;
        private string remindText;

        public string SolarDay
        {
            get { return solarDay; }
            //set { solarDay = value; }
        }

        public string LunarDesc
        {
            get { return lunarDesc; }
            //set { lunarDesc = value; }
        }

        public byte ElementType
        {
            get { return elementType; }
            //set { elementType = value; }
        }

        public int Weekday
        {
            get { return weekday; }
            //set { weekday = value; }
        }

        public byte RemindFlag
        {
            get { return remindFlag; }
            //set { remindFlag = value; }
        }

        public DateTime RemindTime
        {
            get { return remindTime; }
            //set { remindTime = value; }
        }

        public string RemindText
        {
            get { return remindText; }
            //set { remindText = value; }
        }

        public override string ToString()
        {
            return SolarDay + "\n" + LunarDesc;
        }

        public CalendarElement(string solarDay, string lunarDesc, byte elementType, int weekday, byte remindFlag, DateTime remindTime, string remindText)
        {
            this.solarDay = solarDay;
            this.lunarDesc = lunarDesc;
            this.elementType = elementType;
            this.weekday = weekday;
            this.remindFlag = remindFlag;
            this.remindTime = remindTime;
            this.remindText = remindText;
        }
    }

}
