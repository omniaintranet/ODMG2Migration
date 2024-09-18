using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SP = Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using Microsoft.SharePoint.Client;

namespace Models.Archived
{
    public class ArchivedInfo
    {
        public string SiteURL { get; set; }
        public SP.List MyList { get; set; }
        public ClientContext MyContext { get; set; }
    }
}
