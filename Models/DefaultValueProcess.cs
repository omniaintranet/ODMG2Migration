using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DefaultValueProcess
    {
        public string DefaultValue { get; set; }
        public List<SubString> SubString { get; set; }
        public List<Replacement> Replacement { get; set; }
    }

    public class SubString
    {
        public int Start { get; set; }
        public string LastIndexOf { get; set; }
    }
     
    public class Replacement
    {
        public string Source { get; set; }
        public string Dest { get; set; }
    }
}
