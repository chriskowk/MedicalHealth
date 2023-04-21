using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Controls.Primitives;
using System.Data;
using System.Linq;
using System.Data.Linq;
using System.Diagnostics;
using System.Data.OleDb;
using LunarCalendar.Entities;
using LunarCalendar.SqlContext;
using LunarCalendar.DAL;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.ServiceModel;
using System.Runtime.InteropServices;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using SWF = System.Windows.Forms;
using System.Threading;
using System.Collections.ObjectModel;

namespace LunarCalendar
{
    /// <summary>
    /// Interaction logic for CalendarWindow.xaml
    /// </summary>
    public partial class CalendarWindow : Window
    {
        private const int MINIMUM_YEAR = 1902;
        private const int MAXIMUM_YEAR = 2100;
        // Specified year, month and day
        private int _year;
        private int _month;
        private int _day;

        private readonly CultureInfo _localCultureInfo = new CultureInfo(CultureInfo.CurrentUICulture.ToString());
        private StackPanel _selectedDatePanel;
        private UniformGrid _calendarDisplayUniformGrid;

        //Display a date with three labels, and 
        //The first label is used to contain the hidden festival text,
        //the second label is used to contain the Gregorian text,
        //the third label is used to contain the Lunar text.
        //private static readonly int _hiddenLabelIndex = 0;
        private static readonly int _mainLabelIndex = 1;
        private static readonly int _subsidiaryLabelIndex = 2;

        private readonly string[] WeekDays = new string[]{
            LunarCalendar.Properties.Resources.Sunday,
            LunarCalendar.Properties.Resources.Monday,
            LunarCalendar.Properties.Resources.Tuesday,
            LunarCalendar.Properties.Resources.Wednesday,
            LunarCalendar.Properties.Resources.Thursday,
            LunarCalendar.Properties.Resources.Friday,
            LunarCalendar.Properties.Resources.Saturday
        };

        private ObservableCollection<Diary> _diaries = new ObservableCollection<Diary>();
        public CalendarWindow()
        {
            try
            {
                ConnectionOption.ConnectionString = SQLiteHelper.ConnectionString;

                InitializeComponent();

                this.Loaded += WindowOnLoad;
                this.CalendarListBox.SelectionChanged += SelectedDateOnDisplay;

                this.Background = Brushes.Black;
                this.ResizeMode = ResizeMode.CanMinimize;
                this.text7.Foreground = Brushes.White;
                this.text8.Foreground = Brushes.Cyan;
                this.txtHolidayTips.Foreground = Brushes.White;
                this.txtAnimalYear.Foreground = Brushes.LightGray;
                this.txtHoroscope.Foreground = Brushes.LightGray;
                this.CalendarListBox.Foreground = Brushes.White;
                this.CalendarListBox.Background = Brushes.Black;
                this.CalendarListBox.Padding = new Thickness(0, 0, 0, 0);

                _year = DateTime.Now.Year;
                _month = DateTime.Now.Month;
                _day = DateTime.Now.Day;

                WeekdayLabelsConfigure();
                SubscribeNavigationButtonEvents();

                InitializTrayIcon();

                this.Top = SystemParameters.WorkArea.Height - this.Height;
                this.Left = SystemParameters.WorkArea.Width - this.Width;
                //this.Top = SWF.Screen.PrimaryScreen.WorkingArea.Height - this.Height;
                //this.Left = SWF.Screen.PrimaryScreen.WorkingArea.Width - this.Width;

                MonthDiarys.StartRemindJobs(DateTime.Now, out ObservableCollection<Diary> diaries);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: except exec. \n{0}", ex.Message);
                return;
            }
            finally
            {
                Debug.WriteLine("final exec.");
            }
        }

        #region Event handler

        void SelectedDateOnDisplay(object sender, SelectionChangedEventArgs e)
        {
            OnCalendarItemSelected();
        }

        void WindowOnLoad(Object sender, EventArgs e)
        {
            DisplayCalendar(_year, _month, _day);
        }

        /// <summary>
        /// 拖拽主窗口移动位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WindowOnMove(Object sender, EventArgs e)
        {
            this.DragMove();
        }

        #endregion

        //Configure the color of the weekday label.
        private void WeekdayLabelsConfigure()
        {
            UIElementCollection labelCollection = this.stackPanel1.Children;
            Label tempLabel;

            for (int i = 0; i < 7; i++)
            {
                tempLabel = (Label)labelCollection[i];
                tempLabel.Foreground = GetCellBrush(i, null);
                tempLabel.Content = WeekDays[i];
            }
        }

        private Brush GetCellBrush(int weekday, UltraCalendar uc)
        {
            Brush brush = Brushes.White;
            if (weekday == 0) brush = Brushes.DarkRed;
            if (weekday == 6) brush = Brushes.DarkGreen;
            if (uc != null && uc.SolarDate.Date == DateTime.Now.Date) brush = Brushes.DarkOrange;
            if (uc != null && !string.IsNullOrEmpty(uc.SolarTerm)) brush = Brushes.Cyan;
            if (uc != null && uc.IsFestival) brush = Brushes.Red;

            return brush;
        }

        private void SubscribeNavigationButtonEvents()
        {
            PreviousYearButton.Click += PreviousYearOnClick;
            NextYearButton.Click += NextYearOnClick;
            PreviousMonthButton.Click += PreviousMonthOnClick;
            NextMonthButton.Click += NextMonthOnClick;
            CurrentMonthButton.Click += CurrentMonthOnClick;
        }

        public void DisplayCalendar(int year, int month, int day)
        {
            DisplayCalendar(year, month);
            DateTime dt = new DateTime(year, month, 1);
            dt = dt.AddDays(day - 1);
            CalendarListBox.SelectedIndex = dt.Day - 1;
            CalendarListBox.Focus();
        }

        public void DisplayCalendar(int year, int month)
        {
            string[] lunarTerms = new string[2] { "", "" };
            int dayNum = DateTime.DaysInMonth(year, month);

            CalendarListBox.BeginInit();
            CalendarListBox.Items.Clear();

            _calendarDisplayUniformGrid = GetCalendarUniformGrid(CalendarListBox);
            DateTime dt = new DateTime(year, month, 1);
            _calendarDisplayUniformGrid.FirstColumn = (int)(dt.DayOfWeek);

            string sql = $"SELECT * FROM Diary WHERE RecordDate >= '{dt:yyyy-MM-dd}' AND RecordDate < '{dt.AddMonths(1):yyyy-MM-dd}'";
            _diaries = new ObservableCollection<Diary>(new DiaryDAL().GetDiaries(sql));
            for (int i = 0; i < dayNum; i++)
            {
                TextBlock mainDateLabel = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = Brushes.Black,
                    Padding = new Thickness(0, 0, 0, 0),
                    Margin = new Thickness(0, 0, 0, 0)
                };

                //This label is used to hold the holiday string.
                Label hiddenLabel = new Label
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Visibility = Visibility.Collapsed
                };

                //If the application is not running in zh-CN env, 
                //it can display the date number bigger.
                mainDateLabel.FontSize = (_localCultureInfo.ToString() == "zh-CN") ? 20 : 25;

                //计算西历转换农历
                UltraCalendar uc = new UltraCalendar(dt);
                string sLunarDate = uc.CLunarDay;
                if (uc.LunarDay == 1) sLunarDate = uc.CLunarMonth + "月";

                //判断该日期是否为农历节气，是则显示该节气
                string sTmp = uc.SolarTerm;
                if (sTmp != string.Empty)
                {
                    sLunarDate = sTmp;  //如为节气则农历日显示节气
                    sTmp = "【" + sTmp + "】" + (uc.SolarDay > 10 ? "" : " ") + uc.SolarDay.ToString() + "日";
                    if (lunarTerms[0] == string.Empty)
                        lunarTerms[0] = sTmp;
                    else
                        lunarTerms[1] = sTmp;
                }

                //Weekend should be dispaly in red color.
                hiddenLabel.Content = uc.IsFestival ? uc.FestivalName : string.Empty;

                mainDateLabel.Foreground = GetCellBrush(uc.Weekday, uc);
                mainDateLabel.Text = dt.Day.ToString(NumberFormatInfo.CurrentInfo);

                //If the application is running in a non zh-CN locale, display no lunar calendar.
                Label subsidiary = null;
                if (_localCultureInfo.ToString() == "zh-CN")
                {
                    subsidiary = new Label
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Background = Brushes.Black,
                        Padding = new Thickness(0, 0, 0, 0),
                        FontSize = 13,

                        //Control the festival date to be red.
                        Foreground = GetCellBrush(uc.Weekday, uc),
                        Content = sLunarDate
                    };
                }

                //Compose the final displaying unit.
                StackPanel stackPanel = new StackPanel
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };

                stackPanel.Children.Add(hiddenLabel);
                stackPanel.Children.Add(mainDateLabel);
                if (subsidiary != null)
                {
                    stackPanel.Children.Add(subsidiary);
                }

                Diary diary = _diaries.FirstOrDefault(a => a.RecordDate >= dt.Date && a.RecordDate < dt.Date.AddDays(1));
                if (diary != null)    //See if having a diary or note
                {
                    mainDateLabel.TextDecorations = GetUnderlineDecoration();

                    stackPanel.ToolTip = new ToolTip();
                    ToolTip tt = (ToolTip)ToolTipService.GetToolTip(stackPanel);
                    tt.HasDropShadow = false;
                    tt.BorderThickness = new Thickness(0);
                    tt.Background = Brushes.Transparent;
                    FancyToolTip toolTip = new FancyToolTip
                    {
                        Title = diary.Title,
                        InfoText = diary.Content,
                        Footer = string.Format("{0:yyyy-MM-dd HH:mm}", diary.RowVersion)
                    };
                    tt.Content = toolTip;
                }

                Border border = new Border
                {
                    Margin = new Thickness(1.5),
                    Child = stackPanel
                };
                CalendarListBox.Items.Add(border);

                //Display the current day in another color
                if (dt.Date == DateTime.Now.Date)
                {
                    border.BorderThickness = new Thickness(1);
                    border.BorderBrush = Brushes.Orange;
                }
                else if (uc.IsFestival)
                {
                    border.BorderThickness = new Thickness(1);
                    border.CornerRadius = new CornerRadius(2);
                    border.BorderBrush = Brushes.DarkGray;
                }

                dt = dt.AddDays(1);
            }

            CalendarListBox.EndInit();
            this.text8.Text = string.Format("{0}\n{1}", lunarTerms[0], lunarTerms[1]);
        }

        private static bool IsWeekEndOrFestival(UltraCalendar uc)
        {
            return uc.Weekday == 6 || uc.Weekday == 0 || uc.IsFestival;
        }

        private UniformGrid GetCalendarUniformGrid(DependencyObject uniformGrid)
        {
            if (uniformGrid is UniformGrid tempGrid)
            {
                return tempGrid;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(uniformGrid); i++)
            {
                DependencyObject gridReturn =
                    GetCalendarUniformGrid(VisualTreeHelper.GetChild(uniformGrid, i));
                if (gridReturn != null)
                {
                    return gridReturn as UniformGrid;
                }
            }
            return null;
        }

        private void UpdateMonth()
        {
            DisplayCalendar(_year, _month, _day);

            //Check the calendar range and disable corresponding buttons
            CheckRange();
        }

        private void PreviousYearOnClick(Object sender, RoutedEventArgs e)
        {
            if (_year <= MINIMUM_YEAR)
            {
                return;
            }

            _year -= 1;
            UpdateMonth();
        }

        private void NextYearOnClick(Object sender, RoutedEventArgs e)
        {
            if (_year >= MAXIMUM_YEAR)
            {
                return;
            }

            _year += 1;
            UpdateMonth();
        }

        private void PreviousMonthOnClick(Object sender, RoutedEventArgs e)
        {
            if (_month == 1 && _year == MINIMUM_YEAR)
            {
                return;
            }

            _month -= 1;
            if (_month == 0)
            {
                _month = 12;
                _year--;
            }
            UpdateMonth();
        }

        private void NextMonthOnClick(Object sender, RoutedEventArgs e)
        {
            if (_month == 12 && _year == MAXIMUM_YEAR)
            {
                return;
            }

            _month += 1;
            if (_month > 12)
            {
                _month = 1;
                _year++;
            }
            UpdateMonth();
        }

        private void CurrentMonthOnClick(Object sender, RoutedEventArgs e)
        {
            _year = DateTime.Now.Year;
            _month = DateTime.Now.Month;
            _day = DateTime.Now.Day;

            UpdateMonth();
        }

        private void OnCalendarItemSelected()
        {
            Border border = (Border)CalendarListBox.SelectedItem;
            if (border == null) return;

            StackPanel stackPanel = (StackPanel)border.Child;
            int selectedDay = CalendarListBox.SelectedIndex + 1;

            if (_selectedDatePanel != null)
            {
                _selectedDatePanel.Background = Brushes.Black;
                ((TextBlock)_selectedDatePanel.Children[_mainLabelIndex]).Background = Brushes.Black;

                //Detect whether the application is running in a zh-CN locale
                if (_localCultureInfo.ToString() == "zh-CN")
                {
                    ((Label)_selectedDatePanel.Children[_subsidiaryLabelIndex]).Background = Brushes.Black;
                }
            }

            if (stackPanel != null)
            {
                ((StackPanel)stackPanel).Background = Brushes.DarkBlue;
                ((TextBlock)stackPanel.Children[_mainLabelIndex]).Background = Brushes.DarkBlue;

                //If the application is not running in zh-CN env,
                //it has no second element.
                if (_localCultureInfo.ToString() == "zh-CN")
                {
                    ((Label)stackPanel.Children[_subsidiaryLabelIndex]).Background = Brushes.DarkBlue;
                }
            }
            _selectedDatePanel = stackPanel;

            if (stackPanel != null)
            {
                this._day = selectedDay;
                DateTime dateTimeDisplay = new DateTime(_year, _month, _day);
                //UltraCalendar uc = new UltraCalendar(dateTimeDisplay);
                //if (!string.IsNullOrEmpty((string)((Label)stackPanel.Children[_hiddenLabelIndex]).Content))
                //{
                //    string festivalString = ((Label)stackPanel.Children[_hiddenLabelIndex]).Content?.ToString();
                //}
                ShowStatus(dateTimeDisplay);
            }
            else
            {
                ShowStatus(new DateTime(_year, _month, 1));
            }
        }

        private void ShowStatus(DateTime dt)
        {
            string strTmp, strFullHolidayDesc;
            UltraCalendar uc = new UltraCalendar(dt);

            this.text7.Text = GetSolarDateDesc(dt);
            strTmp = uc.SolarHoliday;
            txtHolidayTips.Text = strFullHolidayDesc = strTmp;
            strTmp = uc.WeekHoliday;
            if (txtHolidayTips.Text == "")
                txtHolidayTips.Text = strFullHolidayDesc = strTmp;
            else
                strFullHolidayDesc += (strTmp == "" ? "" : "\n" + strTmp);
            strTmp = uc.LunarHoliday;
            if (txtHolidayTips.Text == "")
                txtHolidayTips.Text = strFullHolidayDesc = strTmp;
            else
                strFullHolidayDesc += (strTmp == "" ? "" : "\n" + strTmp);
            txtHolidayTips.Foreground = Brushes.Red;
            txtHolidayTips.ToolTip = string.IsNullOrWhiteSpace(strFullHolidayDesc) ? null : strFullHolidayDesc;

            if (txtHolidayTips.Text == "")
            {
                txtHolidayTips.Foreground = Brushes.White;
                txtHolidayTips.Text = GetLunarDateDesc(dt);
            }

            _changedByCode = true;
            txtYear.Text = _year.ToString();
            scrBar.Value = _year;
            txtAnimalYear.Text = uc.AnimalYear + "年";
            txtHoroscope.Text = uc.Horoscope + "座";
            _changedByCode = false;
        }
        private string GetSolarDateDesc(DateTime dt)
        {
            UltraCalendar uc = new UltraCalendar(dt);
            return uc.SolarYear + "年" + uc.SolarMonth + "月" + uc.SolarDay + "日 " + uc.CWeekday;
        }
        private string GetLunarDateDesc(DateTime dt)
        {
            UltraCalendar uc = new UltraCalendar(dt);
            return uc.CLunarYear + "年" + uc.CLunarMonth + "月" + uc.CLunarDay + " " + uc.CKanChih + "日";
        }

        private string GetFullDateDesc(DateTime dt, bool hasWeekday)
        {
            UltraCalendar uc = new UltraCalendar(dt);
            return uc.SolarYear + "年" + uc.SolarMonth + "月" + uc.SolarDay + "日" + " " + uc.CLunarYear + "年" + uc.CLunarMonth + "月" + uc.CLunarDay + " " + uc.CKanChih + "日" + (hasWeekday ? " " + uc.CWeekday : string.Empty);
        }

        private void CheckRange()
        {
            //The calendar range is between 01/01/1902 and 12/31/2100
            PreviousYearButton.IsEnabled = (_year <= MINIMUM_YEAR) ? false : true;
            PreviousMonthButton.IsEnabled = (_month == 01 && _year <= MINIMUM_YEAR) ? false : true;
            NextYearButton.IsEnabled = (_year >= MAXIMUM_YEAR) ? false : true;
            NextMonthButton.IsEnabled = (_month == 12 && _year >= MAXIMUM_YEAR) ? false : true;
        }

        private bool _changedByCode = false;
        private void scrBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int year;
            if (_changedByCode) return;
            if (!int.TryParse(txtYear.Text, out year)) return;

            year = e.NewValue < e.OldValue ? year + 1 : year - 1;
            if (year < MINIMUM_YEAR || year > MAXIMUM_YEAR) return;

            txtYear.Text = year.ToString();
        }

        private void txtYear_TextChanged(object sender, TextChangedEventArgs e)
        {
            int year;
            if (_changedByCode) return;
            if (!int.TryParse(txtYear.Text, out year)) return;
            if (year < MINIMUM_YEAR || year > MAXIMUM_YEAR) return;

            _year = year;
            UpdateMonth();
        }

        private void CalendarListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            using (MonthDiarys form = new MonthDiarys())
            {
                form.LoadDiarys(new DateTime(_year, _month, _day));
                form.ShowDialog();
            }
            DisplayCalendar(_year, _month, _day);
        }

        private TextDecorationCollection GetUnderlineDecoration()
        {
            // Fill the baseline decoration with a linear gradient brush.
            TextDecorationCollection myCollection = new TextDecorationCollection();
            TextDecoration myBaseline = new TextDecoration
            {
                Location = TextDecorationLocation.Underline
            };

            // Set the linear gradient brush.
            Pen myPen = new Pen
            {
                Brush = new LinearGradientBrush(Colors.Orange, Colors.Red, 0),
                Thickness = 1
            };
            myBaseline.Pen = myPen;
            myBaseline.PenThicknessUnit = TextDecorationUnit.FontRecommended;

            // Set the baseline decoration to the text block.
            myCollection.Add(myBaseline);

            return myCollection;
        }

        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private SWF.NotifyIcon _notifyIcon = null;
        private SWF.ToolStripMenuItem _muiShow = null;
        private Timer _timer;
        private void InitializTrayIcon()
        {
            this.Visibility = Visibility.Hidden;
            _notifyIcon = new SWF.NotifyIcon
            {
                //BalloonTipText = "万年历程序运行中...";
                Text = GetFullDateDesc(DateTime.Now, true),
                Visible = true,
                //重要提示：此处的图标图片在resouces文件夹。可是打包后安装发现无法获取路径，导致程序死机。建议复制一份resouces文件到UI层的bin目录下，确保万无一失。
                Icon = new System.Drawing.Icon(Application.GetResourceStream(new Uri($"images/cal{DateTime.Now:dd}.ico", UriKind.Relative)).Stream)
            };
            //_notifyIcon.ShowBalloonTip(1000);//托盘气泡显示时间
            //双击事件
            _notifyIcon.MouseDoubleClick -= NotifyIcon_MouseClick;
            _notifyIcon.MouseDoubleClick += NotifyIcon_MouseClick;
            //鼠标点击事件
            _notifyIcon.MouseClick -= NotifyIcon_MouseClick;
            _notifyIcon.MouseClick += NotifyIcon_MouseClick;
            //右键弹出式菜单
            SWF.ContextMenuStrip notifyContextMenu = new SWF.ContextMenuStrip();
            _muiShow = new SWF.ToolStripMenuItem("显示日历窗口", new System.Drawing.Bitmap(Application.GetResourceStream(new Uri($"images/screen.png", UriKind.Relative)).Stream), ShowMenuItem_Click);
            SWF.ToolStripMenuItem muiAutoStartNextTime = new SWF.ToolStripMenuItem("下次自动启动")
            {
                Checked = AutoManageHelper.IsMeAutoStart(),
                CheckOnClick = true
            };
            muiAutoStartNextTime.Click -= MuiAutoStartNextTime_Click;
            muiAutoStartNextTime.Click += MuiAutoStartNextTime_Click;
            SWF.ToolStripMenuItem muiAbout = new SWF.ToolStripMenuItem("关于...");
            SWF.ToolStripMenuItem muiExit = new SWF.ToolStripMenuItem("退出", new System.Drawing.Bitmap(Application.GetResourceStream(new Uri($"images/exit.png", UriKind.Relative)).Stream), ExitMenuItem_Click);
            notifyContextMenu.Items.AddRange(new SWF.ToolStripItem[] { _muiShow, muiAutoStartNextTime, muiAbout, new SWF.ToolStripSeparator(), muiExit });
            //关联托盘控件
            _notifyIcon.ContextMenuStrip = notifyContextMenu;
            //时钟按秒
            _timer = new Timer(new TimerCallback(ResetTrayIcon));
            _timer.Change(0, 1000);
        }

        private void MuiAutoStartNextTime_Click(object sender, EventArgs e)
        {
            SWF.ToolStripMenuItem mi = sender as SWF.ToolStripMenuItem;
            AutoManageHelper.SetMeStart(mi.Checked);
        }

        private DateTime _latestRefreshOn = DateTime.Now;
        private bool _isDateChanged = false;
        private void ResetTrayIcon(object state)
        {
            if (_latestRefreshOn.Date != DateTime.Now.Date)
            {
                _isDateChanged = true;
                _notifyIcon.Text = GetFullDateDesc(DateTime.Now, true);
                _notifyIcon.Icon = new System.Drawing.Icon(Application.GetResourceStream(new Uri($"images/cal{DateTime.Now:dd}.ico", UriKind.Relative)).Stream);
            }
            _latestRefreshOn = DateTime.Now;
        }


        // 托盘图标鼠标单击事件
        private void NotifyIcon_MouseClick(object sender, SWF.MouseEventArgs e)
        {
            if (e.Button == SWF.MouseButtons.Left)
            {
                if (this.Visibility == Visibility.Visible)
                {
                    this.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.Visibility = Visibility.Visible;
                    this.Activate();
                }
            }
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Visibility = Visibility.Hidden;
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (_timer != null) _timer.Dispose();
        }

        private void MainWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = bool.Parse(e.NewValue.ToString());
            if (_isDateChanged && isVisible)
            {
                _isDateChanged = false;
                _year = DateTime.Now.Year;
                _month = DateTime.Now.Month;
                _day = DateTime.Now.Day;

                if (!this.IsLoaded) { MainWindow.Show(); }
                DisplayCalendar(_year, _month, _day);
            }
            _muiShow.Text = string.Format("{0}日历窗口", isVisible ? "隐藏" : "显示");
            string imagefile = isVisible ? "noscreen.png" : "screen.png";
            _muiShow.Image = new System.Drawing.Bitmap(Application.GetResourceStream(new Uri($"images/{imagefile}", UriKind.Relative)).Stream);
        }

        private void ShowMenuItem_Click(object sender, EventArgs e)
        {
            if (this.IsVisible)
                this.Visibility = Visibility.Hidden;
            else
            {
                this.Visibility = Visibility.Visible;
                this.Activate();
            }
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要退出本程序吗？", "万年历", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                _notifyIcon.Visible = false;

                //System.Environment.Exit(0);
                Application.Current.Shutdown();
            }
        }

        private void CalendarListBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && (e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                NextMonthOnClick(null, null);
            }
            else if (e.Key == Key.Up && (e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                PreviousMonthOnClick(null, null);
            }
        }
    }
}
