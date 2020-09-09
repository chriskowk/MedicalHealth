using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Interop;

namespace LunarCalendar
{
    /// <summary>
    /// 正确表达式：0 0 5 ? * 2-6  或者 0 0 5 ? * MON-FRI （Cron支持配置星期缩写 MON TUE WED THU FRI SAT SUN）
    /// 星期数值枚举如下
    /// </summary>
    public enum WeekdayEnum
    {
        SUNDAY = 1,
        MONDAY = 2,
        TUESDAY = 3,
        WEDNESDAY = 4,
        THURSDAY = 5,
        FRIDAY = 6,
        SATURDAY = 7
    }
    /// <summary>
    /// 
    /// </summary>
    public static class WindowStateMessage
    {
        public static int WS_MINIMIZEBOX = 0x00020000;
        public static int WS_MAXIMIZEBOX = 0x00010000;
    }

    /// <summary>
    /// 公共类库
    /// </summary>
    public class Common
    {
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLong(IntPtr hwnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong(IntPtr hMenu, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        private static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
        public static string REMIND_CONTENT = "RemindContent";
        public static void EnableWindowControlBox(IntPtr handle, bool enabled, int ws_msg)
        {
            int GWL_STYLE = -16;
            int SWP_NOSIZE = 0x0001;
            int SWP_NOMOVE = 0x0002;
            int SWP_FRAMECHANGED = 0x0020;

            //IntPtr handle = new WindowInteropHelper(this).Handle;
            int nStyle = GetWindowLong(handle, GWL_STYLE);
            if (enabled)
                nStyle |= ws_msg;
            else
                nStyle &= ~(ws_msg);

            SetWindowLong(handle, GWL_STYLE, nStyle);
            SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_FRAMECHANGED);
        }

        public static string ExecBatch(string batPath, bool useShellExecute, bool redirectStandardOutput, bool redirectStandardError, string arguments, ref string errMsg)
        {
            string outputString = string.Empty;

            using (Process pro = new Process())
            {
                FileInfo file = new FileInfo(batPath);
                ProcessStartInfo psi = new ProcessStartInfo(batPath, arguments)
                {
                    WorkingDirectory = file.Directory.FullName,
                    CreateNoWindow = false,
                    RedirectStandardOutput = redirectStandardOutput,
                    RedirectStandardError = redirectStandardError,
                    UseShellExecute = useShellExecute
                };
                pro.StartInfo = psi;
                pro.Start();
                pro.WaitForExit();

                outputString = redirectStandardOutput ? pro.StandardOutput.ReadToEnd() : string.Empty;
                errMsg = redirectStandardError ? pro.StandardError.ReadToEnd() : string.Empty;
            }
            return outputString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weekday"></param>
        /// <returns></returns>
        public static string GetWeekDayText(WeekdayEnum weekday)
        {
            string ret = string.Empty;
            switch (weekday)
            {
                case WeekdayEnum.SUNDAY:
                    ret = "星期日";
                    break;
                case WeekdayEnum.MONDAY:
                    ret = "星期一";
                    break;
                case WeekdayEnum.TUESDAY:
                    ret = "星期二";
                    break;
                case WeekdayEnum.WEDNESDAY:
                    ret = "星期三";
                    break;
                case WeekdayEnum.THURSDAY:
                    ret = "星期四";
                    break;
                case WeekdayEnum.FRIDAY:
                    ret = "星期五";
                    break;
                case WeekdayEnum.SATURDAY:
                    ret = "星期六";
                    break;
                default:
                    break;
            }
            return ret;
        }

        public static Assembly CurrentAssembly => Assembly.GetExecutingAssembly();

        public static bool IsInt(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*$");
        }
    }
}
