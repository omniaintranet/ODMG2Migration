using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DocTemplate
{
    public class DocumentTemplates
    {
        public string id { get; set; }
        public Settings settings { get; set; }
    }

    public class Settings
    {
        public int type { get; set; }
        public dynamic contents { get; set; }
        public List<string> categoryIds { get; set; }
    }
}
