using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ListItemSimple
    {
        public bool IsSelected { get; set; }
        public int Id { get; set; }
        public string FileRef { get; set; }
        public string NewName { get; set; }
        public string LatestVersion { get; set; }
    }
    public class ListIntranetItemSimple
    {
        public bool Selected { get; set; }
        public int Id { get; set; }
        public string FileRef { get; set; }
        public ListItem Item { get; set; }
    }

    public class ListItemWithFields: ListItemSimple
    { 
        public ListItem Item { get; set; }
    }

    public class SavedListItemSimple
    {
        public string LibraryName { get; set; }
        public List<ListItemSimple> ItemsCollection { get; set; }
    }
}
