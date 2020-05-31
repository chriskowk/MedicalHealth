using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LunarCalendar.Entities
{
    public class BaseEntity
    {
        [Key]
        public int ID { get; set; }
    }
}
