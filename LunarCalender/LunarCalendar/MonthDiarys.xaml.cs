﻿using System;
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

namespace LunarCalendar
{
    /// <summary>
    /// MonthDiarys.xaml 的交互逻辑
    /// </summary>
    public partial class MonthDiarys : Window, IDisposable
    {
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLong(IntPtr hwnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong(IntPtr hMenu, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        private static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

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
            EnableWindowControlBox(false, WindowStateMessage.WS_MINIMIZEBOX);
        }

        private static class WindowStateMessage
        {
            public static int WS_MINIMIZEBOX = 0x00020000;
            public static int WS_MAXIMIZEBOX = 0x00010000;
        }

        private void EnableWindowControlBox(bool enabled, int ws_msg)
        {
            int GWL_STYLE = -16;
            int SWP_NOSIZE = 0x0001;
            int SWP_NOMOVE = 0x0002;
            int SWP_FRAMECHANGED = 0x0020;

            IntPtr handle = new WindowInteropHelper(this).Handle;
            int nStyle = GetWindowLong(handle, GWL_STYLE);
            if (enabled)
                nStyle |= ws_msg;
            else
                nStyle &= ~(ws_msg);

            SetWindowLong(handle, GWL_STYLE, nStyle);
            SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_FRAMECHANGED);
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
            Current = Diaries.FirstOrDefault(a => a.RecordOn >= recordedOn.Date && a.RecordOn < recordedOn.Date.AddDays(1));
            if (Current == null)
            {
                _lvwDiarys.SelectedIndex = -1;
                Current = new Diary() { ID = -1, Title = "<新日记>", Keywords = "", Content = "", RemindFlag = 0, RemindOn = null, RecordOn = new DateTime(_year, _month, _day) };
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

            string sql = $"SELECT * FROM Diary WHERE RecordOn >= '{start:yyyy-MM-dd}' AND RecordOn < '{end:yyyy-MM-dd}'";
            Diaries = new DiaryDAL().GetDiaries(sql);

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
            string message = "确认要删除该日记 “" + title + "” 吗?";
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


        #region IDisposable 成员

        public void Dispose()
        {
            this.Close();
        }

        #endregion
    }
}
