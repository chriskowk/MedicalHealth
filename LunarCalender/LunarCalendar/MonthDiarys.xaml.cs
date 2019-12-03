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

namespace LunarCalendar
{
    /// <summary>
    /// MonthDiarys.xaml 的交互逻辑
    /// </summary>
    public partial class MonthDiarys : Window, IDisposable
    {
        private DiaryBase _mdbDiarys;
        private IList<Diary> _diaries = new List<Diary>();
        public IList<Diary> Diaries
        {
            get { return _diaries; }
        }

        private Diary _current;

        public MonthDiarys()
        {
            InitializeComponent();

            LoadDiarys(DateTime.Now.Date);
            this.Activated += new EventHandler(MonthDiarys_Activated);
            this.Closing += new System.ComponentModel.CancelEventHandler(MonthDiarys_Closing);
        }

        void MonthDiarys_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LoadDiarys(new DateTime(_year, _month, _day));
        }

        void MonthDiarys_Activated(object sender, EventArgs e)
        {
            CheckNewDiary();
        }

        private void CheckNewDiary()
        {
            DateTime recordedOn = new DateTime(_year, _month, _day);
            _current = _diaries.FirstOrDefault(a => a.RecordDate >= recordedOn.Date && a.RecordDate < recordedOn.Date.AddDays(1));
            if (_current == null)
            {
                _current = new Diary() { ID = -1, Title = "<新日记>", Keywords = "", Content = "", RemindFlag = 0, RemindTime = new DateTime(1900, 1, 1), RecordDate = new DateTime(_year, _month, _day) };
                _diaries.Add(_current);
            }

            for (int i = 0; i < _lvwDiarys.Items.Count; i++)
            {
                Diary diary = (Diary)_lvwDiarys.Items[i];
                if (diary.Equals(_current))
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

            if (_mdbDiarys == null) _mdbDiarys = new DiaryMDB();
            _mdbDiarys.GetLists(a => a.RecordDate >= start && a.RecordDate < end);
            _diaries = _mdbDiarys.Diaries;

            this.DataContext = null;
            this.DataContext = _diaries;

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

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            _mdbDiarys.MoveNext();
        }

        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            _mdbDiarys.MoveLast();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveDiary()) LoadDiarys(new DateTime(_year, _month, _day));
        }

        private bool SaveDiary()
        {
            return _mdbDiarys.Save(_current);
        }

        private void addNewButton_Click(object sender, RoutedEventArgs e)
        {
            CheckNewDiary();
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            Diary selectedRow = (Diary)_lvwDiarys.SelectedItem;
            string title = selectedRow.Title;
            string message = "确认要删除该日记 “" + title + "” 吗?";
            if (MessageBox.Show(message, "删除日记", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                _mdbDiarys.Delete(selectedRow.ID);
                LoadDiarys(new DateTime(_year, _month, _day));
            }
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (_year == 0) _year = DateTime.Now.Year;
            if (_month == 0) _month = DateTime.Now.Month;

            LoadDiarys(new DateTime(_year, _month, _day));
        }

        private void saveCloseButton_Click(object sender, RoutedEventArgs e)
        {
            SaveDiary();
            this.Close();
        }

        private void _recordTime_Click(object sender, RoutedEventArgs e)
        {
            // handle ascending/descending last name context menu choices
        }


        #region IDisposable 成员

        public void Dispose()
        {
            this.Close();
        }

        #endregion
    }
}
