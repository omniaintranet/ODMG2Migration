using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SP = Microsoft.SharePoint.Client;

namespace Models
{
    public class ODMLibs
    {
        public SP.List ODMPublished { get; set; }
        public SP.List ODMDrafts { get; set; }
        public SP.List ODMUnprocessed { get; set; }
        public SP.List MigratedLibrary { get; set; }
    }
    public class ConnectionDetail
    {        
        private const string SHAREPOINTDOMAIN = ".sharepoint.com";
        public string SourceUrl { get; set; }
        public ODMLibs ODMLibraries { get; set; }
        public List<SimpleObject> ListSites { get; set; }
        public bool BatExcecute { get; set; }
        public string SourceLibrary { get; set; }
        public string DestLibrary { get; set; }
        public string TermStoreId { get; set; }
        public string OmniaCookies { get; set; }
        public bool ThresholdMode { get; set; }
        public string OmniaODMSiteUrl { get; set; }
        public string OmniaAPI { get; set; }
        public string OmniaODMAPI { get; set; }
        public List<FieldInfo> SourceFields { get; set; }

        public SP.ClientContext SourceCtx { get; set; }
        public SP.ClientContext ODMSiteCtx { get; set; }

        public List<ListInfo> SourceLists { get; set; }
        public List<ListInfo> DestLists { get; set; }
        public List<MappingAccount> MappingAccounts { get; set; }
        public SP.List SourceList { get; set; }

        public string ShowMe()
        {
            return "SourceUrl:" + SourceUrl + Environment.NewLine
            + "SourceLibrary:" + SourceLibrary + Environment.NewLine
            + "DestLibrary:" + DestLibrary + Environment.NewLine
            + "TermStoreId:" + TermStoreId + Environment.NewLine
            + "OmniaCookies:" + OmniaCookies + Environment.NewLine
            + "OmniaODMSiteUrl:" + OmniaODMSiteUrl + Environment.NewLine
            + "OmniaAPI:" + OmniaAPI + Environment.NewLine
            + "OmniaODMAPI:" + OmniaODMAPI + Environment.NewLine;
        }

        public PublishingDocument PublishingSettings { get; set; }

        public bool IsSameDomain()
        {
            string siteUrl = this.SourceUrl.ToLower();
            int idxSource = siteUrl.LastIndexOf(SHAREPOINTDOMAIN);


            string destSite = this.OmniaODMSiteUrl.ToLower();
            int idxDest = destSite.LastIndexOf(SHAREPOINTDOMAIN);

            return siteUrl.Length > 0 && destSite.Length > 0 && idxSource > 0 && idxDest > 0
                && siteUrl.Substring(0, idxSource) == destSite.Substring(0, idxDest);
        }

        public List<HeaderData> NetworkHeaders
        {
            get
            {
                return new List<HeaderData>() { new HeaderData()
                {
                    Key="cookie",
                    Value=this.OmniaCookies
                } };
            }
        }
    }

    public class ListInfo
    {
        public string Title { get; set; }
        public Guid Id { get; set; }
        public string EntityTypeName { get; set; }
    }
}
