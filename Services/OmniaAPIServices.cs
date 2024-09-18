using Models;
using Models.DocumentTypes;
using Models.DraftObjectResult;
using Models.EnterpriseProperties; 
using Models.UnProcessedDocument;
using Newtonsoft.Json; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class OmniaAPIServices : BaseService
    {
        public ConnectionDetail SiteInfo { get; set; }
        public OmniaAPIServices(ConnectionDetail siteInfo)
        {
            SiteInfo = siteInfo;
        }

        public List<EnterpriseProperty> GetPublishedbyGraph()
        {
            try
            {
                string api = EnsureCorrectAPI(SiteInfo.OmniaAPI + "/api/documents/getpublishedbygraph?omniaapp=false");
                Root ret = CreateGetRequestWithHeader<Root>(api, SiteInfo.NetworkHeaders);
                return JsonConvert.DeserializeObject<List<EnterpriseProperty>>(ret.data.ToString());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        
        public List<EnterpriseProperty> GetEnterpriseProperties()
        {
            try
            {
                string api = EnsureCorrectAPI(SiteInfo.OmniaAPI + "/api/enterpriseproperties?includeTypeData=true");
                Root ret = CreateGetRequestWithHeader<Root>(api, SiteInfo.NetworkHeaders);
                return JsonConvert.DeserializeObject<List<EnterpriseProperty>>(ret.data.ToString());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public DocumentStatus GetDocumentStatus(string spDocumentId, string documentTypeId, string odmDocId="")
        {
            try
            {
                string api = string.Format(SiteInfo.OmniaODMAPI + 
                    "/api/documents/updatedocumentfieldsstatus?omniaapp=false&siteUrl={0}&spDocumentId={1}&odmDocId={2}&documentTypeId={3}",
                    this.SiteInfo.OmniaODMSiteUrl, spDocumentId, odmDocId, documentTypeId);
                api = EnsureCorrectAPI(api);
                Root ret = CreateGetRequestWithHeader<Root>(api, SiteInfo.NetworkHeaders);
                return JsonConvert.DeserializeObject<DocumentStatus>(ret.data.ToString());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public Root PublishDocument(PublishingDocument document)
        {
            try
            {
                string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "/api/publish/withoutapproval?omniaapp=false");
                Root ret = CreatePostRequestWithHeaders<Root>(api, document, SiteInfo.NetworkHeaders);
                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
                
        public Root CreateDraftDocumentFromNewUploadedFile(DraftDocument draftDocument,VersionData versionData)
        {
            try
            {
                //https://b3d13ef75ff84496a28381a742080aef-vardforbundet.omniacloud.net/api/documents/createdraftdocumentfromnewuploadedfile?omniaapp=false&webUrl=https:%2F%2Fvardforbundet.sharepoint.com%2Fsites%2Favtal-exempel-1&odmDocId=11129&edition=1
                string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "/api/documents/createdraftdocumentfromnewuploadedfile?omniaapp=false&webUrl={0}&odmDocId={1}&edition={2}");
                api=string.Format(api,draftDocument.webUrl,draftDocument.odmDocumentId,draftDocument.editionToCreateDraft);
                Root ret = UploadFileWithHeaders<Root>(api, versionData, SiteInfo.NetworkHeaders);
                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public Root UploadDraftDocument(string odmUrl, VersionData versionData)
        {
            try
            {
                /*
                 {
                  "odmDocumentId": "Doc-1306",
                  "webUrl": "https://arrivagroup.sharepoint.com/sites/dk-sls-kon-dok",
                  "editionToCreateDraft": 1,
                  "latestEdition": 1
                }
                 */
                string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "/api/uploaddocuments/draftdocument?omniaapp=false&spUrl="+ odmUrl);
                Root ret = UploadFileWithHeaders<Root>(api, versionData, SiteInfo.NetworkHeaders);
                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public Root CreateDraftDocumentFromUnprocessedEdition(DraftDocument document)
        {
            try
            {
                /*
                 {
                  "odmDocumentId": "Doc-1306",
                  "webUrl": "https://arrivagroup.sharepoint.com/sites/dk-sls-kon-dok",
                  "editionToCreateDraft": 1,
                  "latestEdition": 1
                }
                 */
                string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "/api/documents/createdraftdocumentfromunprocessededition?omniaapp=false");
                Root ret = CreatePostRequestWithHeaders<Root>(api, document, SiteInfo.NetworkHeaders);
                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public Root DeleteDraftDocuments(dynamic arg)
        {
            try
            {
                /*
                 {"webUrl":"https://omniademosto.sharepoint.com/sites/HRAuthoringSite","spDocumentIds":[284],"sqlIds":["dce41acf-02f0-492c-bdd9-f7ac7328a9db"]}
                 */
                string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "/api/documents/deletedraftdocuments?omniaapp=false");
                Root ret = CreatePostRequestWithHeaders<Root>(api, arg, SiteInfo.NetworkHeaders);
                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public Root FilterDrafts(dynamic arg)
        {
            try
            {
                /*
                 {"viewFields":["Title","ODMDocId","Modified","Editor","ODMDocumentType","ODMReviewReminderDate","OmniaProcess","ODMDocumentTypeId","ODMIsLimitedAccess","ODMLimitedUsers"],"queryText":"52","rowPerPage":"20","currentPage":0,"pagingInfo":"","sortBy":"Modified","sortAsc":false,"webUrl":"https://omniademosto.sharepoint.com/sites/HRAuthoringSite","filters":[],"sqlIds":[],"spItemIds":[],"lastItemIdInThresholdQuery":0,"loadPreviousPage":false,"hasDocumentIdParam":false}
                 */
                string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "/api/documents/filterdrafts?omniaapp=false");
                Root ret = CreatePostRequestWithHeaders<Root>(api, arg, SiteInfo.NetworkHeaders);
                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public Root GetExistingDraftDocument(string odmSite, string docId)
        {
            try
            {
                //https://b3d13ef75ff84496a28381a742080aef-vardforbundet.omniacloud.net/api/documents/createdraftdocumentfromnewuploadedfile?omniaapp=false&webUrl=https:%2F%2Fvardforbundet.sharepoint.com%2Fsites%2Favtal-exempel-1&odmDocId=11129&edition=1
                //string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "/api/documents/getexistingdraftdocument?omniaapp=false&webUrl={0}&odmDocId={1}");
                string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "/api/documents/isexistingdraftdocument?omniaapp=false&webUrl={0}&odmDocId={1}");
                api = string.Format(api, odmSite, docId);
                Root ret = CreateGetRequestWithHeader<Root>(api, SiteInfo.NetworkHeaders);
                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public Root DeletePublishDocument(DeletedDocument document)
        {
            try
            {
                string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "/api/documents/deletepublishdocument?omniaapp=false&webUrl={0}&odmDocId={1}&documentTypeId={2}&odmDocIdNumber={3}");
                api = string.Format(api, document.webUrl, document.odmDocId, document.documentTypeId, document.odmDocIdNumber);
                Root ret = CreateGetRequestWithHeader<Root>(api, SiteInfo.NetworkHeaders);
                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public Root GetDocumentHistory(string docId, string webURL)
        {
            try
            {
                string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "/api/documenthistory/getdocumenthistory?omniaapp=false&odmDocId={0}&webUrl={1}");
                api = string.Format(api, docId, webURL);
                Root ret = CreateGetRequestWithHeader<Root>(api, SiteInfo.NetworkHeaders);
                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public Root GetDocumentStatistics(int statisticsType)
        {
            try
            { 
                string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "api/documentstatistics/statisticstype/count?statisticsType=" + statisticsType);
                Root ret = CreatePostRequestWithHeaders<Root>(api, "", SiteInfo.NetworkHeaders);
                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public Root GetDocumentStatisticsDetail(int statisticsType)
        {
            try
            {
                string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "api/documentstatistics/statisticstype?statisticsType=" + statisticsType);
                Root ret = CreatePostRequestWithHeaders<Root>(api, "", SiteInfo.NetworkHeaders);
                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public Root GetDocumentFields(DocumentInfoRequest document)
        {
            try
            {
                /*
                 documentUrl: "https://arrivagroup.sharepoint.com/sites/dk-sls-kon-dok/ODMPublished/Doc-1280/6_03_025_APB_Odense.pptx"
                internalNames: null
                siteUrl: "https://arrivagroup.sharepoint.com/sites/dk-sls-kon-dok"
                versionHash: ""
                 */
                //get document id
                string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "/api/documents/getdocumentfields?omniaapp=false");
                Root ret = CreatePostRequestWithHeaders<Root>(api, document, SiteInfo.NetworkHeaders);
                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public Root MergeWithDocumentTemplate(PublishingDocument document)
        {
            try
            {
                string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "/api/publish/withoutapproval?omniaapp=false");
                Root ret = CreatePostRequestWithHeaders<Root>(api, document, SiteInfo.NetworkHeaders);
                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public string UpdateDocumentProperties(UpdatingDocument document, out string errorMsg)
        {
            try
            { 
                string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "/api/documents/updatedocumentfields?omniaapp=false");
                Root ret = CreatePostRequestWithHeaders<Root>(api,document, SiteInfo.NetworkHeaders);
                if (ret.success && ret.data!=null)
                {
                    errorMsg = "";
                    return ret.data.ToString();
                }
                else
                {
                    errorMsg = ret.errorMessage+"";
                    return "";
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public DraftObjectResult GetDraftbyGraph(int listItemId)
        {
            try
            {
                DraftObject draftObject = new DraftObject()
                {
                    viewFields = new List<string>() {"Title",
                                                    "Modified",
                                                    "Editor",
                                                    "ODMDocId",
                                                    "ODMDocumentTypeId",
                                                    "ODMIsLimitedAccess",
                                                    "ODMLimitedUsers" },
                    pagingInfo = "",
                    rowPerPage = 10,
                    sortAsc = false,
                    sortBy = "Modified",
                    webUrl = SiteInfo.OmniaODMSiteUrl,
                    spItemIds = new List<int>() { listItemId }
                };

                string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "/api/documents/getdraftsbygraph");
                Root ret = CreatePostRequestWithHeaders<Root>(api, draftObject,  SiteInfo.NetworkHeaders);
                return JsonConvert.DeserializeObject<DraftObjectResult>(ret.data.ToString());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public List<PropertiesSet> GetODMPropertiesSet()
        {
            try
            {
                string api = EnsureCorrectAPI(SiteInfo.OmniaAPI + "/api/enterprisepropertysets");
                Root ret = CreateGetRequestWithHeader<Root>(api, SiteInfo.NetworkHeaders);
                return JsonConvert.DeserializeObject<List<PropertiesSet>>(ret.data.ToString());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public Root GetPublishedSites()
        {
            try
            { 
                string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "/api/documentstatistics/statisticstype?statisticsType=1");

                Root ret = CreatePostRequestWithHeaders<Root>(api,"", SiteInfo.NetworkHeaders);
                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public List<DocumentType> GetDocumentTypes(string documentTypeId = null)
        {
            try
            {
                documentTypeId = documentTypeId==null? this.getDocumentTypeId(): documentTypeId;
                string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "/api/documenttypes/children?parentId=" + documentTypeId);
                 
                Root ret = CreateGetRequestWithHeader<Root>(api,SiteInfo.NetworkHeaders);
                return JsonConvert.DeserializeObject<List<DocumentType>>(ret.data.ToString());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }


        private string getDocumentTypeId()
        {
            try
            {
                string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "/api/documenttypes/getdocumenttypetermsetid");
                Root ret = CreateGetRequestWithHeader<Root>(api, SiteInfo.NetworkHeaders);
                return ret.data as string;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }


    }
}
