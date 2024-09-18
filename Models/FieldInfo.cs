using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class FieldInfo
    {
        public bool ColorMe { get; set; }
        public string Id { get; set; }
        public string ContentType { get; set; }
        public string Title { get; set; }
        public string FieldTitle { get; set; }
        public string InternalName { get; set; }
        public FieldType Type { get; set; }
        public ODMFieldInfo AdditionODMField { get; set; }
        public string Value { get; set; }
        public string DefaultValue { get; set; }
        public string SpecialInternalSourceColumnName { get; set; }
    }

    public class ODMFieldInfo
    {
        public EnterpriseProperties.EnterpriseProperty EnterpriseProperty { get; set; }

        public PropertiesSet PropertiesSet { get; set; }


    }

    public class ReplaceField
    {
        public FieldInfo Source { get; set; }
        public FieldInfo Dest { get; set; }
        public string DefaultValue { get; set; }
    }
}
