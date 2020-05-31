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
        public string Title { get; set; }
        public string Keywords { get; set; }
        public string Content { get; set; }
        public int RemindFlag { get; set; }
        public DateTime RemindOn { get; set; }
        public DateTime RecordOn { get; set; }
        public bool IsDeleted { get; set; }
    }

}
