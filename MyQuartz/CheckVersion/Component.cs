using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.Linq.Expressions;
using System.Windows;

namespace CheckVersion
{
    [Table(Name = "Components")]
    public class Component
    {
        [Column(IsPrimaryKey = true)]
        public int ComponentID { get; set; }

        [Column]
        public string FileName { get; set; }

        [Column]
        public long FileSize { get; set; }

        [Column]
        public DateTime CompileDateTime { get; set; }
    }

    public class DBScript
    {
        [Column(IsPrimaryKey = true)]
        public int DBScriptID { get; set; }

        [Column]
        public string CreateEmployeeName { get; set; }

        [Column]
        public string Description { get; set; }

        [Column]
        public string Note { get; set; }

        [Column]
        public string BranchVersion { get; set; }

        [Column]
        public DateTime LastedModifyDateTime { get; set; }
    }
}
