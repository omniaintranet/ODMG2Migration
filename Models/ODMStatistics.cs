 
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
namespace Models
{
    public class ODMSite
    {
        public List ODMDrafts { get; set; }
        public List ODMPublished { get; set; }
        public List ODMUnprocessed { get; set; }
        public ClientContext ClientContext { get; set; }
        public List<Field> DraftFields { get; set; }
        public List<Field> PublishedFields { get; set; }
        public List<Field> UnprocessedFields { get; set; }
        public IDictionary<string, string> Fields{ get; set; }
        public string RelativeUrl { get; set; }
        public string SiteUrl { get; set; }

    } 

    public class ODMStatistics
    {
        public string SiteUrl { get; set; }
        public string SiteTitle { get; set; }

        public int Draft { get; set; }
        public int Published { get; set; }
        public int Archived { get; set; }
        public int PassforReview { get; set; }
        public int SentforComment { get; set; }
        public int SentforApproval { get; set; }


        public List<ODMDocument> DraftDocuments { get; set; }
        public List<ODMDocument> PublishedDocuments { get; set; }
        public List<ODMDocument> ArchivedDocuments { get; set; }
        public List<ODMDocument> PassforReviewDocuments { get; set; }
        public List<ODMDocument> SentforCommentDocuments { get; set; }
        public List<ODMDocument> SentforApprovalDocuments { get; set; }
    } 
    public class ODMSiteInfo
    {
        public string SiteURL { get; set; }
        public string Name { get; set; }

        public int Draft { get; set; }
        public int Published { get; set; }
        public int Archived { get; set; }
        public int Review { get; set; }
        public int Comment { get; set; }
        public int Approval { get; set; }

        public int LibPublished { get; set; }
        public int LibDraft { get; set; }
        public int LibArchived { get; set; }
    }
    public class ODMDocument
    {
        public const char split = '⌂';
        public string DocId { get; set; }
        public string Url { get; set; }
        public string SiteName { get; set; }
        public string SiteUrl
        {
            get
            {
                if (Url == null || Url.Length == 0)
                {
                    return "";
                }
                string tmp = Url.ToLower().Replace("/odm", split.ToString());
                return tmp.Split(split)[0];
            }
        }
        public IDictionary<string, object> AdditionalData { get; set; }
    }
    public class ODMDocumentSearchSimple
    {
        public ODMDocumentSearchSimple(string url, IDictionary<string, object> additionalData) {
            //DocId=docId;
            Url=url;
            AdditionalData=additionalData;
        }
        public const char split = '⌂';
        public const string ListItemID = "ListItemID";
        //public string DocId { get; set; }
        public string Url { get; set; }  
        private IDictionary<string, object> AdditionalData { get; set; }

        public DataTable GetAdditionalData()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Column");
            dt.Columns.Add("Value");

            foreach (var item in AdditionalData)
            {
                DataRow dr = dt.NewRow();
                dr["Column"] = item.Key;
                dr["Value"] = item.Value;

                dt.Rows.Add(dr);
            }

            return dt;
        }

        public string GetAdditionalDatabyName(string key)
        {
            if(AdditionalData.ContainsKey(key)) return AdditionalData[key] + "";
            return "";
        }

        public bool HasMatchedData(string value)
        {
            if(value.Contains(':'))
            foreach (var item in AdditionalData)
            {
                if(item.Value!=null && item.Value.ToString().ToLower().Contains(value)) return true;
                string combineSearch=item.Key+ ":" + item.Value;
                if(combineSearch.ToLower()==value) return true;
            }
            return false;
        }
         


        public string GetSPSite()
        {
            string tmp = Url.ToLower().Replace("/odm", split.ToString());
            var arr = tmp.Split(split);
            return Url.Substring(0,arr[0].Length);
        }
        public string GetListIdentityName()
        { 
            string siteUrl = GetSPSite();
            string tmp = (Url.Replace(siteUrl + "/", ""));
            var arr = tmp.Split('/');
            return arr[0];
        }
        public int GetListItemId()
        {
            foreach (var item in AdditionalData)
            {
                if(item.Key == ListItemID)
                {
                    return Convert.ToInt32( item.Value);
                }
            }
            return -1;
        }
        public string GetODMDocId()
        {
            string siteUrl = GetSPSite();
            string identityName = GetListIdentityName();
            siteUrl = siteUrl + "/" + identityName + "/";

            string tmp = (Url.Replace(siteUrl, ""));
            var arr = tmp.Split('/');
            return arr[0];
        }
        public string GetServerRelativeUrl()
        {
            string siteUrl = GetSPSite();
            siteUrl = siteUrl.Replace("sharepoint.com", split.ToString());
            var arr = siteUrl.Split(split);
            return arr[1];
        }
    }
}