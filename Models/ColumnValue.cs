using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class FieldInfoValue
    {
        public FieldInfo Field { get; set; }
        public string DocumentTypeFieldTitle { get; set; }
        //public FieldInfo DocumentTypeField { get; set; }
        public FieldInfo DraftField { get; set; }
        public string Value { get; set; }
        public string DefaultValue { get; set; }


        public object OriginalValue { get; set; }
        public FieldInfoValue Clone()
        {
            return (FieldInfoValue)this.MemberwiseClone();
        }
    }
}
