using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ConsoleSecurity
{
    public static class LocalGroupHelper
    {
        //创建本地用户组
        [DllImport("Netapi32.dll")]
        extern static int NetLocalGroupAdd([MarshalAs(UnmanagedType.LPWStr)] string sName, int Level, ref LOCALGROUP_INFO_1 buf, int parm_err);
        //删除本地组
        [DllImport("Netapi32.dll")]
        extern static int NetLocalGroupDel([MarshalAs(UnmanagedType.LPWStr)] string sName, [MarshalAs(UnmanagedType.LPWStr)] string GroupName);
        //获取本地组信息
        [DllImport("Netapi32.dll")]
        extern static int NetLocalGroupGetInfo([MarshalAs(UnmanagedType.LPWStr)] string sName, [MarshalAs(UnmanagedType.LPWStr)] string GroupName, int Level, out IntPtr bufptr);
        //更改本地组信息
        [DllImport("Netapi32.dll")]
        extern static int NetLocalGroupSetInfo([MarshalAs(UnmanagedType.LPWStr)] string sName, [MarshalAs(UnmanagedType.LPWStr)] string GroupName, int Level, ref LOCALGROUP_INFO_1 buf, int parm_err);
        //枚举全部本地用户组
        [DllImport("Netapi32.dll")]
        extern static int NetLocalGroupEnum([MarshalAs(UnmanagedType.LPWStr)] string sName, int Level, out IntPtr bufPtr, int prefmaxlen, out int entriesread, out int totalentries, out int resume_handle);
        //添加用户到组
        [DllImport("Netapi32.dll")]
        extern static int NetLocalGroupAddMembers([MarshalAs(UnmanagedType.LPWStr)] string sName, [MarshalAs(UnmanagedType.LPWStr)] string GroupName, int Level, ref LOCALGROUP_MEMBERS_INFO_3 buf, int totalentries);
        [DllImport("Netapi32.dll")]
        extern static int NetLocalGroupDelMembers([MarshalAs(UnmanagedType.LPWStr)] string sName, [MarshalAs(UnmanagedType.LPWStr)] string GroupName, int Level, ref LOCALGROUP_MEMBERS_INFO_3 bufPtr, int totalentries);
        //释放API
        [DllImport("Netapi32.dll")]
        extern static int NetApiBufferFree(IntPtr Buffer);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LOCALGROUP_INFO_0
        {
            public string LocalGroup_Name_0;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LOCALGROUP_INFO_1
        {
            public string LocalGroup_Name_1;
            public string LocalGroup_Comment_1;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LOCALGROUP_MEMBERS_INFO_3
        {
            public string DomainName;
        }


        //从本地用户组中删除指定用户
        public static bool LocalGroupDelMembers(string GroupName, string UserName)
        {
            LOCALGROUP_MEMBERS_INFO_3 Members = new LOCALGROUP_MEMBERS_INFO_3();
            Members.DomainName = UserName.ToString();
            if (NetLocalGroupDelMembers(null, GroupName.ToString(), 3, ref Members, 1) != 0)
            {
                throw (new Exception("从本地用户组删除指定用户时出现错误"));
            }
            else
            {
                return true;
            }
        }


        //把用户添加至本地用户组
        public static bool LocalGroupAddMembers(string GroupName, string UserName)
        {
            LOCALGROUP_MEMBERS_INFO_3 Members = new LOCALGROUP_MEMBERS_INFO_3();
            Members.DomainName = UserName.ToString();
            if (NetLocalGroupAddMembers(null, GroupName.ToString(), 3, ref Members, 1) != 0)
            {
                throw (new Exception("把用户添加至本地用户组时出现错误"));
            }
            else
            {
                return true;
            }
        }


        //枚举全部本地用户组信息
        public static string LocalGroupEnum()
        {
            string tempStr = "";
            int entriesread;
            int totalentries;
            int resume_handle;
            IntPtr bufPtr;

            NetLocalGroupEnum(null, 1, out bufPtr, -1, out entriesread, out totalentries, out resume_handle);

            if (entriesread > 0)
            {
                LOCALGROUP_INFO_1[] GroupInfo = new LOCALGROUP_INFO_1[entriesread];
                IntPtr iter = bufPtr;
                tempStr = "<?xml version=\"1.0\" encoding=\"gb2312\" ?>\r\n";
                tempStr += "<INFO>\r\n";
                for (int i = 0; i < entriesread; i++)
                {
                    GroupInfo[i] = (LOCALGROUP_INFO_1)Marshal.PtrToStructure(iter, typeof(LOCALGROUP_INFO_1));
                    iter = (IntPtr)((int)iter + Marshal.SizeOf(typeof(LOCALGROUP_INFO_1)));
                    tempStr += "<ITEM value=\"" + GroupInfo[i].LocalGroup_Comment_1 + "\">" + GroupInfo[i].LocalGroup_Name_1 + "</ITEM>\r\n";
                }
                tempStr += "</INFO>";
            }
            return tempStr;
        }

        /// <summary>
        /// 枚举全部本地用户组名称
        /// </summary>
        /// <returns></returns>
        public static IList<string> LocalGroupNameEnum()
        {
            IList<string> ret = new List<string>();
            int entriesread;
            int totalentries;
            int resume_handle;
            IntPtr bufPtr;

            NetLocalGroupEnum(null, 1, out bufPtr, -1, out entriesread, out totalentries, out resume_handle);

            if (entriesread > 0)
            {
                LOCALGROUP_INFO_1[] GroupInfo = new LOCALGROUP_INFO_1[entriesread];
                IntPtr iter = bufPtr;
                for (int i = 0; i < entriesread; i++)
                {
                    GroupInfo[i] = (LOCALGROUP_INFO_1)Marshal.PtrToStructure(iter, typeof(LOCALGROUP_INFO_1));
                    iter = (IntPtr)((int)iter + Marshal.SizeOf(typeof(LOCALGROUP_INFO_1)));
                    ret.Add(GroupInfo[i].LocalGroup_Name_1);
                }
            }
            return ret;
        }

        //修改本地用户组信息
        public static bool LocalGroupSetInfo(string GroupName, string GroupDescription)
        {
            LOCALGROUP_INFO_1 GroupInfo = new LOCALGROUP_INFO_1();
            GroupInfo.LocalGroup_Name_1 = GroupName.ToString();
            GroupInfo.LocalGroup_Comment_1 = GroupDescription.ToString();
            if (NetLocalGroupSetInfo(null, GroupName.ToString(), 1, ref GroupInfo, 0) != 0)
            {
                throw (new Exception("修改用户组信息时出现错误"));
            }
            else
            {
                return true;
            }
        }


        //读取本地用户组信息
        public static string LocalGroupGetInfo(string GroupName)
        {
            IntPtr bufptr;
            LOCALGROUP_INFO_1 GroupInfo = new LOCALGROUP_INFO_1();
            if (NetLocalGroupGetInfo(null, GroupName.ToString(), 1, out bufptr) != 0)
            {
                throw (new Exception("读取用户组信息时出现错误"));
            }
            else
            {
                GroupInfo = (LOCALGROUP_INFO_1)Marshal.PtrToStructure(bufptr, typeof(LOCALGROUP_INFO_1));
                string tempStr = "<?xml version=\"1.0\" encoding=\"gb2312\" ?>\r\n";
                tempStr += "<INFO>\r\n";
                tempStr += "<NAME>" + GroupInfo.LocalGroup_Name_1 + "</NAME>\r\n";
                tempStr += "<DESCRIPTION>" + GroupInfo.LocalGroup_Comment_1 + "</DESCRIPTION>\r\n";
                tempStr += "</INFO>\r\n";
                NetApiBufferFree(bufptr);
                return tempStr;
            }
        }


        //删除本地用户组
        public static bool LocalGroupDel(string GroupName)
        {
            if (NetLocalGroupDel(null, GroupName) != 0)
            {
                throw (new Exception("删除用户组时出现错误"));
            }
            else
            {
                return true;
            }
        }


        //添加本地用户组
        public static bool LocalGroupAdd(string GroupName, string GroupDescription)
        {
            LOCALGROUP_INFO_1 NewLocalGroup = new LOCALGROUP_INFO_1();

            NewLocalGroup.LocalGroup_Name_1 = GroupName.ToString();

            NewLocalGroup.LocalGroup_Comment_1 = GroupDescription.ToString();

            if (NetLocalGroupAdd(null, 1, ref NewLocalGroup, 0) != 0)
            {
                throw (new Exception("创建用户组时出现错误"));
            }
            else
            {
                return true;
            }
        }
    }
}