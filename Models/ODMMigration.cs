using Models.EnterpriseProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace Models
{
    public class ODMMigration
    {
        public List<PropertiesSet> PropertiesSets { get; set; }
         

        public List<EnterpriseProperty> EnterpriseProperties { get; set; }
    }
}
