using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace TFSideKicks.Controls
{
    public class MultiComboBox : ComboBox
    {
        static MultiComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiComboBox), new FrameworkPropertyMetadata(typeof(MultiComboBox)));
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(e.Property, e.NewValue);
        }

        /// <summary>
        /// 选中项列表
        /// </summary>
        public ObservableCollection<MultiCbxBaseData> ChekedItems = new ObservableCollection<MultiCbxBaseData>();

        /// <summary>
        /// ListBox竖向列表
        /// </summary>
        private ListBox _listBoxV;

        /// <summary>
        /// ListBox横向列表
        /// </summary>
        private TextBox _selectedTextBox;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _listBoxV = Template.FindName("PART_ListBox", this) as ListBox;
            _selectedTextBox = Template.FindName("PART_TextBox", this) as TextBox;
            _listBoxV.SelectionChanged += _ListBoxV_SelectionChanged;
            _selectedTextBox.KeyDown += _selectedTextBox_KeyDown;
            _selectedTextBox.TextChanged += _selectedTextBox_TextChanged;

            if (ItemsSource != null)
            {
                foreach (var item in ItemsSource)
                {
                    MultiCbxBaseData bdc = item as MultiCbxBaseData;
                    if (bdc.IsChecked)
                    {
                        _listBoxV.SelectedItems.Add(bdc);
                    }
                }
            }
        }

        private void _selectedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = _selectedTextBox.Text;
        }

        private void _selectedTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
        }

        void _ListBoxV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.AddedItems)
            {
                MultiCbxBaseData datachk = item as MultiCbxBaseData;
                datachk.IsChecked = true;
                if (ChekedItems.IndexOf(datachk) < 0)
                {
                    ChekedItems.Add(datachk);
                }
            }

            foreach (var item in e.RemovedItems)
            {
                MultiCbxBaseData datachk = item as MultiCbxBaseData;
                datachk.IsChecked = false;
                ChekedItems.Remove(datachk);
            }

            string s = string.Join(";", ChekedItems);
            _selectedTextBox.Text = s;
        }

        public class MultiCbxBaseData : INotifyPropertyChanged
        {
            public override string ToString()
            {
                return this.ViewName;
            }

            private int _id;
            /// <summary>
            /// 关联主键
            /// </summary>
            public int ID
            {
                get { return _id; }
                set { _id = value; OnPropertyChanged("ID"); }
            }

            private string _viewName;
            /// <summary>
            /// 显示名称
            /// </summary>
            public string ViewName
            {
                get { return _viewName; }
                set { _viewName = value; OnPropertyChanged("ViewName"); }
            }

            private bool _isChecked;
            /// <summary>
            /// 是否选中
            /// </summary>
            public bool IsChecked
            {
                get { return _isChecked; }
                set { _isChecked = value; OnPropertyChanged("IsChecked"); }
            }

            #region INotifyPropertyChanged
            /// <summary>
            /// Occurs when a property value changes.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Called when [property changed].
            /// </summary>
            /// <param name="name">The name.</param>
            protected void OnPropertyChanged(string name)
            {
                var handler = PropertyChanged;

                if (null != handler)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }
            #endregion // INotifyPropertyChanged
        }
    }
}
