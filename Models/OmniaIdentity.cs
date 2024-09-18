using Microsoft.SharePoint.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Models
{
    public class OmniaIdentityQuery
    {

        public List<int> type { get; set; }
        public string searchText { get; set; }
        public string emailEnabled { get; set; }
        public string permissionEnabled { get; set; }
        public bool tenantIdentityOnly { get; set; }
        public string ownerId { get; set; }
        public object typeId { get; set; }
        public List<string> providerIds { get; set; }
        public List<string> excludeTypeIds { get; set; }
        public int itemLimit { get; set; }
    }


    public class UserIdentity
    {
        public dynamic username { get; set; }
        public dynamic displayName { get; set; }
        public dynamic image { get; set; } 
        public Email email { get; set; }
        public string providerId { get; set; }
        public string providerIdentity { get; set; } 
        public bool isRemoved { get; set; }  
        public string userTypeId { get; set; }
        public int type { get; set; }
        public string id { get; set; }
    }
    public class Email
    {
        public Value value { get; set; }
        public string propertyBindingId { get; set; }
    }
    public class Value
    { 
        public string email { get; set; } 
    }
    public class UserSearchResult
    {
        public int total { get; set; }
        public List<UserIdentity> items { get; set; }
    }
}
