using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace LunarCalendar
{
    /// <summary>
    /// 表示标准的时间日期数据格式
    /// </summary>
    public class DateTimeFormatter
    {
        #region ctor 及字段
        /// <summary>
        /// 年
        /// </summary>
        public static readonly DateTimeFormatter LongYear;
        /// <summary>
        /// 年-月
        /// </summary>
        public static readonly DateTimeFormatter LongYearMonth;

        /// <summary>
        /// 短日期格式。yy/MM/dd
        /// </summary>
        public static readonly DateTimeFormatter ShortDate;
        /// <summary>
        /// 长日期格式。yyyy/MM/dd
        /// </summary>
        public static readonly DateTimeFormatter LongDate;
        /// <summary>
        /// 短时间格式。HH:mm
        /// </summary>
        public static readonly DateTimeFormatter ShortTime;
        /// <summary>
        /// 长时间格式。HH:mm:ss
        /// </summary>
        public static readonly DateTimeFormatter LongTime;
        /// <summary>
        /// 长日期短时间格式。yyyy/MM/dd HH:mm
        /// </summary>
        public static readonly DateTimeFormatter LongDateShortTime;
        /// <summary>
        /// 短日期长时间格式。yy/MM/dd HH:mm:ss
        /// </summary>
        public static readonly DateTimeFormatter ShortDateLongTime;
        /// <summary>
        /// 长日期长时间格式。yyyy/MM/dd HH:mm:ss
        /// </summary>
        public static readonly DateTimeFormatter LongDateTime;
        /// <summary>
        /// 短日期短时间格式。yy/MM/dd HH:mm
        /// </summary>
        public static readonly DateTimeFormatter ShortDateTime;

        /// <summary>
        /// 长日期长时间格式。yyyy/MM/dd HH
        /// </summary>
        public static readonly DateTimeFormatter LongDateShortHourTime;
        /// <summary>
        /// 短日期短时间格式。yy/MM/dd HH
        /// </summary>
        public static readonly DateTimeFormatter ShortDateShortHourTime;

        /// <summary>
        /// 带毫秒的长时间格式。yyyy/MM/dd HH:mm:ss.fff
        /// </summary>
        public static readonly DateTimeFormatter LongDateTimeMillisecond;

        static string dsp
        {
            get { return CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator; }
        }

        /// <summary>
        /// 日期时间分隔符
        /// </summary>
        static char[] _dateTimeSeparators = new char[] { ' ' };
        /// <summary>
        /// 日期部分分隔符
        /// </summary>
        static char[] _dateSeparators = new char[] { '-', '\\', '/', '年' };
        /// <summary>
        /// 时间部分分隔符
        /// </summary>
        static char[] _timeSeparators = new char[] { '.', ',', ';', ':' };
        /// <summary>
        /// 毫秒部分分隔符
        /// </summary>
        static char[] _millisecondSeparators = new char[] { '.' };
        static IList<DateTimeFormatter> _list;
        static DateTimeFormatter()
        {
            _list = new List<DateTimeFormatter>();

            LongYear = new DateTimeFormatter(DateTimePrecision.Year, string.Format("yyyy{0}", "年"), "0000", "^[0-9][0-9][0-9][0-9][-,年,/]?$");
            LongYearMonth = new DateTimeFormatter(DateTimePrecision.Month, string.Format("yyyy{0}MM", dsp), "0000/00/00", "^[1-9][0-9][0-9][0-9][-,年,/][0-9][0-9][-,月,/]?$");
            ShortDate = new DateTimeFormatter(DateTimePrecision.Day, string.Format("yy{0}MM{0}dd", dsp), "00/90/90", "^[0-9][0-9][-,年,/][0-9][0-9][-,月,/][0-9][0-9][日]?$");
            LongDate = new DateTimeFormatter(DateTimePrecision.Day, string.Format("yyyy{0}MM{0}dd", dsp), "0000/00/00", "^[1-9][0-9][0-9][0-9][-,年,/][0-9][0-9][-,月,/][0-9][0-9][日]?$");
            ShortTime = new DateTimeFormatter(DateTimePrecision.Minute, "HH:mm", "00:00", "^[0-9][0-9][:,时][0-9][0-9]$");
            LongTime = new DateTimeFormatter(DateTimePrecision.Second, "HH:mm:ss", "00:00:00", "^[0-9][0-9][:,时][0-9][0-9]:[0-9][0-9]$");
            LongDateShortTime = new DateTimeFormatter(DateTimePrecision.Minute, string.Format("yyyy{0}MM{0}dd HH:mm", dsp), "0000/00/00 00:00", "^[1-9][0-9][0-9][0-9][-,年,/][0-9][0-9][-,月,/][0-9][0-9][日]? [0-9][0-9][:,时][0-9][0-9][分]?$");
            ShortDateLongTime = new DateTimeFormatter(DateTimePrecision.Second, string.Format("yy{0}MM{0}dd HH:mm:ss", dsp), "00/00/00 00:00:00", "^[0-9][0-9][-,年,/][0-9][0-9][-,月,/][0-9][0-9][日]? [0-9][0-9]:[0-9][0-9][:,时][0-9][0-9][分]?$");
            LongDateTime = new DateTimeFormatter(DateTimePrecision.Second, string.Format("yyyy{0}MM{0}dd HH:mm:ss", dsp), "0000/00/00 00:00:00", "^[1-9][0-9][0-9][0-9][-,年,/][0-9][0-9][-,月,/][0-9][0-9][日]? [0-9][0-9][:,时][0-9][0-9][:,分][0-9][0-9][秒]?$");
            ShortDateTime = new DateTimeFormatter(DateTimePrecision.Minute, string.Format("yy{0}MM{0}dd HH:mm", dsp), "00/00/00 00:00", "^[0-9][0-9][-,年,/][0-9][0-9][-,月,/][0-9][0-9][日]? [0-9][0-9][:,时][0-9][0-9][分]?$");
            LongDateShortHourTime = new DateTimeFormatter(DateTimePrecision.Hour, string.Format("yyyy{0}MM{0}dd HH时", dsp), "0000/00/00 00", "^[1-9][0-9][0-9][0-9][-,年,/][0-9][0-9][-,月,/][0-9][0-9][日]? [0-9][0-9]$");
            ShortDateShortHourTime = new DateTimeFormatter(DateTimePrecision.Hour, string.Format("yy{0}MM{0}dd HH时", dsp), "00/00/00 00", "^[0-9][0-9][-,年,/][0-9][0-9][-,月,/][0-9][0-9][日]? [0-9][0-9]$");
            LongDateTimeMillisecond = new DateTimeFormatter(DateTimePrecision.Second, string.Format("yyyy{0}MM{0}dd HH:mm:ss.fff", dsp), "0000/00/00 00:00:00.000", "^[1-9][0-9][0-9][0-9][-,年,/][0-9][0-9][-,月,/][0-9][0-9][日]? [0-9][0-9][:,时][0-9][0-9][:,分][0-9][0-9][.,秒][0-9][0-9][毫秒]?$");
        }

        private string _stringFormat;
        private string _editMask;
        private List<Regex> _regexes;
        private DateTimeFormatter(DateTimePrecision precision, string format, string mask, params string[] regexes)
        {
            this.Precision = precision;
            _stringFormat = format;
            _editMask = mask;
            _regexes = new List<Regex>();

            foreach (var item in regexes)
            {
                _regexes.Add(new Regex(item));
            }

            _list.Add(this);
        }
        #endregion

        #region 实例成员
        /// <summary>
        /// 获取字符格式。
        /// </summary>
        public string StringFormat
        {
            get { return _stringFormat; }
        }
        /// <summary>
        /// 获取编辑掩码。
        /// </summary>
        public string EditMask
        {
            get { return _editMask; }
        }
        /// <summary>
        /// 获取时间精度。
        /// </summary>
        public DateTimePrecision Precision { get; private set; }

        /// <summary>
        /// 返回是否
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsMatch(string value)
        {
            if (this.StringFormat.Equals(value, StringComparison.CurrentCultureIgnoreCase))
                return true;

            foreach (var item in _regexes)
            {
                if (item.IsMatch(value))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static DateTimePrecision ParsePrecision(string format)
        {
            if (string.IsNullOrWhiteSpace(format)) format = "";
            DateTimeFormatter formatter = GetFormat(DateTime.Now.ToString(format));

            return formatter == null ? DateTimePrecision.None : formatter.Precision;
        }

        /// <summary>
        /// 将一个时间日期值格式化为一个字符串。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Format(DateTime value)
        {
            string s = value.ToString(_stringFormat);
            if (s.IndexOf(':') == -1) return s;

            if (s.Length > _editMask.Length && s.EndsWith(_editMask))
                return s.Left(s.Length - _editMask.Length).Trim();
            else
                return s;
        }

        /// <summary>
        /// 将一字符串解析为一个时间日期。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DateTime Parse(string value)
        {
            return Parse(this, value);
        }

        /// <summary>
        /// 将一字符串在一个边界范围内解析为一个时间日期。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public DateTime Parse(string value, DateTime min, DateTime max)
        {
            DateTime dt = Parse(value);
            if (dt < min) return min;
            if (dt > max) return max;
            return dt;
        }
        #endregion

        #region 静态成员
        /// <summary>
        /// 将一个字符串解析为一个时间日期。字符串为空时返回当前日期。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>        
        public static DateTime ParseDateTime(string value)
        {
            return ParseDateTime(value, true);
        }

        /// <summary>
        /// 将一个字符串解析为一个时间日期。字符串为空时返回当前日期。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isNullReturnNow"></param>
        /// <returns></returns>
        public static DateTime ParseDateTime(string value, bool isNullReturnNow)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (isNullReturnNow)
                    return DateTime.Now;
                else
                    return DateTime.MinValue;
            }

            DateTimeFormatter format;
            if (value.IndexOfAny(_dateSeparators) > -1 && value.IndexOfAny(_dateTimeSeparators) > -1 && value.IndexOfAny(_millisecondSeparators) > -1)
                format = LongDateTimeMillisecond;
            else if (value.IndexOfAny(_dateSeparators) > -1 && value.IndexOfAny(_dateTimeSeparators) > -1)
                format = LongDateTime;
            else if (value.IndexOfAny(_dateTimeSeparators) > -1)
                format = LongTime;
            else if (value.Trim().Length <= 5 && value.IndexOfAny(_timeSeparators) > -1)
                format = ShortTime;
            else
                format = LongDate;

            return Parse(format, value);
        }

        /// <summary>
        /// 将一个字符串解析为一个时间日期。字符串为空时返回null。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatString"></param>
        /// <returns></returns>
        public static DateTime? ParseDateTime(string value, string formatString)
        {
            if (string.IsNullOrEmpty(value)) return null;

            value = GetDateTimeString(value, formatString);
            DateTimeFormatter format = GetFormat(formatString) ?? GetFormat(value);
            if (format == null)
            {
                if (value.Contains("时") || value.Contains("分") || value.Contains(":"))
                    format = LongDateTime;
                else
                    format = LongDate;
            }

            return DoParse(format, value);
        }

        private static DateTimeFormatter GetFormat(string sample)
        {
            if (string.IsNullOrEmpty(sample)) return null;

            sample = sample.Trim();

            return _list.FirstOrDefault(a => a.IsMatch(sample));
        }

        /// <summary>
        /// 返回字符串解析可包含的格式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTimeFormatter GetFormatter(string value)
        {
            if (string.IsNullOrEmpty(value)) return LongDate;

            value = GetDateTimeString(value);
            DateTimeFormatter format;
            if (value.IndexOfAny(_dateSeparators) > -1 && value.IndexOfAny(_dateTimeSeparators) > -1)
            {
                if (value.EndsWith(":00") || value.Count(':') == 1)
                    format = LongDateShortTime;
                else
                    format = LongDateTime;
            }
            else if (value.IndexOfAny(_dateTimeSeparators) > -1)
            {
                format = LongTime;
            }
            else
            {
                format = LongDate;
            }

            return format;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static DateTimeFormatter GetFormatter(DateTimePrecision precision)
        {
            switch (precision)
            {
                case DateTimePrecision.Year:
                    return DateTimeFormatter.LongYear;
                case DateTimePrecision.Month:
                    return DateTimeFormatter.LongYearMonth;
                case DateTimePrecision.Day:
                    return DateTimeFormatter.LongDate;
                case DateTimePrecision.Hour:
                    return DateTimeFormatter.LongDateShortHourTime;
                case DateTimePrecision.Minute:
                    return DateTimeFormatter.LongDateShortTime;
                case DateTimePrecision.Second:
                    return DateTimeFormatter.LongDateTime;
                default:
                    return DateTimeFormatter.LongDate;
            }
        }

        private static DateTime Parse(DateTimeFormatter format, string value)
        {
            value = GetDateTimeString(value, format.StringFormat);

            DateTime dt = DoParse(format, value);

            return dt;
        }

        private static DateTime DoParse(DateTimeFormatter format, string value)
        {
            DateTime dtNow = DateTime.Now;
            if (string.IsNullOrEmpty(value)) return dtNow;

            bool isOnlyTime = format == DateTimeFormatter.ShortTime
                            || format == DateTimeFormatter.LongTime;

            bool isOnlyDate = format == DateTimeFormatter.ShortDate
                            || format == DateTimeFormatter.LongDate;

            bool isOnlyYear = format == DateTimeFormatter.LongYear;

            if (isOnlyYear)
            {
                int year = 0;
                if (!int.TryParse(value, out year))
                    return dtNow;

                year = int.Parse(value);
                return new DateTime(year, 1, 1);
            }
            DateTime dt;

            if (TryParseKeyWord(dtNow, value, out dt)) return dt;

            if (TryParseDouble(dtNow, value, isOnlyTime, out dt)) return dt;

            if (value.IndexOfAny(_dateSeparators) > -1)
            {
                if (DateTime.TryParse(value, out dt) && dt.Year >= 1900) return dt;
            }

            return ParseHalfBaked(dtNow, value, isOnlyDate, isOnlyTime, format);
        }

        private static string GetDateTimeString(string value, string formatString)
        {
            try
            {
                value = value.Trim('\'', ' ', '\t');
                if (Regex.IsMatch(value, @"^[0-9]*$"))
                {
                    int len = value.Length;
                    if (len > 4 && len <= 6)
                        return string.Format("{0}-{1}-{2}", value.Left(2), value.Substring(2, 2), value.Substring(4, 2, "0"));
                    else if (len > 6 && len <= 8)
                        return string.Format("{0}-{1}-{2}", value.Left(4), value.Substring(4, 2), value.Substring(6, 2, "0"));
                    else if (len > 8 && len <= 10)
                        return string.Format("{0}-{1}-{2} {3}", value.Left(4), value.Substring(4, 2), value.Substring(6, 2), value.Substring(8, 2, "0"));
                    else if (len > 10 && len <= 12)
                    {
                        if (formatString.Contains("ss"))
                            return string.Format("20{0}-{1}-{2} {3}:{4}:{5}", value.Left(2), value.Substring(2, 2), value.Substring(4, 2), value.Substring(6, 2), value.Substring(8, 2), value.Substring(10, 2, "0"));
                        else
                            return string.Format("{0}-{1}-{2} {3}:{4}", value.Left(4), value.Substring(4, 2), value.Substring(6, 2), value.Substring(8, 2), value.Substring(10, 2, "0"));
                    }
                    else if (len > 12)
                        return string.Format("{0}-{1}-{2} {3}:{4}:{5}", value.Left(4), value.Substring(4, 2), value.Substring(6, 2), value.Substring(8, 2), value.Substring(10, 2), value.Substring(12, 2, "0"));
                }

                DateTime dt = System.Convert.ToDateTime(value);
                if (string.IsNullOrEmpty(formatString))
                    return dt.ToString();
                else
                    return dt.ToString(formatString);
            }
            catch
            {
                return value;
            }
        }

        private static string GetDateTimeString(string value)
        {
            return GetDateTimeString(value, string.Empty);
        }

        private static bool TryParseKeyWord(DateTime dtNow, string value, out DateTime result)
        {
            result = DateTime.MinValue;

            if (value.Equals("now", StringComparison.CurrentCultureIgnoreCase))
                result = dtNow;
            else if (value.Equals("today", StringComparison.CurrentCultureIgnoreCase))
                result = dtNow.Date;
            else
                return false;

            return false;
        }

        private static bool TryParseDouble(DateTime dtNow, string value, bool isOnlyTime, out DateTime result)
        {
            result = DateTime.MinValue;

            if (value.IndexOfAny(_timeSeparators) > -1) return false;

            double dv;
            if (double.TryParse(value, out dv))
            {
                try
                {
                    switch (value[0])
                    {
                        case '+':
                        case '-':
                            if (isOnlyTime)
                                result = dtNow.AddHours(dv);
                            else
                                result = dtNow.AddDays(dv);
                            return true;
                        default:
                            if (isOnlyTime)
                            {
                                result = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day).AddHours(dv);//从0开始
                                return true;
                            }

                            int days = GetMonthDays(dtNow.Year, dtNow.Month);
                            if (dv <= days)
                            {
                                result = new DateTime(dtNow.Year, dtNow.Month, 1).AddDays(dv - 1);//从1开始
                                return true;
                            }

                            return TryParseNoSeperateDate(dtNow, (int)dv, out result);
                    }
                }
                catch { }
            }
            return false;
        }

        private static bool TryParseNoSeperateDate(DateTime dtNow, int value, out DateTime result)
        {
            result = DateTime.MinValue;

            if (value < 0 || value > 99991231) return false;

            int day = value % 100;
            int month = (value / 100) % 100;
            int year = (value / 10000);

            if (month == 0)
                month = dtNow.Month;
            else if (month > 12)
                return false;

            if (year == 0)
                year = dtNow.Year;
            else if (year < 100)
                year = (dtNow.Year / 100) * 100 + year;

            result = new DateTime(year, month, 1).AddDays(day - 1);

            return true;
        }

        private static int GetMonthDays(int y, int m)
        {
            int mnext;
            int ynext;

            if (m < 12)
            {
                mnext = m + 1;
                ynext = y;
            }
            else
            {
                mnext = 1;
                ynext = y + 1;
            }

            DateTime dt1 = new DateTime(y, m, 1);
            DateTime dt2 = new DateTime(ynext, mnext, 1);
            TimeSpan diff = dt2 - dt1;
            return diff.Days;
        }

        private static DateTime ParseHalfBaked(DateTime dtNow, string value, bool isOnlyDate, bool isOnlyTime, DateTimeFormatter fi)
        {
            int year = dtNow.Year;
            int month = dtNow.Month;
            int day = dtNow.Day;
            int hour = dtNow.Hour;
            int minute = dtNow.Minute;
            int second = dtNow.Second;

            string datePart = string.Empty;
            string timePart = string.Empty;

            string[] items = value.Split(new char[] { ' ' });
            if (items.Length == 1)
            {
                if (isOnlyDate)
                    datePart = items[0];
                else if (isOnlyTime)
                    timePart = items[0];
                else if (items[0].IndexOfAny(_dateSeparators) > 0)
                    datePart = items[0];
                else
                    timePart = items[0];
            }
            else
            {
                datePart = items[0];
                timePart = items[1];
            }

            items = datePart.Split(_dateSeparators).Reverse().ToArray();
            TryParseDatePart(items, 0, 1, 31, ref day);
            TryParseDatePart(items, 1, 1, 12, ref month);
            TryParseDatePart(items, 2, 1, 9999, ref year);
            if (year < 1900)
                year = (int)Math.Truncate((double)(dtNow.Year / 100)) * 100 + year - (int)Math.Truncate((double)(year / 100)) * 100;

            items = timePart.Split(_timeSeparators);

            if (items.Length == 1 && fi.Precision == DateTimePrecision.Second)
            {
                items = new string[3];
                items[2] = timePart.Substring(4, 2);
                items[1] = timePart.Substring(2, 2);
                items[0] = timePart.Left(2);
            }
            else if (items.Length == 1 && fi.Precision == DateTimePrecision.Minute)
            {
                //格式化时进行后面补零处理 注123格式化为12：03
                items = new string[2];

                //char[] chars = timePart.Replace(":", "").Replace("：", "").ToArray();
                //string txt = string.Format("{0}{1}{2}{3}", chars.Length >= 1 ? chars[0] : '0', chars.Length >= 2 ? chars[1] : '0', chars.Length == 4 ? chars[2] : '0', chars.Length == 4 ? chars[3] : chars.Length == 3 ? chars[2] : '0');
                //items[1] = txt.Right(2);
                //items[0] = txt.Left(2);
                items[1] = timePart.Right(2);
                items[0] = timePart.Left(2);
            }
            hour = 23;
            minute = 59;
            second = 59;
            TryParseTimePart(items, 0, 0, 23, ref hour);
            TryParseTimePart(items, 1, 0, 59, ref minute);
            TryParseTimePart(items, 2, 0, 59, ref second);

            if (!fi.StringFormat.Contains("ss")) second = 0;
            if (isOnlyDate)
                return new DateTime(year, month, 1).AddDays(day - 1);
            else
                return new DateTime(year, month, 1, hour, minute, second).AddDays(day - 1);
        }

        private static void TryParseTimePart(string[] items, int index, int min, int max, ref int result)
        {
            if (index > items.Length - 1) return;

            int value;
            if (!int.TryParse(items[index], out value))
            {
                result = 0;
                return;
            }

            if (value >= min && value <= max) result = value;
        }
        private static void TryParseDatePart(string[] items, int index, int min, int max, ref int result)
        {
            if (index > items.Length - 1) return;

            int value;
            if (!int.TryParse(items[index], out value))
                return;

            if (value >= min && value <= max) result = value;
        }
        #endregion

        #region 农历
        //天干
        private static string[] TianGan = { "甲", "乙", "丙", "丁", "戊", "己", "庚", "辛", "壬", "癸" };
        //地支
        private static string[] DiZhi = { "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥" };
        //十二生肖
        private static string[] ShengXiao = { "鼠", "牛", "虎", "兔", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" };

        //农历日期
        private static string[] DayName =   {"*","初一","初二","初三","初四","初五",
             "初六","初七","初八","初九","初十",
             "十一","十二","十三","十四","十五",
             "十六","十七","十八","十九","二十",
             "廿一","廿二","廿三","廿四","廿五",
             "廿六","廿七","廿八","廿九","三十"};

        //农历月份
        private static string[] MonthName = { "*", "正", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "腊" };
        //公历月计数天
        private static int[] MonthAdd = { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334 };
        //农历数据
        private static int[] LunarData = {2635,333387,1701,1748,267701,694,2391,133423,1175,396438
            ,3402,3749,331177,1453,694,201326,2350,465197,3221,3402
            ,400202,2901,1386,267611,605,2349,137515,2709,464533,1738
            ,2901,330421,1242,2651,199255,1323,529706,3733,1706,398762
            ,2741,1206,267438,2647,1318,204070,3477,461653,1386,2413
            ,330077,1197,2637,268877,3365,531109,2900,2922,398042,2395
            ,1179,267415,2635,661067,1701,1748,398772,2742,2391,330031
            ,1175,1611,200010,3749,527717,1452,2742,332397,2350,3222
            ,268949,3402,3493,133973,1386,464219,605,2349,334123,2709
            ,2890,267946,2773,592565,1210,2651,395863,1323,2707,265877};

        /// <summary>
        /// 获取对应日期的农历
        /// </summary>
        /// <param name="dtDay">公历日期</param>
        /// <returns></returns>
        public static string GetLunarCalendar(DateTime dtDay)
        {
            int nTheDate;
            int nIsEnd;
            int k, m, n, nBit, i;

            int year = dtDay.Year;
            int month = dtDay.Month;
            int day = dtDay.Day;
            string calendar = string.Empty;

            //计算到初始时间1921年2月8日的天数：1921-2-8(正月初一)
            nTheDate = (year - 1921) * 365 + (year - 1921) / 4 + day + MonthAdd[month - 1] - 38;
            if ((year % 4 == 0) && (month > 2))
                nTheDate += 1;

            //计算天干，地支，月，日
            nIsEnd = 0;
            m = 0;
            k = 0;
            n = 0;
            while (nIsEnd != 1)
            {
                if (LunarData[m] < 4095)
                    k = 11;
                else
                    k = 12;
                n = k;
                while (n >= 0)
                {
                    //获取LunarData[m]的第n个二进制位的值
                    nBit = LunarData[m];
                    for (i = 1; i < n + 1; i++)
                        nBit = nBit / 2;
                    nBit = nBit % 2;
                    if (nTheDate <= (29 + nBit))
                    {
                        nIsEnd = 1;
                        break;
                    }
                    nTheDate = nTheDate - 29 - nBit;
                    n = n - 1;
                }
                if (nIsEnd == 1)
                    break;
                m = m + 1;
            }
            year = 1921 + m;
            month = k - n + 1;
            day = nTheDate;

            //return year+"-"+month+"-"+day;            
            if (k == 12)
            {
                if (month == LunarData[m] / 65536 + 1)
                    month = 1 - month;
                else if (month > LunarData[m] / 65536 + 1)
                    month = month - 1;
            }

            //天干
            calendar = TianGan[(year - 4) % 60 % 10].ToString();
            //地支
            calendar += DiZhi[(year - 4) % 60 % 12].ToString() + " ";

            //生肖
            calendar += ShengXiao[(year - 4) % 60 % 12].ToString() + "年 ";

            //农历月
            if (month < 1)
                calendar += "闰" + MonthName[-1 * month].ToString() + "月";
            else
                calendar += MonthName[month].ToString() + "月";

            //农历日
            calendar += DayName[day].ToString() + "日";

            return calendar;
        }

        #endregion
    }

    /// <summary>
    /// 格式化的时间日期。
    /// </summary>
    public class FormatedDateTime
    {
        #region 解析
        static Regex _regDateTime = new Regex(@"^(?<Year>(([1-9]\d)?\d{2}))(?<YM>[-/.年])(?<Month>(0?[1-9]|1[0-2]))(?<MD>[-/.月])(?<Day>(0?[1-9]|[1-2][0-9]|3[0-1]))(?<DT>((\s+|日)\s*)?)(?<Hour>([01]?[0-9]|2[0-3])?)(?<HM>(:|时|小时)?)(?<Minute>([0-5]?[0-9])?)(?<MS>(:|分|分钟)?)(?<Second>([0-5]?[0-9])?)(?<SU>(秒))?$");
        static Regex _regTime = new Regex(@"^(?<Hour>([01]?[0-9]|2[0-3]))(?<HM>(:|时|小时))(?<Minute>[0-5]?[0-9])(?<MS>(:|分|分钟))(?<Second>[0-5]?[0-9])(?<SU>(秒))?$");
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool CanParse(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            return _regDateTime.IsMatch(text) || _regTime.IsMatch(text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static FormatedDateTime Parse(string text)
        {
            Match match = _regDateTime.Match(text);
            if (!match.Success)
                match = _regTime.Match(text);

            if (!match.Success)
                return FormatedDateTime.Empty;

            FormatedDateTime fdt = new FormatedDateTime();
            fdt.DateTimeSeparator = match.Groups["DT"].Value;
            fdt.Day = match.Groups["Day"].Value;
            fdt.Hour = match.Groups["Hour"].Value;
            fdt.HourMinuteSeparator = match.Groups["HM"].Value;
            fdt.Minute = match.Groups["Minute"].Value;
            fdt.MinuteSecondSeparator = match.Groups["MS"].Value;
            fdt.Month = match.Groups["Month"].Value;
            fdt.MonthDaySeparator = match.Groups["MD"].Value;
            fdt.Second = match.Groups["Second"].Value;
            fdt.SecondUnit = match.Groups["SU"].Value;
            fdt.Year = match.Groups["Year"].Value;
            fdt.YearMonthSeparator = match.Groups["YM"].Value;

            return fdt;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public static readonly FormatedDateTime Empty = new FormatedDateTime();

        /// <summary>
        /// 
        /// </summary>
        public string Year { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Month { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Day { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Hour { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Minute { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Second { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string YearMonthSeparator { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string MonthDaySeparator { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string DateTimeSeparator { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string HourMinuteSeparator { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string MinuteSecondSeparator { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string SecondUnit { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime Value { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}", Year, YearMonthSeparator, Month, MonthDaySeparator, Day, DateTimeSeparator, Hour, HourMinuteSeparator, Minute, MinuteSecondSeparator, Second, SecondUnit);
        }
    }


    /// <summary>
    /// 时间日期精度
    /// </summary>
    public enum DateTimePrecision
    {
        /// <summary>
        /// 未设置
        /// </summary>
        None = 0,
        /// <summary>
        /// 年
        /// </summary>
        [Description("年")]
        Year,
        /// <summary>
        /// 月
        /// </summary>
        [Description("月")]
        Month,
        /// <summary>
        /// 日
        /// </summary>
        [Description("日")]
        Day,
        /// <summary>
        /// 时
        /// </summary>
        [Description("时")]
        Hour,
        /// <summary>
        /// 分钟
        /// </summary>
        [Description("分钟")]
        Minute,
        /// <summary>
        /// 秒
        /// </summary>
        [Description("秒")]
        Second,
    }
}
