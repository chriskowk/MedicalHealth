using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportWorkItems
{
    public class RegistryHelper
    {
        public static RegistryKey GetRegistryKey(string keyPath)
        {
            RegistryKey localMachineRegistry
                = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                                          Environment.Is64BitOperatingSystem
                                              ? RegistryView.Registry64
                                              : RegistryView.Registry32);

            return string.IsNullOrEmpty(keyPath)
                ? localMachineRegistry
                : localMachineRegistry.OpenSubKey(keyPath);
        }

        public static object GetRegistryValue(string keyPath, string keyName)
        {
            RegistryKey registry = GetRegistryKey(keyPath);
            return registry.GetValue(keyName);
        }
    }

    public class TfsTeamProjectCollectionUri
    {
        public TfsTeamProjectCollectionUri(string key, string value)
        {
            _key = key;
            _value = value;
        }

        private string _key;
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        private string _value;
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", this.Key, this.Value);
        }
    }

}
