using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /*
     
         {
  "spDocumentId": 22,
  "webUrl": "https://adventrixgroup.sharepoint.com/sites/migrated-document",
  "convertToPDF": true,
  "isRevisionPublishing": false,
  "comment": "",
  "isLimitedAccess": true,
  "limitedUsers": [
    {
      "uid": "55c28f5d-ae5a-4ed4-b55e-1a502d8deea1"//group
    },
    {
      "uid": "anna.karlsson@adventrixgroup.onmicrosoft.com"
    }
  ],
  "notifiedUsers": [
    {
      "uid": "admin@adventrixgroup.onmicrosoft.com"
    },
    {
      "uid": "anna.karlsson@adventrixgroup.onmicrosoft.com"
    }
  ],
  "isReadReceiptRequired": false
}
         */
    public class PublishingDocument
    {
        public int spDocumentId { get; set; }
        public string webUrl { get; set; }
        public bool convertToPDF { get; set; }
        public bool mergeTemplate { get; set; }
        public string convertToPDFwithFileTypes { get; set; }
        public bool isRevisionPublishing { get; set; }
        public string comment { get; set; }
        public bool isLimitedAccess { get; set; }
        public List<LimitedUser> limitedUsers { get; set; }
        public List<NotifiedUser> notifiedUsers { get; set; }
        public bool isReadReceiptRequired { get; set; }

        public bool attachDocumentTemplate { get; set; }
        public bool keepingVersionHistory { get; set; }
        public bool onlyMigrateLatestEdition { get; set; }
        public bool onlyDraft { get; set; }
        public bool onlyMigrateLatestAndDraft { get; set; }

        //version 6

        public int draftSPId { get; set; }
        public string sqlId { get; set; }

    }
    public class LimitedUser
    {
        public string uid { get; set; }

    }

    public class NotifiedUser
    {
        public string uid { get; set; }

    }

    public class DraftData
    {
        public bool isDeleted { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public string webUrl { get; set; }
        public string fileUrl { get; set; }
        public string fileExtension { get; set; }
        public string documentId { get; set; }
        public string contentLanguageName { get; set; }
        public int documentIdNumber { get; set; }
        public string documentTypeId { get; set; }
        public bool isConvertedPDF { get; set; }
        public int edition { get; set; }
        public int revision { get; set; }
        public DateTime published { get; set; }
        public DateTime reviewDate { get; set; }
        public DateTime reviewReminderDate { get; set; }
        public string contentTypeId { get; set; }
        public int status { get; set; }
        public int processingStatus { get; set; }
        public List<object> appendices { get; set; }
        public object signedDocuments { get; set; }
        public string sqlId { get; set; }
        public string templateId { get; set; }
        public int templateDocumentIdNumber { get; set; }
        public int templateDocumentEditionNumber { get; set; }
        public FieldValues fieldValues { get; set; }
        public DateTime modified { get; set; }
        public string modifiedBy { get; set; }
        public string uniqueId { get; set; }
    }
}
