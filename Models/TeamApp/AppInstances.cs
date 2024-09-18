using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.TeamApp
{
    public class AppInstances
    {
        public List<AppInstance> appInstances { get; set; }
        public string authorityUrl { get; set; }
    }

    public class AppInstance
    {
        public string id { get; set; }
        public string appDefinitionId { get; set; }
        public string appTemplateId { get; set; }
        public string businessProfileId { get; set; }
        public string appDefinitionVersion { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public bool showInPublicListings { get; set; }
        public string error { get; set; }
        public int status { get; set; }
        public string createdBy { get; set; }
        public string modifiedBy { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime modifiedAt { get; set; }
        public bool isCreated { get; set; }
        public bool hasRequesterInfo { get; set; }
        public string defaultResourceUrl { get; set; }
    }

    public class OrderBy
    {
        public string propertyName { get; set; }
        public bool descending { get; set; }
    }

    public class AppInstancesRequest
    {
        public bool includeTotal { get; set; }
        public int itemLimit { get; set; }
        public int skip { get; set; }
        public OrderBy orderBy { get; set; }
        public List<int> statuses { get; set; }
    }
}
