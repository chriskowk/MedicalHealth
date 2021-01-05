using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Windows.Forms;
using Quartz;
using Quartz.Spi;

namespace LunarCalendar
{
    public partial class QuartzCronForm : Form
    {
        public QuartzCronForm()
        {
            InitializeComponent();
            CreateControls();
            InitControlValue();
            BindControlEvent(Controls);
            //生成cron
            txtExpression.Text = GenerateCron();
            ReloadRunDetail(txtExpression.Text);
        }

        public void InitControlValue()
        {
            var currentYear = DateTime.Now.Year;
            nudYearFrequencyBaseNum.Minimum = currentYear;
            nudYearFrequencyBaseNum.Maximum = currentYear + 100;
            nudYearFrequencyBaseNum.Value = currentYear;
            nudYearCycleBegin.Minimum = currentYear;
            nudYearCycleBegin.Maximum = currentYear + 100;
            nudYearCycleBegin.Value = currentYear;
            nudYearCycleEnd.Minimum = currentYear;
            nudYearCycleEnd.Maximum = currentYear + 100;
            nudYearCycleEnd.Value = currentYear;
        }

        private void QuartzCronForm_Load(object sender, EventArgs e)
        {
            Common.EnableWindowControlBox(this.Handle, false, WindowStateMessage.WS_MINIMIZEBOX);
            Common.EnableWindowControlBox(this.Handle, false, WindowStateMessage.WS_MAXIMIZEBOX);
        }

        #region 生成控件
        private void CreateControls()
        {
            CreateSecondControls();
            CreateMinuteControls();
            CreateHourControls();
            CreateDayControls();
            CreateMonthControls();
            CreateWeekControls();
        }

        private void CreateSecondControls()
        {
            var xStep = 0;
            var yStep = 0;
            int width = rdoAppointSecond.Width + lblAppointSecond.Width;
            int height = lblAppointSecond.Height;
            int px = lblAppointSecond.Left + 5;
            int py = rdoAppointSecond.Top;
            for (var i = 0; i < 60; i++)
            {
                if (i % 10 == 0)
                {
                    xStep = 0;
                    yStep += rdoAppointSecond.Height * 2;
                }
                var chkTime = new CheckBox
                {
                    Text = i.ToString(),
                    Name = "chkSecond" + i,
                    Size = new Size(width, height),
                    Location = new Point(px + width * xStep, py + yStep)
                };
                tabSecond.Controls.Add(chkTime);
                xStep++;
            }
        }

        private void CreateMinuteControls()
        {
            var xStep = 0;
            var yStep = 0;
            int width = rdoAppointMinute.Width + lblAppointMinute.Width;
            int height = lblAppointMinute.Height;
            int px = lblAppointMinute.Left + 5;
            int py = rdoAppointMinute.Top;
            for (var i = 0; i < 60; i++)
            {
                if (i % 10 == 0)
                {
                    xStep = 0;
                    yStep += rdoAppointMinute.Height * 2;
                }
                var chkTime = new CheckBox
                {
                    Text = i.ToString(),
                    Name = "chkMinute" + i,
                    Size = new Size(width, height),
                    Location = new Point(px + width * xStep, py + yStep)
                };
                tabMinute.Controls.Add(chkTime);
                xStep++;
            }
        }

        private void CreateHourControls()
        {
            var xStep = 0;
            var yStep = 0;
            int width = rdoAppointHour.Width + lblAppointHour.Width;
            int height = lblAppointHour.Height;
            int px = lblAppointHour.Left + 5;
            var py = rdoAppointHour.Top;
            for (var i = 0; i < 24; i++)
            {
                if (i % 10 == 0)
                {
                    xStep = 0;
                    yStep += rdoAppointHour.Height * 2;
                }
                var chkTime = new CheckBox
                {
                    Text = i.ToString(),
                    Name = "chkHour" + i,
                    Size = new Size(width, height),
                    Location = new Point(px + width * xStep, py + yStep)
                };
                tabHour.Controls.Add(chkTime);
                xStep++;
            }
        }

        private void CreateDayControls()
        {
            var xStep = 0;
            var yStep = 0;
            int width = rdoAppointDay.Width + lblAppointDay.Width;
            int height = lblAppointDay.Height;
            int px = lblAppointDay.Left + 5;
            var py = rdoAppointDay.Top;
            for (var i = 0; i < 31; i++)
            {
                if (i % 10 == 0)
                {
                    xStep = 0;
                    yStep += rdoAppointDay.Height * 2;
                }
                var chkTime = new CheckBox
                {
                    Text = (i + 1).ToString(),
                    Name = "chkDay" + i,
                    Size = new Size(width, height),
                    Location = new Point(px + width * xStep, py + yStep)
                };
                tabDay.Controls.Add(chkTime);
                xStep++;
            }
        }

        private void CreateMonthControls()
        {
            var xStep = 0;
            var yStep = 0;
            int width = rdoAppointMonth.Width + lblAppointMonth.Width;
            int height = lblAppointMonth.Height;
            int px = lblAppointMonth.Left + 5;
            var py = rdoAppointMonth.Top;
            for (var i = 0; i < 12; i++)
            {
                if (i % 10 == 0)
                {
                    xStep = 0;
                    yStep += rdoAppointMonth.Height * 2;
                }
                var chkTime = new CheckBox
                {
                    Text = (i + 1).ToString(),
                    Name = "chkMonth" + i,
                    Size = new Size(width, height),
                    Location = new Point(px + width * xStep, py + yStep)
                };
                tabMonth.Controls.Add(chkTime);
                xStep++;
            }
        }

        private void CreateWeekControls()
        {
            var xStep = 0;
            var yStep = 0;
            int width = (rdoAppointWeek.Width + lblAppointWeek.Width) * 2;
            int height = lblAppointWeek.Height;
            int px = lblAppointWeek.Left + 5;
            var py = rdoAppointWeek.Top;
            for (var i = 0; i < 7; i++)
            {
                if (i % 4 == 0)
                {
                    xStep = 0;
                    yStep += rdoAppointWeek.Height * 2;
                }
                var chkTime = new CheckBox
                {
                    Text = Common.GetWeekDayText((WeekdayEnum)(i + 1)),
                    Tag = i + 1,
                    Name = "chkWeek" + i,
                    Size = new Size(width, height),
                    Location = new Point(px + width * xStep, py + yStep)
                };
                tabWeek.Controls.Add(chkTime);
                xStep++;
            }
        }

        #endregion

        /// <summary>
        /// 为控件绑定事件
        /// </summary>
        private void BindControlEvent(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control.HasChildren)
                {
                    BindControlEvent(control.Controls);
                }
                else
                {
                    //checkbox事件
                    if (control is CheckBox chk)
                    {
                        chk.CheckedChanged += checkBox_CheckedChanged;
                        chk.CheckedChanged += GenerateExpressionAndShowDetail;
                    }
                    //numericUpDown事件
                    if (control.Parent is NumericUpDown nud)
                    {
                        nud.TextChanged += numericUpDown_ValueChanged;
                        nud.MouseDown += numericUpDown_ValueChanged;
                        nud.MouseUp += numericUpDown_ValueChanged;
                        nud.TextChanged += GenerateExpressionAndShowDetail;
                        nud.MouseDown += GenerateExpressionAndShowDetail;
                        nud.MouseUp += GenerateExpressionAndShowDetail;
                    }
                    //radio事件
                    if (control is RadioButton rdo)
                    {
                        rdo.CheckedChanged += radioButton_CheckedChanged;
                        rdo.CheckedChanged += GenerateExpressionAndShowDetail;
                    }
                }
            }
        }

        /// <summary>
        ///通过txtCron获取完整表达式
        /// </summary>
        /// <returns></returns>
        private string GenerateCron()
        {
            var strCron = string.Empty;
            strCron += txtSecondCron.Text + " ";
            strCron += txtMinuteCron.Text + " ";
            strCron += txtHourCron.Text + " ";
            strCron += txtDayCron.Text + " ";
            strCron += txtMonthCron.Text + " ";
            strCron += txtWeekCron.Text + " ";
            strCron += txtYearCron.Text;
            return strCron;
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;
            var currentTab = chk.Parent;
            //设置容器中相应radio选中
            if (chk.Checked)
            {
                foreach (Control control in currentTab.Controls)
                {
                    if (control.Name.Contains("rdoAppoint"))
                    {
                        ((RadioButton)control).Checked = true;
                    }
                }
            }
            var dic = GetCurrentCron(currentTab, GetCurrentCheckBoxValue);
            foreach (var kvp in dic)
            {
                LoadCronField(kvp.Key, kvp.Value);
            }
        }

        private void GenerateExpressionAndShowDetail(object sender, EventArgs e)
        {
            //生成cron
            txtExpression.Text = GenerateCron();
            ReloadRunDetail(txtExpression.Text);
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            var nud = (NumericUpDown)sender;
            var currentTab = nud.Parent;
            foreach (Control control in currentTab.Controls)
            {
                if (nud.Name.Contains("Cycle") && control.Name.Contains("rdoCycle"))
                {
                    ((RadioButton)control).Checked = true;
                    var dic = GetCurrentCron(currentTab, GetCurrentCycleRadioValue);
                    foreach (var kvp in dic)
                    {
                        LoadCronField(kvp.Key, kvp.Value);
                    }
                }
                if (nud.Name.Contains("Frequency") && control.Name.Contains("rdoFrequency"))
                {
                    ((RadioButton)control).Checked = true;
                    var dic = GetCurrentCron(currentTab, GetCurrentFrequencyRadioValue);
                    foreach (var kvp in dic)
                    {
                        LoadCronField(kvp.Key, kvp.Value);
                    }
                }
                if (nud.Name.Contains("Last") && control.Name.Contains("rdoLast"))
                {
                    ((RadioButton)control).Checked = true;
                    var dic = GetCurrentCron(currentTab, GetCurrentLastRadioValue);
                    foreach (var kvp in dic)
                    {
                        LoadCronField(kvp.Key, kvp.Value);
                    }
                }
                if (nud.Name.Contains("Special") && control.Name.Contains("rdoSpecial"))
                {
                    ((RadioButton)control).Checked = true;
                    var dic = GetCurrentCron(currentTab, GetCurrentSpecialRadioValue);
                    foreach (var kvp in dic)
                    {
                        LoadCronField(kvp.Key, kvp.Value);
                    }
                }
                if (nud.Name.Contains("Rencent") && control.Name.Contains("rdoRencent"))
                {
                    ((RadioButton)control).Checked = true;
                    var dic = GetCurrentCron(currentTab, s => nudDayRencent.Value + "W");
                    foreach (var kvp in dic)
                    {
                        LoadCronField(kvp.Key, kvp.Value);
                    }
                }

            }
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            var rdo = (RadioButton)sender;
            var currentTab = rdo.Parent;
            if (rdo.Checked)
            {
                //判断指定rdo下的checkbox是否选中如果没有选中则默认选择第一个
                if (rdo.Name.Contains("rdoAppoint"))
                {
                    var isHaveChecked = false;
                    var firstCheckBox = new CheckBox();
                    var count = 0;
                    foreach (Control control in currentTab.Controls)
                    {
                        if (control.Name.Contains("chk"))
                        {
                            var chk = (CheckBox)control;
                            if (count == 0)
                            {
                                firstCheckBox = chk;
                            }
                            if (chk.Checked)
                            {
                                isHaveChecked = chk.Checked;
                            }
                            count++;
                        }
                    }
                    if (!isHaveChecked)
                    {
                        firstCheckBox.Checked = true;
                    }
                    var dic = GetCurrentCron(currentTab, GetCurrentCheckBoxValue);
                    foreach (var kvp in dic)
                    {
                        LoadCronField(kvp.Key, kvp.Value);
                    }
                }
                //rdoEvery值
                if (rdo.Name.Contains("Every"))
                {
                    var dic = GetCurrentCron(currentTab, s => "*");
                    foreach (var kvp in dic)
                    {
                        LoadCronField(kvp.Key, kvp.Value);
                    }
                }
                //rdoNotSpecify值
                if (rdo.Name.Contains("NotSpecify"))
                {
                    var dic = rdo.Name.Contains("Year") ? GetCurrentCron(currentTab, s => " ") : GetCurrentCron(currentTab, s => "?");
                    foreach (var kvp in dic)
                    {
                        LoadCronField(kvp.Key, kvp.Value);
                    }
                }
                //rdoCycle值
                if (rdo.Name.Contains("Cycle"))
                {
                    var dic = GetCurrentCron(currentTab, GetCurrentCycleRadioValue);
                    foreach (var kvp in dic)
                    {
                        LoadCronField(kvp.Key, kvp.Value);
                    }
                }
                //rdoFrequency值
                if (rdo.Name.Contains("Frequency"))
                {
                    var dic = GetCurrentCron(currentTab, GetCurrentFrequencyRadioValue);
                    foreach (var kvp in dic)
                    {
                        LoadCronField(kvp.Key, kvp.Value);
                    }
                }
                //rdoLast值
                if (rdo.Name.Contains("Last"))
                {
                    var dic = GetCurrentCron(currentTab, GetCurrentLastRadioValue);
                    foreach (var kvp in dic)
                    {
                        LoadCronField(kvp.Key, kvp.Value);
                    }
                }
                //rdoSpecial值
                if (rdo.Name.Contains("Special"))
                {
                    var dic = GetCurrentCron(currentTab, GetCurrentSpecialRadioValue);
                    foreach (var kvp in dic)
                    {
                        LoadCronField(kvp.Key, kvp.Value);
                    }
                }
                //rdoRencent值
                if (rdo.Name.Contains("Rencent"))
                {
                    var dic = GetCurrentCron(currentTab, s => nudDayRencent.Value + "W");
                    foreach (var kvp in dic)
                    {
                        LoadCronField(kvp.Key, kvp.Value);
                    }
                }
                //星期与日互斥 检测是否点击星期radiobutton
                if (rdo.Name.Contains("Week") && !rdo.Name.Contains("NotSpecify"))
                {
                    //设置日期rdo不选中及表达式变为?
                    ClearCheckedTabDayRadioStatus();
                    LoadCronField("txtDayCron", "?");
                }
                //检测是否点击日期radiobutton
                if (rdo.Name.Contains("Day") && !rdo.Name.Contains("NotSpecify"))
                {
                    //设置星期rdo为不指定及表达式变为?
                    SetTabWeekRadioNotSpecifyChecked();
                    LoadCronField("txtWeekCron", "?");
                }
            }
        }

        /// <summary>
        /// 将day下面的radiobutton置为不选
        /// </summary>
        private void ClearCheckedTabDayRadioStatus()
        {
            foreach (Control control in tabDay.Controls)
            {
                if (!control.Name.Contains("rdo")) continue;
                var rdo = (RadioButton)control;
                if (rdo.Checked)
                {
                    rdo.Checked = false;
                }
            }
        }

        /// <summary>
        /// 设置星期为不指定
        /// </summary>
        private void SetTabWeekRadioNotSpecifyChecked()
        {
            foreach (Control control in tabWeek.Controls)
            {
                if (!control.Name.Contains("rdo")) continue;
                var rdo = (RadioButton)control;
                if (rdo.Checked)
                {
                    rdo.Checked = false;
                }
                if (rdo.Name.Contains("NotSpecify"))
                {
                    rdo.Checked = true;
                }
            }
        }

        private Dictionary<string, string> GetCurrentCron(Control currentTab, Func<Control, string> method)
        {
            var dic = new Dictionary<string, string>();
            var controlName = string.Empty;
            switch (currentTab.Name)
            {
                case "tabSecond":
                    controlName = "txtSecondCron";
                    break;
                case "tabMinute":
                    controlName = "txtMinuteCron";
                    break;
                case "tabHour":
                    controlName = "txtHourCron";
                    break;
                case "tabDay":
                    controlName = "txtDayCron";
                    break;
                case "tabMonth":
                    controlName = "txtMonthCron";
                    break;
                case "tabWeek":
                    controlName = "txtWeekCron";
                    break;
                case "tabYear":
                    controlName = "txtYearCron";
                    break;
            }
            dic.Add(controlName, method(currentTab));
            return dic;
        }

        private string GetCurrentCheckBoxValue(Control currentTab)
        {
            var strCron = string.Empty;
            foreach (Control control in currentTab.Controls)
            {
                if (!control.Name.Contains("chk")) continue;

                var chk = (CheckBox)control;
                if (chk.Checked)
                {
                    string text = chk.Text.StartsWith("星期") ? chk.Tag.ToString() : chk.Text;
                    strCron += text + ",";
                }
            }
            strCron = string.IsNullOrEmpty(strCron) ? "?" : strCron.Substring(0, strCron.Length - 1);
            return strCron;
        }

        private string GetCurrentCycleRadioValue(Control currentTab)
        {
            var cycleBegin = string.Empty;
            var cycleEnd = string.Empty;
            foreach (Control control in currentTab.Controls)
            {
                if (control.Name.Contains("CycleBegin"))
                {
                    cycleBegin = control.Text;
                }
                if (control.Name.Contains("CycleEnd"))
                {
                    cycleEnd = control.Text;
                }
            }
            return cycleBegin + "-" + cycleEnd;
        }

        private string GetCurrentFrequencyRadioValue(Control currentTab)
        {
            var baseNum = string.Empty;
            var intervalNum = string.Empty;
            foreach (Control control in currentTab.Controls)
            {
                if (control.Name.Contains("FrequencyBaseNum"))
                {
                    baseNum = control.Text;
                }
                if (control.Name.Contains("IntervalNum"))
                {
                    intervalNum = control.Text;
                }
            }
            return baseNum + "/" + intervalNum;
        }

        private string GetCurrentLastRadioValue(Control currentTab)
        {
            var lastCron = "L";
            foreach (Control control in currentTab.Controls)
            {
                if (control.Name == "nudWeekLastDay")
                {
                    lastCron = control.Text + lastCron;
                }
            }
            return lastCron;
        }

        private string GetCurrentSpecialRadioValue(Control currentTab)
        {
            var baseNum = string.Empty;
            var day = string.Empty;
            foreach (Control control in currentTab.Controls)
            {
                if (control.Name.Contains("SpecialBaseNum"))
                {
                    baseNum = control.Text;
                }
                if (control.Name.Contains("SpecialDay"))
                {
                    day = control.Text;
                }
            }
            if (currentTab.Name.Contains("Week"))
            {
                return day + "#" + baseNum;
            }
            return baseNum + "#" + day;
        }

        public bool HasCopied { get; private set; } = false;

        public void SetCornExpress(string cronexpr)
        {
            HasCopied = false;
            txtExpression.Text = cronexpr;
            ReverseButton_Click(null, null);
        }

        /// <summary>
        ///反解析到UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReverseButton_Click(object sender, EventArgs e)
        {
            var cronStr = txtExpression.Text;
            if (string.IsNullOrEmpty(cronStr))
            {
                return;
            }
            ReloadRunDetail(cronStr);
            var crons = ReverseCron(cronStr);
            if (crons != null)
            {
                try
                {
                    ReverseUI(crons);
                }
                catch (Exception ex)
                {
                    txtRunDetail.Text = ex.Message;
                }
            }
        }

        /// <summary>
        /// 解析到表达式字段
        /// </summary>
        /// <param name="cronStr"></param>
        /// <returns></returns>
        private List<string> ReverseCron(string cronStr)
        {
            var crons = cronStr.Split(' ').ToList();
            if (crons.Count >= 6 && crons.Count <= 7)
            {
                txtSecondCron.Text = crons[0];
                txtMinuteCron.Text = crons[1];
                txtHourCron.Text = crons[2];
                txtDayCron.Text = crons[3];
                txtMonthCron.Text = crons[4];
                txtWeekCron.Text = crons[5];
                txtYearCron.Text = string.Empty;
                if (crons.Count == 7)
                {
                    txtYearCron.Text = crons[6];
                }
            }
            else
            {
                return null;
            }
            return crons;
        }

        private void ReloadRunDetail(string cronStr)
        {
            txtRunDetail.Text = String.Empty;
            try
            {
                var timeList = GetTaskeFireTime(cronStr);
                foreach (var time in timeList)
                {
                    var formatTime = Convert.ToDateTime(time).ToString("yyyy-MM-dd HH:mm:ss dddd");
                    txtRunDetail.Text += formatTime + "\r\n";
                }
            }
            catch (Exception ex)
            {
                txtRunDetail.Text = "解析错误：" + ex.Message;
            }
        }

        /// <summary>
        /// 反解析到UI
        /// </summary>
        /// <param name="crons"></param>
        private void ReverseUI(List<string> crons)
        {
            var tabPages = tab.TabPages;
            for (var i = 0; i < crons.Count; i++)
            {
                var tabControls = tabPages[i].Controls;
                //按表达式特征设置控件状态
                if (crons[i] == "*")
                {
                    foreach (Control control in tabControls)
                    {
                        if (!control.Name.Contains("rdo")) continue;
                        var rdo = (RadioButton)control;
                        rdo.Checked = rdo.Name.Contains("Every");
                    }
                }
                if (crons[i].Contains(",") || Common.IsInt(crons[i]))
                {
                    var nums = crons[i].Split(',');
                    foreach (Control control in tabControls)
                    {
                        if (!control.Name.Contains("chk")) continue;
                        var chk = (CheckBox)control;
                        chk.Checked = false;
                        foreach (var num in nums)
                        {
                            if (chk.Text == num || (chk.Tag != null && chk.Tag.ToString() == num))
                            {
                                chk.Checked = true;
                            }
                        }
                    }
                }
                if (crons[i].Contains("-"))
                {
                    var nums = crons[i].Split('-');
                    if (nums.Length != 2)
                    {
                        throw new Exception("表达式格式错误解析\"-\"时失败");
                    }
                    foreach (Control control in tabControls)
                    {
                        if (control.Name.Contains("nud") && control.Name.Contains("Cycle"))
                        {
                            var nud = (NumericUpDown)control;
                            if (nud.Name.Contains("Begin"))
                            {
                                nud.Value = Convert.ToDecimal(nums[0]);
                            }
                            if (nud.Name.Contains("End"))
                            {
                                nud.Value = Convert.ToDecimal(nums[1]);
                            }
                        }
                    }
                }
                if (crons[i].Contains("/"))
                {
                    var nums = crons[i].Split('/');
                    if (nums.Length != 2)
                    {
                        throw new Exception("表达式格式错误解析\"/\"时失败");
                    }
                    foreach (Control control in tabControls)
                    {
                        if (control.Name.Contains("nud") && control.Name.Contains("Frequency"))
                        {
                            var nud = (NumericUpDown)control;
                            if (nud.Name.Contains("BaseNum"))
                            {
                                nud.Value = Convert.ToDecimal(nums[0]);
                            }
                            if (nud.Name.Contains("IntervalNum"))
                            {
                                nud.Value = Convert.ToDecimal(nums[1]);
                            }
                        }
                    }
                }
                if (crons[i] == "?")
                {
                    foreach (Control control in tabControls)
                    {
                        if (!control.Name.Contains("rdo")) continue;
                        var rdo = (RadioButton)control;
                        rdo.Checked = rdo.Name.Contains("NotSpecify");
                    }
                }
                //特殊处理L C忽略
                if (crons[i].Contains("L"))
                {
                    var chars = crons[i].ToCharArray();
                    foreach (Control control in tabControls)
                    {
                        if (!control.Name.Contains("Last")) continue;
                        if (control.Name.Contains("nud"))
                        {
                            var nud = (NumericUpDown)control;
                            nud.Value = Common.IsInt(chars[0].ToString()) ? Convert.ToDecimal(chars[0].ToString()) : 1;
                        }
                        else
                        {
                            if (control.Name.Contains("rdo"))
                            {
                                var rdo = (RadioButton)control;
                                rdo.Checked = rdo.Name.Contains("Last");
                            }
                        }
                    }
                }
                //特殊处理W
                if (crons[i].Contains("W"))
                {
                    var rencentNum = crons[i].Substring(0, crons[i].Length - 1);
                    if (rencentNum.Length > 0)
                    {
                        foreach (Control control in tabControls)
                        {
                            if (!(control.Name.Contains("Rencent") && control.Name.Contains("nud"))) continue;
                            var nud = (NumericUpDown)control;
                            nud.Value = Convert.ToDecimal(rencentNum);
                        }
                    }
                }
                //特殊处理# 星期专用
                if (crons[i].Contains("#"))
                {
                    var nums = crons[i].Split('#');
                    if (nums.Length == 2)
                    {
                        foreach (Control control in tabControls)
                        {
                            if (!control.Name.Contains("undWeekSpecial")) continue;
                            var nud = (NumericUpDown)control;
                            if (nud.Name.Contains("BaseNum"))
                            {
                                nud.Value = Convert.ToDecimal(nums[1]);
                            }
                            if (nud.Name.Contains("Day"))
                            {
                                nud.Value = Convert.ToDecimal(nums[0]);
                            }
                        }
                    }
                }

            }
            //年可为空
            if (crons.Count < 7)
            {
                var tabControls = tabPages[6].Controls;
                foreach (Control control in tabControls)
                {
                    if (!control.Name.Contains("rdo")) continue;
                    var rdo = (RadioButton)control;
                    rdo.Checked = rdo.Name.Contains("NotSpecify");
                }
            }

        }

        /// <summary>
        /// 加载表达式字段
        /// </summary>
        private void LoadCronField(string controlName, string value)
        {
            var controls = this.Controls.Find(controlName, true);
            foreach (var control in controls)
            {
                control.Text = value;
            }
        }

        /// <summary>
        /// 获取任务在未来周期内哪些时间会运行
        /// </summary>
        /// <param name="cronExpression">Cron表达式</param>
        /// <param name="numTimes">运行次数</param>
        /// <returns>运行时间段</returns>
        public static List<string> GetTaskeFireTime(string cronExpression, int numTimes = 50)
        {
            if (numTimes < 0)
            {
                throw new Exception("参数numTimes值大于等于0");
            }
            //时间表达式
            ITrigger trigger = TriggerBuilder.Create().WithCronSchedule(cronExpression).Build();
            IReadOnlyList<DateTimeOffset> dates = TriggerUtils.ComputeFireTimes(trigger as IOperableTrigger, null, numTimes);
            List<string> list = new List<string>();
            foreach (DateTimeOffset dtf in dates)
            {
                list.Add(TimeZoneInfo.ConvertTimeFromUtc(dtf.DateTime, TimeZoneInfo.Local).ToString());
            }
            return list;
        }

        /// <summary>
        /// 复制表达式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtExpression.Text))
            {
                Clipboard.SetDataObject(txtExpression.Text);
                HasCopied = true;
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
