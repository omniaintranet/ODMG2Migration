 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class MappingField: BaseMappingField
    { 
        public FieldInfo SourceObj { get; set; }
        public FieldInfo ODMField { get; set; } 
    }

    public class BaseMappingField
    {
        public string Source { get; set; }
        public string Dest { get; set; }
        public string Value { get; set; }
    }


}
