using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace LunarCalendar.Entities
{
    public class BaseEntity : INotifyPropertyChanged
    {
        private int _id;
        [Key]
        public int ID
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged("ID"); }
        }
        protected internal virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
