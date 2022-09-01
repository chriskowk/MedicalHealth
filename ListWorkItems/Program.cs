using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListWorkItems
{
    class Program
    {
        static void Main(string[] args)
        {
            //WorkItemHelper.List(new string[] { "http://172.18.128.166:8080/medicalhealthsy", "$/ClinicalManagement" });
            //TFSHelper.ShowHistory(new string[] { "http://172.18.128.166:8080/medicalhealthsy" });
            //Console.WriteLine("Press any key...");
            //Console.ReadKey();


            // 获取最近签入的100个变更集关联的工作项列表
            //WorkItemHelper.List(args);

            // TFSH.exe 显示最近10条签入历史记录
            TFSHelper.ShowHistory(args);
            Console.ReadKey();

            // SFW.exe 文件夹（及子文件夹）设置可写
            //FileHelper.SetFilesWritable(args);
        }
    }
}
