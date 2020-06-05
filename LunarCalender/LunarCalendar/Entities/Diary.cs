﻿using Dapper.Contrib.Extensions;
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
        public string Title { get; set; }
        public string Keywords { get; set; }
        public string Content { get; set; }
        public DateTime RecordDate { get; set; }
        public bool IsRemindRequired { get; set; }
        public string JobGroup { get; set; }
        public string JobName { get; set; }
        public string JobTypeFullName { get; set; }
        public string CronExpress { get; set; }
        public DateTime? RunningStart { get; set; }
        public DateTime? RunningEnd { get; set; }
        public DateTime RowVersion { get; set; }
        public bool IsDeleted { get; set; }
    }

}