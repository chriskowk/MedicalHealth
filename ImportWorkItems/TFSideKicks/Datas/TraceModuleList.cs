using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFSideKicks.Controls;

namespace TFSideKicks.Datas
{
    public class TraceModuleList
    {
        private static TraceModuleList dataInit;

        public static TraceModuleList Instance
        {
            get
            {
                if (dataInit == null)
                    dataInit = new TraceModuleList();
                return dataInit;
            }
        }

        private TraceModuleList()
        {
            TraceModuleListData = new ObservableCollection<MultiComboBox.MultiCbxBaseData>()
            {
                new MultiComboBox.MultiCbxBaseData(){
                    ID = 0,
                    ViewName = "w3wp.exe",
                    IsChecked = false
                },
                new MultiComboBox.MultiCbxBaseData(){
                    ID = 1,
                    ViewName = "WcfSvcHost.exe",
                    IsChecked = false
                },
                new MultiComboBox.MultiCbxBaseData(){
                    ID = 2,
                    ViewName = "jssvc.exe",
                    IsChecked = false
                },
                new MultiComboBox.MultiCbxBaseData(){
                    ID = 3,
                    ViewName = "sqlservr.exe",
                    IsChecked = false
                },
                new MultiComboBox.MultiCbxBaseData(){
                    ID = 4,
                    ViewName = "JDBC Thin Client",
                    IsChecked = false
                },
            };
        }
        public ObservableCollection<MultiComboBox.MultiCbxBaseData> TraceModuleListData;
    }
}
