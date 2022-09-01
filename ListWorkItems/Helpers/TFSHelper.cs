using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.Win32;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace ListWorkItems
{
    public static class TFSHelper
    {
        public static void ShowHistory(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: TFSHelper.ShowHistory <URL for TFS>");
                return;
            }

            // 方法1：
            //TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri(args[0]), CredentialCache.DefaultNetworkCredentials);
            //tpc.Authenticate();

            // 方法2：
            NetworkCredential networkCredential = new NetworkCredential("guoshaoyue", "Bronzepen1o3$");
            WindowsCredential winCred = new WindowsCredential(networkCredential);
            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri(args[0]), winCred);
            tpc.EnsureAuthenticated();

            VersionControlServer vcs = tpc.GetService(typeof(VersionControlServer)) as VersionControlServer;
            var histories = vcs.QueryHistory("$/", VersionSpec.Latest, 0, RecursionType.Full, null, null, null, 10, true, false);
            foreach (Changeset change in histories)//每个历史版本下修改了几个文件
            {
                Console.WriteLine($"{change.ChangesetId} {change.CommitterDisplayName} {change.CreationDate:yyyy-MM-dd HH:mm:ss}: {change.Comment}");
            }
        }
    }
}
