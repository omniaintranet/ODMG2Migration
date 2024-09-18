using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SourceData
    {
        public List<ListItemSimple> ItemsCollection { get; set; }
        public List<FieldInfo> FieldsCollection { get; set; }
        public SourceData()
        {
            ItemsCollection = new List<ListItemSimple>();
            FieldsCollection = new List<FieldInfo>();
        }
    }
}
