using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DocumentTypes
{
    public class DocumentType
    {
        public string id { get; set; }
        public string parentId { get; set; }
        public int childCount { get; set; }
        public Dictionary<string, string> title { get; set; }
        public Settings settings { get; set; }
        public int secondaryOrderNumber { get; set; }
    }

    public class Settings
    {
        public int type { get; set; }
        public int conversions { get; set; }
        public bool allowAppendices { get; set; }
        public string enterprisePropertySetId { get; set; } 
        public List<string> documentTemplateIds { get; set; }
        public string defaultDocumentTemplateId { get; set; }
        public ReviewReminder reviewReminder { get; set; }
    }
    public class ReviewReminder
    {
        public Schedule schedule { get; set; }
        public ReminderInAdvance reminderInAdvance { get; set; }
        public Task task { get; set; }
        public List<string> personEnterprisePropertyDefinitionIds { get; set; }
    }

    public class ReminderInAdvance
    {
        //type=2:day, type =1 month, type=0 year
        public int type { get; set; }
        public int value { get; set; }
    }
    public class Schedule
    {
        public int type { get; set; }
        public ReminderInAdvance settings { get; set; }
    }
}
