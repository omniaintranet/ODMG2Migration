using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DocumentInfoRequest
    {
        public string documentUrl { get; set; }
        public object internalNames { get; set; }
        public string siteUrl { get; set; }
        public string versionHash { get; set; }
    }
}
