using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LunarCalendar
{
    public static class AutoManageHelper
    {
        /// <summary>
        /// 将本程序设为开机自启
        /// </summary>
        /// <param name="onOff">自启开关</param>
        /// <returns></returns>
        public static bool SetMeStart(bool onOff)
        {
            string appName = Process.GetCurrentProcess().MainModule.ModuleName;
            string appPath = Process.GetCurrentProcess().MainModule.FileName;
            bool ret = SetAutoStart(onOff, appName, appPath);
            return ret;
        }

        /// <summary>
        /// 检查本程序是否为开机自启
        /// </summary>
        /// <returns></returns>
        public static bool IsMeAutoStart()
        {
            string appPath = Process.GetCurrentProcess().MainModule.FileName;
            bool ret = CheckIsAutoStart(appPath);
            return ret;
        }

        /// <summary>
        /// 将应用程序设为或不设为开机启动
        /// </summary>
        /// <param name="onOff">自启开关</param>
        /// <param name="appName">应用程序名</param>
        /// <param name="appPath">应用程序完全路径</param>
        public static bool SetAutoStart(bool onOff, string appName, string appPath)
        {
            bool ret = true;
            //如果从没有设为开机启动设置到要设为开机启动
            if (!IsExistKey(appName) && onOff)
            {
                ret = SelfRunning(onOff, appName, @appPath);
            }
            //如果从设为开机启动设置到不要设为开机启动
            else if (IsExistKey(appName) && !onOff)
            {
                ret = SelfRunning(onOff, appName, @appPath);
            }
            return ret;
        }

        /// <summary>
        /// 判断注册键值对是否存在，即是否处于开机启动状态
        /// </summary>
        /// <param name="keyName">键值名</param>
        /// <returns></returns>
        private static bool IsExistKey(string keyName)
        {
            try
            {
                bool _exist = false;
                RegistryKey local = Registry.LocalMachine;
                RegistryKey runs = local.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (runs == null)
                {
                    RegistryKey key2 = local.CreateSubKey("SOFTWARE");
                    RegistryKey key3 = key2.CreateSubKey("Microsoft");
                    RegistryKey key4 = key3.CreateSubKey("Windows");
                    RegistryKey key5 = key4.CreateSubKey("CurrentVersion");
                    RegistryKey key6 = key5.CreateSubKey("Run");
                    runs = key6;
                }
                string[] runsName = runs.GetValueNames();
                foreach (string strName in runsName)
                {
                    if (strName.ToUpper() == keyName.ToUpper())
                    {
                        _exist = true;
                        return _exist;
                    }
                }
                return _exist;

            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 写入或删除注册表键值对,即设为开机启动或开机不启动
        /// </summary>
        /// <param name="isStart">是否开机启动</param>
        /// <param name="exeName">应用程序名</param>
        /// <param name="path">应用程序路径带程序名</param>
        /// <returns></returns>
        private static bool SelfRunning(bool isStart, string exeName, string path)
        {
            try
            {
                RegistryKey local = Registry.LocalMachine;
                RegistryKey key = local.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (key == null)
                {
                    local.CreateSubKey("SOFTWARE//Microsoft//Windows//CurrentVersion//Run");
                }

                if (isStart) //若开机自启动则添加键值对
                {
                    key.SetValue(exeName, path);
                }
                else //否则删除键值对
                {
                    string[] keyNames = key.GetValueNames();
                    foreach (string keyName in keyNames)
                    {
                        string keyValue = key.GetValue(keyName).ToString();
                        if (keyValue.ToUpper() == path.ToUpper())
                        {
                            key.DeleteValue(keyName);
                        }
                    }
                }
                key.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "捕获异常", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private static bool CheckIsAutoStart(string path)
        {
            bool ret = false;
            try
            {
                RegistryKey local = Registry.LocalMachine;
                RegistryKey key = local.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (key == null)
                {
                    return false;
                }

                string[] keyNames = key.GetValueNames();
                foreach (string keyName in keyNames)
                {
                    string keyValue = key.GetValue(keyName).ToString();
                    if (keyValue.ToUpper() == path.ToUpper())
                    {
                        ret = true;
                        break;
                    }
                }
                key.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "捕获异常", MessageBoxButton.OK, MessageBoxImage.Error);
                ret = false;
            }

            return ret;
        }
    }
}
