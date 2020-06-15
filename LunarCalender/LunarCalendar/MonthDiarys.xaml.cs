using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.Data.OleDb;
using LunarCalendar.Entities;
using LunarCalendar.SqlContext;
using LunarCalendar.DAL;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using LunarCalendar.JobScheduler;

namespace LunarCalendar
{
    /// <summary>
    /// MonthDiarys.xaml 的交互逻辑
    /// </summary>
    public partial class MonthDiarys : Window, IDisposable
    {
        public IList<Diary> Diaries { get; private set; } = new List<Diary>();
        public Diary Current
        {
            get { return _current; }
            private set
            {
                _current = value;
                _grdDetail.DataContext = _current;
            }
        }

        private Diary _current;

        public MonthDiarys()
        {
            InitializeComponent();

            LoadDiarys(DateTime.Now.Date);
            this.Activated += new EventHandler(MonthDiarys_Activated);
            this.Closing += new System.ComponentModel.CancelEventHandler(MonthDiarys_Closing);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Common.EnableWindowControlBox(new WindowInteropHelper(this).Handle, false, WindowStateMessage.WS_MINIMIZEBOX);
            CheckNewDiary();
        }

        void MonthDiarys_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LoadDiarys(new DateTime(_year, _month, _day));
        }

        void MonthDiarys_Activated(object sender, EventArgs e)
        {

        }

        private void CheckNewDiary()
        {
            DateTime recordedOn = new DateTime(_year, _month, _day);
            Current = Diaries.FirstOrDefault(a => a.RecordDate >= recordedOn.Date && a.RecordDate < recordedOn.Date.AddDays(1));
            if (Current == null)
            {
                _lvwDiarys.SelectedIndex = -1;
                Current = new Diary()
                {
                    ID = -1,
                    Title = "[新日记]",
                    Keywords = string.Empty,
                    Content = string.Empty,
                    IsRemindRequired = false,
                    JobGroup = "TaskGroup",
                    JobName = Guid.NewGuid().ToString(),
                    JobTypeFullName = $"{typeof(RemindJob).FullName},{System.IO.Path.GetFileName(Common.CurrentAssembly.Location)}",
                    CronExpress = string.Empty,
                    RunningStart = null,
                    RunningEnd = null,
                    RecordDate = new DateTime(_year, _month, _day),
                    RowVersion = DateTime.Now
                };
            }

            for (int i = 0; i < _lvwDiarys.Items.Count; i++)
            {
                Diary diary = (Diary)_lvwDiarys.Items[i];
                if (diary.Equals(Current))
                {
                    _lvwDiarys.SelectedIndex = i;
                    break;
                }
            }
            _txtTitle.SelectAll();
            _txtTitle.Focus();
        }

        private int _year = 0;
        private int _month = 0;
        private int _day = 0;
        public void LoadDiarys(DateTime recordedOn)
        {
            DateTime start = new DateTime(recordedOn.Year, recordedOn.Month, 1);
            DateTime end = GetNextMonthFirstDate(recordedOn);

            string sql = $"SELECT * FROM Diary WHERE RecordDate >= '{start:yyyy-MM-dd}' AND RecordDate < '{end:yyyy-MM-dd}'";
            Diaries = new DiaryDAL().GetDiaries(sql);
            foreach (var item in Diaries.Where(a => a.IsRemindRequired).ToList())
            {
                JobCenter.StartScheduleJobAsync(item).Wait();
            }

            this.DataContext = null;
            this.DataContext = Diaries;

            _year = recordedOn.Year;
            _month = recordedOn.Month;
            _day = recordedOn.Day;
            if (_lvwDiarys.Items.Count > 0) _lvwDiarys.SelectedIndex = 0;
        }

        private DateTime GetNextMonthFirstDate(DateTime dt)
        {
            int year = dt.Year;
            int month = dt.Month;

            if (++month > 12) { year++; month = 1; }

            return new DateTime(year, month, 1);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_lvwDiarys.SelectedIndex >= _lvwDiarys.Items.Count - 1) return;
            _lvwDiarys.SelectedIndex++;
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (_lvwDiarys.SelectedIndex == 0) return;
            _lvwDiarys.SelectedIndex--;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveDiary()) LoadDiarys(new DateTime(_year, _month, _day));
        }

        private bool SaveDiary()
        {
            Current.RowVersion = DateTime.Now;
            if (Current.ID > 0)
                return DapperExHelper<Diary>.Update(Current);
            else
                return DapperExHelper<Diary>.Insert(Current) > 0;
        }

        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            CheckNewDiary();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Diary selectedRow = (Diary)_lvwDiarys.SelectedItem;
            string title = selectedRow.Title;
            string message = $"确认要删除该日记 “{title}” 吗?";
            if (MessageBox.Show(message, "删除日记", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                //selectedRow.IsDeleted = true;
                //DapperExHelper<Diary>.Update(selectedRow);
                if (DapperExHelper<Diary>.Delete(selectedRow))
                    LoadDiarys(new DateTime(_year, _month, _day));
                else
                    MessageBox.Show("删除日记失败！");
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (_year == 0) _year = DateTime.Now.Year;
            if (_month == 0) _month = DateTime.Now.Month;

            LoadDiarys(new DateTime(_year, _month, _day));
        }

        private void SaveCloseButton_Click(object sender, RoutedEventArgs e)
        {
            SaveDiary();
            this.Close();
        }

        private void MnuRecordTime_Click(object sender, RoutedEventArgs e)
        {
            // handle ascending/descending last name context menu choices
        }

        private void _lvwDiarys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Current = (Diary)_lvwDiarys.SelectedItem;
        }

        private void _btnRemind_Click(object sender, RoutedEventArgs e)
        {
            using (QuartzCronForm form = new QuartzCronForm())
            {
                form.ShowDialog();
            }
        }


        #region IDisposable 成员

        public void Dispose()
        {
            this.Close();
        }

        #endregion
    }
}
