using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace GetIPAddr
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> ipaddrs = new List<string>();
            ManagementClass networkAdapterConfig = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc_NetworkAdapterConfig = networkAdapterConfig.GetInstances();
            foreach (ManagementObject mo in moc_NetworkAdapterConfig)
            {
                string mServiceName = mo["ServiceName"] as string;
                //过滤掉非真实网卡
                if (!(bool)mo["IPEnabled"]) { continue; }

                if (mServiceName.ToLower().Contains("vmnetadapter") || mServiceName.ToLower().Contains("ppoe") || mServiceName.ToLower().Contains("bthpan") || mServiceName.ToLower().Contains("tapvpn") || mServiceName.ToLower().Contains("ndisip") || mServiceName.ToLower().Contains("sinforvnic")) { continue; }
                //bool mDHCPEnabled = (bool)mo["IPEnabled"];//是否启用DHCP
                //string mCaption = mo["Caption"] as string;                
                //string mMACAddress = mo["MACAddress"] as string;                
                string[] mIPAddress = mo["IPAddress"] as string[];
                //string[] mIPSubnet = mo["IPSubnet"] as string[];                
                //string[] mDefaultIPGateway = mo["DefaultIPGateway"] as string[];                
                //string[] mDNSServerSearchOrder = mo["DNSServerSearchOrder"] as string[];                 
                //Console.WriteLine(mDHCPEnabled);                
                //Console.WriteLine(mCaption);                
                //Console.WriteLine(mMACAddress);                
                //PrintArray(mIPAddress);                
                //PrintArray(mIPSubnet);                
                //PrintArray(mDefaultIPGateway);                
                //PrintArray(mDNSServerSearchOrder);                 
                if (mIPAddress != null)
                {
                    foreach (string ip in mIPAddress)
                    {
                        if (ip != "0.0.0.0")
                        {
                            ipaddrs.Add(ip);
                        }
                    }
                }
                mo.Dispose();
            }
            PrintList(ipaddrs);
            Console.WriteLine("-- -- The End -- --");
            Console.ReadKey();
        }

        static void PrintList<T>(List<T> list)
        {
            foreach (T item in list)
            {
                Console.WriteLine(item);
            }
        }

        static void PrintArray<T>(T[] list)
        {
            foreach (T item in list)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();
        }
    }
}
