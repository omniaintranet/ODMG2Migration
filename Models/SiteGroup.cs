using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class O365Root
    {
        public List<SiteGroup> Row { get; set; }
        public int FirstRow { get; set; }
        public string FolderPermissions { get; set; }
        public int LastRow { get; set; }
        public int RowLimit { get; set; }
        public string FilterLink { get; set; }
        public string ForceNoHierarchy { get; set; }
        public string HierarchyHasIndention { get; set; }
        public string CurrentFolderSpItemUrl { get; set; }
    }

    public class Parameters
    {
        public Parameters() { }
        public bool DatesInUtc { get { return true; } }
        public string Paging { get {
                return "Paged=TRUE&p_Title=APQPTemplateProject&p_ID=4696&PageFirstRow=31&View=00000000-0000-0000-0000-000000000000";
            } }
        public string ViewXml { get {
                return "<View><Query><Where><And><And><IsNull><FieldRef Name=\"TimeDeleted\"/></IsNull><Neq><FieldRef Name='State'/><Value Type='Integer'>0</Value></Neq></And><And><Neq><FieldRef Name='TemplateName'/>      <Value Type='Text'>TEAMCHANNEL#0</Value></Neq><Neq><FieldRef Name='TemplateName'/>      <Value Type='Text'>TEAMCHANNEL#1</Value></Neq></And></And></Where><OrderBy><FieldRef Name='Title' Ascending='true' /></OrderBy></Query><ViewFields><FieldRef Name=\"AllowGuestUserSignIn\"/><FieldRef Name=\"ChannelSitesCount\"/><FieldRef Name=\"ChannelType\"/><FieldRef Name=\"CreatedBy\"/><FieldRef Name=\"CreatedByEmail\"/><FieldRef Name=\"DeletedBy\"/><FieldRef Name=\"ExternalSharing\"/><FieldRef Name=\"FileViewedOrEdited\"/><FieldRef Name=\"GroupId\"/><FieldRef Name=\"HubSiteId\"/><FieldRef Name=\"IBSegmentsGuids\"/><FieldRef Name=\"LastActivityOn\"/><FieldRef Name=\"NumOfFiles\"/><FieldRef Name=\"PagesVisited\"/><FieldRef Name=\"PageViews\"/><FieldRef Name=\"RelatedGroupId\"/><FieldRef Name=\"SensitivityLabel\"/><FieldRef Name=\"SiteCreationSource\"/><FieldRef Name=\"SiteFlags\"/><FieldRef Name=\"SiteId\"/><FieldRef Name=\"SiteOwnerEmail\"/><FieldRef Name=\"SiteOwnerName\"/><FieldRef Name=\"SiteUrl\"/><FieldRef Name=\"StorageQuota\"/><FieldRef Name=\"StorageUsed\"/><FieldRef Name=\"StorageUsedPercentage\"/><FieldRef Name=\"TemplateName\"/><FieldRef Name=\"TimeCreated\"/><FieldRef Name=\"TimeDeleted\"/><FieldRef Name=\"Title\"/></ViewFields><RowLimit Paged=\"TRUE\">3000</RowLimit></View>";
            } 
        }
    }
    public class O365ActiveSites
    {
        public O365ActiveSites() { }
        public Parameters parameters { get; set; }
    }

    public class SiteGroup
    {
        public string ID { get; set; }
        public string PermMask { get; set; }
        public string FSObjType { get; set; }
        public string UniqueId { get; set; }
        public string ContentTypeId { get; set; }
        public string FileRef { get; set; } 
        public string CreatedBy { get; set; }
        public string CreatedByEmail { get; set; }
        public string DeletedBy { get; set; }
        public string ExternalSharing { get; set; }
        public string FileViewedOrEdited { get; set; } 
        public string RelatedGroupId { get; set; } 
        public string SiteId { get; set; }
        public string SiteOwnerEmail { get; set; }
        public string SiteOwnerName { get; set; }
        public string SiteUrl { get; set; }
        public string StorageQuota { get; set; } 
        public string Title { get; set; }
        public string State { get; set; }
        public string GroupId { get; set; }  
    }

    public class UserGroupRoot
    {
        public object Members { get; set; }
        public int TotalMembersCount { get; set; }
        public List<Owner> Owners { get; set; }
        public int TotalOwnersCount { get; set; }
        public object NextPage { get; set; }
        public ActionResultModel ActionResultModel { get; set; }
    }
    public class ActionResultModel
    {
        public object GroupId { get; set; }
        public int Status { get; set; }
        public object Code { get; set; }
        public object Message { get; set; }
        public object JobId { get; set; }
        public object FailedMembers { get; set; }
        public object EditExchangeSettingsUrl { get; set; }
    }

    public class Owner
    {
        public string ObjectId { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public string UserPrincipalName { get; set; }
        public string MemberType { get; set; }
        public bool isGuestUserTypeMember { get; set; }
        public bool hasExchangeMailbox { get; set; }
        public bool HasTeams { get; set; }
    }
}
