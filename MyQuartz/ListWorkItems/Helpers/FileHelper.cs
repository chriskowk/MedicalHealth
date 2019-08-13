using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListWorkItems
{
    public static class FileHelper
    {
        public static void SetFilesWritable(string foldername, SearchOption searchOption)
        {
            if (!Directory.Exists(foldername)) return;

            DirectoryInfo folder = new DirectoryInfo(foldername);
            FileInfo[] fileInfos = folder.GetFiles("*", searchOption);
            foreach (FileInfo fi in fileInfos)
            {
                if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                {
                    fi.Attributes = FileAttributes.Normal;
                }
            }
        }

    }
}
