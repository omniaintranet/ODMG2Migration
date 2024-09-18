using Models;
using Models.Moving;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ODMAPIServices : BaseService
    {
        public ConnectionDetail SiteInfo { get; set; }
        public ODMAPIServices(ConnectionDetail siteInfo)
        {
            SiteInfo = siteInfo;
        }

        public Root FilterPublished(PublishedQuery query)
        {
            /*
             {
              "webUrl": "https://ivlse.sharepoint.com/sites/processadmin-stodja",
              "pagingInfo": "",
              "rowPerPage": "5000",
              "sortBy": "ODMPublished",
              "sortAsc": true,
              "viewFields": [
                "Title"
              ], 
              "sqlIds": [ 
              ]
            }
             */
            try
            { 
                string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "/api/documents/filterpublished?omniaapp=false");
                Root ret = CreatePostRequestWithHeaders<Root>(api, query, SiteInfo.NetworkHeaders);
                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public Root GetDocumentFields(dynamic arg)
        { 
            try
            {
                string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "/api/documents/getdocumentfields?omniaapp=false");
                Root ret = CreatePostRequestWithHeaders<Root>(api, arg, SiteInfo.NetworkHeaders);
                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public Root GetAllDocumentTemplates()
        {
            try
            {
                string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "/api/documenttemplates/getall");
                Root ret = CreateGetRequestWithHeader<Root>(api, SiteInfo.NetworkHeaders);
                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public Root GetPublishedbyGraph(PublishedQuery query )
        {
            /*
             {
              "webUrl": "https://ivlse.sharepoint.com/sites/processadmin-stodja",
              "pagingInfo": "",
              "rowPerPage": "5000",
              "sortBy": "ODMPublished",
              "sortAsc": true,
              "viewFields": [
                "Title"
              ], 
              "sqlIds": [ 
              ]
            }
             */
            try
            {
                //https://b3d13ef75ff84496a28381a742080aef-ivlse.omniacloud.net/api/documents/getpublishedbygraph?omniaapp=false
                string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "/api/documents/getpublishedbygraph?omniaapp=false");
                Root ret = CreatePostRequestWithHeaders<Root>(api, query, SiteInfo.NetworkHeaders);
                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public Root SetDocumentStatus(ODMPublishedDocument odmDocument, string targetSite)
        {
            try
            {
                //https://b3d13ef75ff84496a28381a742080aef-ci.omniatestcloud.net/api/publish/movetotargetsite/setdocumentstatus?omniaapp=false&sqlDocumentId=d904709e-996a-4a11-86df-39ce4b54d5d1&odmDocumentId=46&currentSiteUrl=https:%2F%2Fomniatestcloud.sharepoint.com%2Fsites%2Fjk-ci-odm&targetSiteUrl=https:%2F%2Fomniatestcloud.sharepoint.com%2Fsites%2Fthuy3
                string api = string.Format("/api/publish/movetotargetsite/setdocumentstatus?omniaapp=false&sqlDocumentId={0}&odmDocumentId={1}&currentSiteUrl={2}&targetSiteUrl={3}",
                    odmDocument.sqlId,odmDocument.documentId,odmDocument.webUrl, targetSite);
                api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + api);
                Root ret = CreatePostRequestWithHeaders<Root>(api, null, SiteInfo.NetworkHeaders);
                //{"data":1,"success":true,"errorMessage":null,"responseCode":0}
                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public Root MovetoTargetSite(MovingDocument movingDoc)
        {
            /*MovingDocument movingDoc = new MovingDocument()
            {
                documentId = odmDocument.documentId,
                sqlId=odmDocument.sqlId,
                targetSiteUrl= targetSite,
                edition=odmDocument.edition,
                sourceSiteUrl = odmDocument.webUrl,
                replaceUser=""
            };*/
            try
            {
                //api/publish/movetotargetsite?spUrl=https://omniatestcloud.sharepoint.com/sites/jk-ci-odm&omniaapp=false
                string api = string.Format("/api/publish/movetotargetsite?spUrl={0}&omniaapp=false", movingDoc.sourceSiteUrl);
                api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + api);


                Root ret = CreatePostRequestWithHeaders<Root>(api, movingDoc, SiteInfo.NetworkHeaders);
                //{"success":true,"errorMessage":null,"responseCode":0}
                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

    }
}
