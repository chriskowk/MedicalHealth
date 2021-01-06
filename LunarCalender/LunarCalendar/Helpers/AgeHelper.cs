using System;
using System.ComponentModel;
using System.Text;

namespace LunarCalendar
{
    /// <summary>
    /// 定义以岁月天时的方式表示年龄的类。
    /// </summary>
    [Description("年龄")]
    public class Age : IComparable<Age>, IComparable, INotifyPropertyChanged
    {
        /// <summary>
        /// 构造相对于当前时间的<see cref="Age"/>的实例
        /// </summary>
        /// <param name="birthday"></param>
        public Age(DateTime? birthday)
            : this(birthday, DateTime.Now)
        {
        }

        /// <summary>
        /// 构造指定相对时间的<see cref="Age"/>的实例
        /// </summary>
        /// <param name="birthday"></param>
        /// <param name="reference"></param>
        public Age(DateTime? birthday, DateTime reference)
        {
            Reset(birthday, reference);
        }

        private int _childYearScope = 6;
        public Age(DateTime? birthday, DateTime reference, int childYearScope)
        {
            this._childYearScope = childYearScope;
            Reset(birthday, reference);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="birthday"></param>
        /// <param name="reference"></param>
        public void Reset(DateTime? birthday, DateTime reference)
        {
            if (this.Reference != reference || this.Birthday != birthday)
            {
                this.Reference = reference;
                this.Birthday = birthday;
            }
        }

        /// <summary>
        /// 返回出生日期
        /// </summary>
        public DateTime? Birthday
        {
            get { return _birthday; }
            set
            {
                SetBirthday(value);
                _text = null;
                IsChild = this.Year <= 14;
                IsChildEx = this.Year <= 18;
                IsFreeBlood = this.Year <= 18 || this.Year >= 55;
                IsNeonatal = this.Year == 0 && this.Month == 0 && this.Day <= 28;
                IsChildAddition = this.Year < 6 || this.Year == 6 && this.Month == 0 && this.Day <= 0;
                OnPropertyChanged("");
                OnPropertyChanged("Reference");
                OnPropertyChanged("Year");
                OnPropertyChanged("Month");
                OnPropertyChanged("Day");
                OnPropertyChanged("Hour");
                OnPropertyChanged("Minute");
                OnPropertyChanged("IsChild");
                OnPropertyChanged("IsChildEx");
                OnPropertyChanged("IsNeonatal");
                OnPropertyChanged("IsChildAddition");
                OnPropertyChanged("IsFreeBlood");
            }
        }

        private DateTime? _birthday;
        private void SetBirthday(DateTime? value)
        {
            _birthday = value;
            _totalMonths = -1;

            this.Year = 0;
            this.Month = 0;
            this.Day = 0;
            this.Hour = 0;
            this.Minute = 0;
            if (_birthday == null || _birthday == EmptyBirthday) return;

            TimeSpan span;
            int yearSpan = 0;
            if (value.HasValue)
            {
                span = Reference.Subtract((DateTime)value);
                yearSpan = Reference.Year - ((DateTime)value).Year;

                if (Reference.Month < ((DateTime)value).Month)
                {
                    yearSpan--;
                }
                else if (Reference.Month == ((DateTime)value).Month)
                {
                    if (Reference.Day < ((DateTime)value).Day)
                        yearSpan--;
                    //else if (Reference.Day == ((DateTime)value).Day && Reference.Hour < ((DateTime)value).Hour)
                    //{
                    //    yearSpan--;
                    //}
                }
            }
            else
                span = new TimeSpan();

            this.Year = yearSpan;
            if (this.Year > 6) return;

            int monthSpan = 0;
            monthSpan = (Reference.Month - ((DateTime)value).Month + 12) % 12;

            if (Reference.Year > ((DateTime)value).Year && monthSpan == 0)
            {
                if (Reference.Day < ((DateTime)value).Day)
                    monthSpan = (--monthSpan) + 12;
            }
            else if (Reference.Day < ((DateTime)value).Day)
                monthSpan--;

            Month = monthSpan;

            int[] everyMonthDay;
            if (DateTime.IsLeapYear(((DateTime)value).Year))
            {
                everyMonthDay = new int[] { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            }
            else
            {
                everyMonthDay = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            }
            int daySpan = 0;
            daySpan = (Reference.Day - ((DateTime)value).Day + everyMonthDay[(((DateTime)value).Month + monthSpan - 1) % 12]) % everyMonthDay[(((DateTime)value).Month + monthSpan - 1) % 12];

            if (Reference.Hour < ((DateTime)value).Hour)
                daySpan--;
            else if (Reference.Hour == ((DateTime)value).Hour)//2015-01-28 11:20~~~~205-02-01 11:16 不够4天
            {
                if (Reference.Minute < ((DateTime)value).Minute)
                    daySpan--;
                else if (Reference.Minute == ((DateTime)value).Minute)
                {
                    if (Reference.Second < ((DateTime)value).Second)
                        daySpan--;
                }
            }

            Day = daySpan;

            if (span.Days > 0) return;
            this.Hour = span.Hours;

            if (span.Hours > 0) return;
            this.Minute = span.Minutes;

        }

        /// <summary>
        /// 返回年龄描述文本
        /// </summary>
        public string AgeText
        {
            get { return this.ToString(); }
        }
        /// <summary>
        /// 返回用以表示编码类的年龄描述
        /// </summary>
        public string AgeCode
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (this.Year > 0) sb.AppendFormat("{0}Y", this.Year);
                if (this.Month > 0) sb.AppendFormat("{0}M", this.Month);
                if (this.Day > 0) sb.AppendFormat("{0}D", this.Day);
                if (this.Hour > 0) sb.AppendFormat("{0}H", this.Hour);
                if (this.Minute > 0) sb.AppendFormat("{0}Mi", this.Minute);

                return sb.ToString();
            }
        }

        /// <summary>
        /// 返回格式化的出生日期文本
        /// </summary>
        public string BirthdayText
        {
            get
            {
                return _birthday == null || _birthday == EmptyBirthday ? string.Empty
                   : ((DateTime)_birthday).ToString(Hour > 0 ? "yyyy-MM-dd HH:mm" : "yyyy-MM-dd");
            }
            set
            {
                Birthday = ParseBirthdayOrAge(value, Reference);
            }
        }

        /// <summary>
        /// 返回计算年龄的参照时间
        /// </summary>
        public DateTime Reference { get; private set; }
        /// <summary>
        /// 返回岁数
        /// </summary>
        public int Year { get; private set; }
        /// <summary>
        /// 返回不足一岁的月数
        /// </summary>
        public int Month { get; private set; }
        /// <summary>
        /// 返回不足一月的天数
        /// </summary>
        public int Day { get; private set; }
        /// <summary>
        /// 返回不足一天的小时数
        /// </summary>
        public int Hour { get; private set; }
        /// <summary>
        /// 返回不足一小时的分钟数minute
        /// </summary>
        public int Minute { get; private set; }
        /// <summary>
        /// 返回是否儿童,小于14岁
        /// </summary>
        public bool IsChild { get; private set; }
        /// <summary>
        /// 返回是否儿童,小于18岁
        /// </summary>
        public bool IsChildEx { get; private set; }
        /// <summary>
        /// 新生儿，小于28天
        /// </summary>
        public bool IsNeonatal { get; private set; }
        /// <summary>
        /// 是否为6岁以下儿童
        /// </summary>
        public bool IsChildAddition { get; private set; }
        /// <summary>
        /// 返回是否免费用血（小于18周岁大于55周岁）
        /// </summary>
        public bool IsFreeBlood { get; private set; }

        private string _text;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(_text)) BuildText();

            return _text;
        }

        private void BuildText()
        {
            StringBuilder sb = new StringBuilder();
            if (this.Year > 0) sb.AppendFormat("{0}岁", this.Year);
            if (this.Month > 0) sb.AppendFormat("{0}月", this.Month);
            if (this.Day > 0) sb.AppendFormat("{0}天", this.Day);
            if (this.Hour > 0) sb.AppendFormat("{0}小时", this.Hour);
            if (this.Minute > 0) sb.AppendFormat("{0}分钟", this.Minute);

            _text = sb.ToString();
        }

        #region INotifyPropertyChanged 成员
        /// <summary>
        /// 触发 PropertyChanged 事件
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (_propertyChangedEventHandler != null)
                _propertyChangedEventHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        private PropertyChangedEventHandler _propertyChangedEventHandler;
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                _propertyChangedEventHandler = (PropertyChangedEventHandler)
                    Delegate.Combine(_propertyChangedEventHandler, value);
            }
            remove
            {
                _propertyChangedEventHandler = (PropertyChangedEventHandler)
                    Delegate.Remove(_propertyChangedEventHandler, value);
            }
        }
        #endregion

        /// <summary>
        /// 表示空的出生日期。1800-01-01
        /// </summary>
        public static readonly DateTime EmptyBirthday = new DateTime(1800, 1, 1);
        /// <summary>
        /// 
        /// </summary>
        public bool IsEmpty { get { return Birthday == EmptyBirthday; } }

        /// <summary>
        /// 解析出生日期或年龄
        /// 1、文本为日期且对应的年龄小于150岁时返回文本表示的日期
        /// 2、1~150之间的数字解析为年龄,返回年龄对应的日期
        /// 3、MMDD格式的4位数字且可以按“当前年-MM-DD"格式转换为日期且对应的年龄小于150岁时返回该日期
        /// 4、YYMMDD格式的6位数字且可以按“YY-MM-DD"格式转换为日期且对应的年龄小于150岁时返回该日期
        /// </summary>
        /// <param name="birthdayOrAge"></param>
        /// <param name="reference"></param>
        /// <returns>返回出生日期</returns>
        public static DateTime ParseBirthdayOrAge(string birthdayOrAge, DateTime reference)
        {
            DateTime birthday;

            if (!DateTime.TryParse(birthdayOrAge, out birthday))
            {
                int years;
                if (!int.TryParse(birthdayOrAge, out years)) return EmptyBirthday;
                int len = birthdayOrAge.Length;
                if (len < 3 && len >= 1)
                    birthday = DateTime.Today.AddYears(-1 * years);
                else
                {
                    if (birthdayOrAge == "100")
                    {
                        birthday = DateTime.Today.AddYears(-100);
                    }
                    else
                    {
                        birthday = DateTimeFormatter.ParseDateTime(birthdayOrAge);
                        if (len == 6 && birthday > reference.Date.AddDays(1)) birthday = birthday.AddYears(-100);
                    }
                }
                if (birthday == DateTime.MinValue) return EmptyBirthday;
            }
            double days = reference.Subtract(birthday).TotalDays;
            double maxDays = 55114;/* 150岁*/
            if (days < 0 || days > maxDays) return EmptyBirthday;

            return birthday;
        }

        /// <summary>
        /// 以当前时间为参照解析一个生日为年龄
        /// </summary>
        /// <param name="birthday"></param>
        /// <returns></returns>
        public static Age Parse(DateTime? birthday)
        {
            return new Age(birthday);
        }

        /// <summary>
        /// 解析年龄
        /// </summary>
        /// <param name="birthday"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static Age Parse(DateTime? birthday, DateTime reference)
        {
            return new Age(birthday, reference);
        }

        /// <summary>
        /// 以当前时间为参照解析一个生日为年龄
        /// </summary>
        /// <param name="birthdayOrAge"></param>
        /// <returns></returns>
        public static Age Parse(string birthdayOrAge)
        {
            return Parse(birthdayOrAge, DateTime.Now);
        }

        /// <summary>
        /// 解析年龄
        /// </summary>
        /// <param name="birthdayOrAge"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static Age Parse(string birthdayOrAge, DateTime reference)
        {
            DateTime birthday = ParseBirthdayOrAge(birthdayOrAge, reference);
            return new Age(birthday, reference);
        }

        #region IComparable<Age> 成员
        /// <summary>
        /// 将此实例与指定的<see cref="Age"/>实例进行比较并返回一个对二者的相对值的指示。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Age other)
        {
            if (other == null) return 1;
            int v;
            if ((v = this.Year.CompareTo(other.Year)) != 0)
                return v;

            if ((v = this.Month.CompareTo(other.Month)) != 0)
                return v;

            if ((v = this.Day.CompareTo(other.Day)) != 0)
                return v;

            if ((v = this.Hour.CompareTo(other.Hour)) != 0)
                return v;

            if ((v = this.Minute.CompareTo(other.Minute)) != 0)
                return v;

            return 0;
        }

        #endregion

        #region IComparable 成员
        /// <summary>
        /// 将此实例与指定对象进行比较并返回一个对二者的相对值的指示。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as Age);
        }

        #endregion

        int _totalMonths = -1;
        /// <summary>
        /// 获取从出生到现在的月龄。
        /// </summary>
        /// <returns></returns>
        public int GetTotalMonths()
        {
            if (_totalMonths > 0) return _totalMonths;

            if (!this.Birthday.HasValue)
            {
                _totalMonths = -1;
            }
            else
            {

                DateTime dt = this.Birthday.Value;
                DateTime now = DateTime.Today;

                if (dt > now) return -1;
                _totalMonths = (now.Year - dt.Year) * 12 + (now.Month - dt.Month);
                if (_totalMonths > 0 && now.Day < 30 && now.Day < dt.Day) _totalMonths -= 1;
            }
            return _totalMonths;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class AgeHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="birthDay"></param>
        public AgeHelper(DateTime birthDay)
        {
            SetBirthday(birthDay);
        }
        /// <summary>
        /// 
        /// </summary>
        public int Years { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public int Months { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public int Days { get; private set; }


        private void SetBirthday(DateTime birthday)
        {
            DateTime reference = DateTime.Now;
            this.Years = 0;
            this.Months = 0;
            this.Days = 0;

            TimeSpan span = reference.Subtract(birthday);
            int yearSpan = 0;
            yearSpan = reference.Year - (birthday).Year;

            if (reference.Month < (birthday).Month)
            {
                yearSpan--;
            }
            else if (reference.Month == (birthday).Month)
            {
                if (reference.Day < (birthday).Day)
                    yearSpan--;
                else if (reference.Day == (birthday).Day && reference.Hour < (birthday).Hour)
                {
                    yearSpan--;
                }
            }
            this.Years = yearSpan;

            int monthSpan = 0;
            monthSpan = (reference.Month - (birthday).Month + 12) % 12;
            if (reference.Year > (birthday).Year && monthSpan == 0)
            {
                if (reference.Day < (birthday).Day)
                    monthSpan = (--monthSpan) + 12;
            }
            else if (reference.Day < (birthday).Day)
                monthSpan--;
            this.Months = monthSpan + this.Years * 12;

            this.Days = span.Days;
        }
    }
}
