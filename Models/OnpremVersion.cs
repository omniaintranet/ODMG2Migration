using Microsoft.SharePoint.Client;
using System.Collections.Generic;

namespace Models
{
    public class OnpremFileVersion
    {
        public File FileVersion { get; set; }
        //public ListItem ListItem { get; set; }
        public string VersionLabel { get; set; }
        public string FileURL { get; set; }
        public Dictionary<string, object> FieldValues { get; set; }
    }
}
