using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunarCalendar.Entities
{

    [Table("Diary")]
    public class Diary : BaseEntity
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged("Title"); }
        }
        private string _keywords;
        public string Keywords
        {
            get { return _keywords; }
            set { _keywords = value; OnPropertyChanged("Keywords"); }
        }
        private string _content;
        public string Content
        {
            get { return _content; }
            set { _content = value; OnPropertyChanged("Content"); }
        }
        private DateTime _recordDate;
        public DateTime RecordDate
        {
            get { return _recordDate; }
            set { _recordDate = value; OnPropertyChanged("RecordDate"); }
        }
        private string _jobGroup;
        public string JobGroup
        {
            get { return _jobGroup; }
            set { _jobGroup = value; OnPropertyChanged("JobGroup"); }
        }
        private string _jobName;
        public string JobName
        {
            get { return _jobName; }
            set { _jobName = value; OnPropertyChanged("JobName"); }
        }
        private string _jobTypeFullName;
        public string JobTypeFullName
        {
            get { return _jobTypeFullName; }
            set { _jobTypeFullName = value; OnPropertyChanged("JobTypeFullName"); }
        }
        private DateTime? _runningStart;
        public DateTime? RunningStart
        {
            get { return _runningStart; }
            set { _runningStart = value; OnPropertyChanged("RunningStart"); }
        }
        private DateTime? _runningEnd;
        public DateTime? RunningEnd
        {
            get { return _runningEnd; }
            set { _runningEnd = value; OnPropertyChanged("RunningEnd"); }
        }
        private DateTime _rowVersion;
        public DateTime RowVersion
        {
            get { return _rowVersion; }
            set { _rowVersion = value; OnPropertyChanged("RowVersion"); }
        }
        private bool _isDeleted;
        public bool IsDeleted
        {
            get { return _isDeleted; }
            set { _isDeleted = value; OnPropertyChanged("IsDeleted"); }
        }
        private bool _isRemindRequired;
        public bool IsRemindRequired
        {
            get { return _isRemindRequired; }
            set { _isRemindRequired = value; OnPropertyChanged("IsRemindRequired"); }
        }
        private string _cronExpress;
        public string CronExpress
        {
            get { return _cronExpress; }
            set
            {
                _cronExpress = value.Trim();
                IsRemindRequired = !string.IsNullOrEmpty(_cronExpress);
                OnPropertyChanged("CronExpress");
            }
        }
    }

}
