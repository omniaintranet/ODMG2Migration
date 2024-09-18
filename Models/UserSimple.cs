using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public enum DataType
    {
        String = 0,
        Taxonomy = 1,
        StringArray,
        TaxonomyArray
    }
    public class UserSimple
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Id { get; set; }
        public User SPUser { get; set; }
    }
    public class SimpleObject
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Status { get; set; }
    }
    public class SimpleListItem
    {
        public string Column { get; set; }
        public string Value { get; set; } 
    }
    public class SimpleField
    {
        public string Key { get; set; }
        public DataType Type { get; set; }
        public object Value { get; set; }
    }

    

    public class TermObject
    {
        public string TermSetId { get; set; }
        public string TermGuid { get; set; }
        public string Label { get; set; }
        public int WssId { get; set; }
    }
}
