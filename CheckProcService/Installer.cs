using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace CheckProcService
{
    [RunInstaller(true)]
    public partial class ServiceInstaller : Installer
    {
        public ServiceInstaller()
        {
            InitializeComponent();

            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            //this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.User;
            //this.serviceProcessInstaller1.Password = "Bronzepen1o3$";
            //this.serviceProcessInstaller1.Username = "guoshaoyue@hissoft.com";

            this.serviceInstaller1.ServiceName = "procsvcl";
            this.serviceInstaller1.DisplayName = "Check Compiler Manager Running";
            this.serviceInstaller1.Description = "Check compiler process running, otherwise run it immediately.";
            this.serviceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);

            //设置允许服务与桌面交互
            RegistryKey rk = Registry.LocalMachine;
            string key = @"SYSTEM\CurrentControlSet\Services\" + this.serviceInstaller1.ServiceName;
            RegistryKey sub = rk.OpenSubKey(key, true);
            int value = (int)sub.GetValue("Type");
            sub.SetValue("Type", value | 256);
        }
    }
}
