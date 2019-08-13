using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;

namespace JobController
{
    /// <summary>
    /// 注册表访问类基类。更多的键访问，可通过派生类来实现。
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors")]
    public class RegistryHelper
    {
        #region ctor 及字段
        /// <summary>
        /// His注册表信息设置的根目录
        /// <para>HKEY_LOCAL_MACHINE\SOFTWARE\JetSun\3.0</para>
        /// </summary>
        public static readonly string RegistryRoot = @"HKEY_LOCAL_MACHINE\SOFTWARE\JetSun\3.0";
        /// <summary>
        /// 是否服务器设置信息路径及键名。
        /// </summary>
        public static readonly string IsServerKey = RegistryRoot + @"\IsServer";
        /// <summary>
        /// 是否调试环境路径及键名。
        /// </summary>
        public static readonly string IsDebugKey = RegistryRoot + @"\IsDebug";
        /// <summary>
        /// 系统运行路径及键名
        /// </summary>
        public static readonly string ExecutablePathKey = RegistryRoot + @"\ExecutablePath";
        /// <summary>
        /// 本地服务路径及键名
        /// </summary>
        public static readonly string LocalServiceKey = RegistryRoot + @"\Runtime\LocalService";

        /// <summary>
        /// 
        /// </summary>
        protected RegistryHelper()
        {
        }
        #endregion

        #region 注册表函数封装
        /// <summary>
        /// 取注册表项对应的值
        /// </summary>
        /// <param name="keyPath">包括键名及值名的全路径</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static object GetValue(string keyPath, object defaultValue)
        {
            string keyName;
            string valueName;
            SplitKey(keyPath, out keyName, out valueName);

            return GetValue(keyName, valueName, defaultValue);
        }

        /// <summary>
        /// 取注册表项对应的值
        /// </summary>
        /// <param name="keyName">键名</param>
        /// <param name="valueName">值名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static object GetValue(string keyName, string valueName, object defaultValue)
        {
            return Registry.GetValue(keyName, valueName, defaultValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hive"></param>
        /// <returns></returns>
        public static RegistryKey GetRootKey(RegistryHive hive)
        {
            switch (hive)
            {
                case RegistryHive.ClassesRoot:
                    return Registry.ClassesRoot;
                case RegistryHive.CurrentConfig:
                    return Registry.CurrentConfig;
                case RegistryHive.CurrentUser:
                    return Registry.CurrentUser;
                //case RegistryHive.DynData:
                //    return Registry.DynData;
                case RegistryHive.LocalMachine:
                    return Registry.LocalMachine;
                case RegistryHive.PerformanceData:
                    return Registry.PerformanceData;
                case RegistryHive.Users:
                    return Registry.Users;
                default:
                    return null;
            }
        }

        private static object Get64Value(string keyName, string valueName)
        {
            return Registry.GetValue(keyName, valueName, string.Empty);
        }
        /// <summary>
        /// 保存注册表项
        /// </summary>
        /// <param name="keyPath">包括键名及值名的全路径</param>
        /// <param name="value">值</param>
        public static void SetValue(string keyPath, object value)
        {
            string keyName;
            string valueName;
            SplitKey(keyPath, out keyName, out valueName);

            Registry.SetValue(keyName, valueName, value);
        }
        /// <summary>
        /// 删除值。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="valueName"></param>
        public static void DeleteValue(string path, string valueName)
        {
            RegistryKey key = GetRegistryKey(path);
            try
            {
                key.DeleteValue(valueName);
            }
            catch (ArgumentException)
            {
            }
        }

        /// <summary>
        /// 保存注册表项
        /// </summary>
        /// <param name="keyName">键名</param>
        /// <param name="valueName">值名</param>
        /// <param name="value">值</param>
        public static void SetValue(string keyName, string valueName, object value)
        {
            Registry.SetValue(keyName, valueName, value);
        }

        /// <summary>
        /// 检索包含与此项关联的所有值名称的字符串数组。
        /// </summary>
        /// <param name="keyPath"></param>
        /// <returns></returns>
        protected static string[] GetValueNames(string keyPath)
        {
            return GetRegistryKey(keyPath).GetValueNames();
        }

        private static RegistryKey GetRegistryKey(string keyPath)
        {
            string subKey;
            RegistryKey rootKey = GetRootRegistryKey(keyPath, out subKey);
            return rootKey.CreateSubKey(subKey);
        }

        private static RegistryKey GetRootRegistryKey(string keyPath, out string subKey)
        {
            foreach (FieldInfo fld in typeof(Registry).GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(a => typeof(RegistryKey).IsAssignableFrom(a.FieldType)))
            {
                RegistryKey key = fld.GetValue(null) as RegistryKey;
                if (keyPath.StartsWith(key.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    subKey = keyPath.Substring(key.Name.Length + 1);
                    return key;
                }
            }
            subKey = keyPath;
            return Registry.LocalMachine;
        }

        /// <summary>
        /// 分解一个路径键名。
        /// </summary>
        /// <param name="keyPath"></param>
        /// <param name="keyName"></param>
        /// <param name="valueName"></param>
        public static void SplitKey(string keyPath, out string keyName, out string valueName)
        {
            int sp = keyPath.LastIndexOf('\\');
            keyName = keyPath.Substring(0, sp);
            valueName = keyPath.Substring(sp + 1);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyPath"></param>
        /// <param name="rootPath"></param>
        /// <param name="subPath"></param>
        public static void SplitRootKey(string keyPath, out string rootPath, out string subPath)
        {
            int inx = keyPath.IndexOf('\\');
            rootPath = keyPath.Substring(0, inx);
            subPath = keyPath.Substring(inx + 1);
        }

        #endregion

        /// <summary>
        /// 用于32位程序访问64位注册表
        /// </summary>
        /// <param name="hive">根级别的名称</param>
        /// <param name="keyName">不包括根级别的名称</param>
        /// <param name="valueName">项名称</param>
        /// <param name="view">注册表视图</param>
        /// <returns>值</returns>
        public static object Get64ValueFrom32(RegistryHive hive, string keyName, string valueName, RegistryView view)
        {
            string oldKey = keyName;
            try
            {
                keyName = keyName.Replace(@"HKEY_LOCAL_MACHINE\", "");
                return RegistryKey.OpenBaseKey(hive, view).OpenSubKey(keyName, true).GetValue(valueName);//获得要访问的键
            }
            catch
            {

            }
            return GetValue(oldKey, valueName, "-1");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hive"></param>
        /// <param name="keyName"></param>
        /// <param name="valueName"></param>
        /// <returns></returns>
        public static object Get32ValueFrom64(RegistryHive hive, string keyName, string valueName)
        {
            RegistryView view = RegistryView.Registry32;
            keyName = keyName.Replace(@"HKEY_LOCAL_MACHINE\", "");
            if (keyName.IndexOf("Wow6432Node", 0, 1, StringComparison.CurrentCultureIgnoreCase) < 0)
                keyName = keyName.Replace(@"SOFTWARE\", @"SOFTWARE\Wow6432Node\");
            return RegistryKey.OpenBaseKey(hive, view).OpenSubKey(keyName, true).GetValue(valueName);//获得要访问的键
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static string Get32Value(string keyName, string valueName)
        {
            string ret = string.Empty;
            try
            {
                if (Environment.Is64BitOperatingSystem)
                    ret = Get32ValueFrom64(Microsoft.Win32.RegistryHive.LocalMachine, keyName, valueName).ToString();
                else
                    ret = Registry.GetValue(keyName, valueName, "").ToString();
            }
            catch
            {
            }
            return ret;
        }

        /// <summary>
        /// 用于32位的程序设置64位的注册表
        /// </summary>
        /// <param name="hive">根级别的名称</param>
        /// <param name="keyName">不包括根级别的名称</param>
        /// <param name="valueName">项名称</param>
        /// <param name="value">值</param>
        /// <param name="kind">值类型</param>
        /// <param name="view">注册表视图</param>
        public static void Set64ValueFrom32(RegistryHive hive, string keyName, string valueName, object value, RegistryValueKind kind, RegistryView view)
        {
            keyName = keyName.Replace(@"HKEY_LOCAL_MACHINE\", "");
            RegistryKey.OpenBaseKey(hive, view).OpenSubKey(keyName, true).SetValue(valueName, value, kind); //需要写的权限,这里的true是关键。 
        }

        /// <summary>
        /// 获取执行文件路径
        /// </summary>
        /// <returns></returns>
        public static string GetExecutablePath()
        {
            return GetValue(ExecutablePathKey, string.Empty).ToString();
        }
    }
}
