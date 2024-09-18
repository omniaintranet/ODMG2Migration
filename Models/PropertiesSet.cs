using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public enum PropertyType
    {
        Text = 1,
        Number = 2,
        DateTime = 3,
        Boolean = 4,
        Person = 5,
        Taxonomy = 6,
        EnterpriseKeywords = 7,
        Media = 8,
        RichText = 9,
        Data = 10,
        Language = 11
    }
    public class PropertiesSet
    {
        public string id { get; set; }
        public Dictionary<string, string> title { get; set; }
        public Settings settings { get; set; }
    }


    public class Settings
    {
        public List<Item> items { get; set; }
    }
    public class Item
    {
        public int type { get; set; }
        public string id { get; set; }
        public bool required { get; set; }
        public bool? allowMultipleValues { get; set; }
        public string parentEnterprisePropertyDefinitionId { get; set; }
        public int? limitLevel { get; set; }
        public bool? includeTime { get; set; }
    }
}
