using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class FieldValue
    {
        public string InternalName { get; set; }
        public string Value { get; set; }
    }

    public class MappingColumnsName
    {
        public string SpecialInternalSourceColumnName { get; set; }
        public string SourceColumnName { get; set; }
        public string DestinationColumnName { get; set; }
        public string DefaultValue { get; set; }
    }
}
