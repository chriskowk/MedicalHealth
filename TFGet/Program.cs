using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TFGet
{
    class Program
    {
        static void Main(string[] args)
        {
            var tfsParams = TfsDownloadParams.Create(args);

            var tpc = new TfsTeamProjectCollection(new Uri(tfsParams.ServerUrl), tfsParams.Credentials);

            CheckAccess(tpc, tfsParams);

            Download(tpc, tfsParams);

            Console.ReadKey();
        }

        private static void CheckAccess(TfsTeamProjectCollection tpc, TfsDownloadParams tfsParams)
        {
            try
            {
                tpc.Authenticate();
            }
            catch (Exception e)
            {
                Console.WriteLine($"TFS Authentication Failed: {e.Message}");
                Console.WriteLine("Server Url:{0}", tfsParams.ServerUrl);
                Console.WriteLine("Project Path:{0}", tfsParams.ServerProjectPath);
                Console.WriteLine("Target Path:{0}", tfsParams.TargetPath);
                Console.ReadKey();

                Environment.Exit(1);
            }
        }

        static void Download(TfsTeamProjectCollection tpc, TfsDownloadParams tfsParams)
        {
            var versionControl = tpc.GetService<VersionControlServer>();
            // Listen for the Source Control events.
            versionControl.NonFatalError += Program.OnNonFatalError;

            var files = versionControl.GetItems(tfsParams.ServerProjectPath, VersionSpec.Latest, RecursionType.Full);
            foreach (Item item in files.Items)
            {
                var localFilePath = GetLocalFilePath(tfsParams, item);

                switch (item.ItemType)
                {
                    case ItemType.Any:
                        throw new ArgumentOutOfRangeException("ItemType.Any - not sure what to do with this");
                    case ItemType.File:
                        if (!tfsParams.Silent) Console.WriteLine("Getting: '{0}'", localFilePath);
                        item.DownloadFile(localFilePath);
                        break;
                    case ItemType.Folder:
                        if (!tfsParams.Silent) Console.WriteLine("Creating Directory: {0}", localFilePath);
                        Directory.CreateDirectory(localFilePath);
                        break;
                }
            }
        }

        private static string GetLocalFilePath(TfsDownloadParams tfsParams, Item item)
        {
            var projectPath = tfsParams.ServerProjectPath;
            var pathExcludingLastFolder = projectPath.Substring(0, projectPath.LastIndexOf('/') + 1);
            string relativePath = item.ServerItem.Replace(pathExcludingLastFolder, "");
            var localFilePath = Path.Combine(tfsParams.TargetPath, relativePath);
            return localFilePath;
        }

        internal static void OnNonFatalError(Object sender, ExceptionEventArgs e)
        {
            var message = e.Exception != null ? e.Exception.Message : e.Failure.Message;
            Console.Error.WriteLine("Exception:" + message);
        }
    }

    public class TfsDownloadParams
    {
        public string ServerUrl { get; set; }
        public string ServerProjectPath { get; set; }
        public string TargetPath { get; set; }
        public WindowsCredential Credentials { get; set; }
        public bool Silent { get; set; }

        public static TfsDownloadParams Create(IList<string> args)
        {
            if (args.Count < 5)
            {
                Console.WriteLine("Please supply 5 or 6 parameters: tfsServerUrl serverProjectPath targetPath userName password [silent]");
                Console.WriteLine("The optional 6th 'silent' parameter will suppress listing each file downloaded");
                Console.WriteLine(@"Ex: tfsget""https://myvso.visualstudio.com/DefaultCollection""""$/MyProject/ProjectSubfolder""""c:\\Projects Folder"", user, password");

                Environment.Exit(1);
            }

            var tfsServerUrl = args[0]; //"https://myvso.visualstudio.com/DefaultCollection";
            var serverProjectPath = args[1]; //"$/MyProject/Folder Path";
            var targetPath = args[2]; // @"c:\\Projects\";
            var userName = args[3]; //"login";
            var password = args[4]; //"passsword";
            var silentFlag = args.Count >= 6 && (args[5].ToLower() == "silent"); //"silent";
            NetworkCredential networkCredential = new NetworkCredential(userName, password);
            WindowsCredential tfsCredentials = new WindowsCredential(networkCredential);

            // 方法1：
            //TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri(args[0]), CredentialCache.DefaultNetworkCredentials);
            //tpc.Authenticate();

            // 方法2：
            //NetworkCredential networkCredential = new NetworkCredential("guoshaoyue", "Bronzepen1o3$");
            //WindowsCredential winCred = new WindowsCredential(networkCredential);
            //TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri(args[0]), winCred);
            //tpc.EnsureAuthenticated();

            var tfsParams = new TfsDownloadParams
            {
                ServerUrl = tfsServerUrl,
                ServerProjectPath = serverProjectPath,
                TargetPath = targetPath,
                Credentials = tfsCredentials,
                Silent = silentFlag,
            };
            return tfsParams;
        }
    }
}