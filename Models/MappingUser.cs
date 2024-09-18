using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{ 
    public class MappingAccount
    {
        public string Source { get; set; }
        public string Dest { get; set; }
    }
    public class MappingDefaultField
    {
        public string Property { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
