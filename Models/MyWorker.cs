using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class MyWorker
    {
        public int RunMode { get; set; }
        public object Data { get; set; }
        public string TargetSite { get; set; }
        public string ReplaceUser { get; set; }
        public string ReplaceFields { get; set; }
        public string TitleFilters { get; set; }

       // public ODMProcesses odmServices { get; set; }
    }
}
