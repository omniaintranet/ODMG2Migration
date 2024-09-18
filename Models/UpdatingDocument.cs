using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class UpdatingDocument
    {
        public string siteUrl { get; set; }
        public string documentUrl { get; set; }
        public List<SharePointField> sharePointFields { get; set; }
        public bool isUpdateDocId { get; set; }
        public string odmDocId { get; set; }
        public string sqlDocumentId { get; set; }
        public bool isUpdateDocStatus { get; set; }
        public MergeSettings mergeSettings { get; set; }
    }
    public class MergeSettings
    {
        public Document document { get; set; }
        public object newEditionTemplate { get; set; }
        public Template template { get; set; }
        public bool replacedByDocumentTemplateContent { get; set; }
        public bool removeDefaultStyle { get; set; }
    }
    public class Template
    {
        public string title { get; set; }
        public int type { get; set; }
        public string documentTemplateContentId { get; set; }
        public string fileName { get; set; }
        public string odmDocumentId { get; set; }
    }
    public class Document
    {
        public int id { get; set; }
        public string title { get; set; }
        public string webUrl { get; set; }
        public string fileUrl { get; set; }
        //public string fileExtension { get; set; }
        public string documentId { get; set; }
        public string templateId { get; set; }
        public string contentLanguageName { get; set; }
        //public int documentIdNumber { get; set; }
        public string documentTypeId { get; set; }
        //public bool isConvertedPDF { get; set; }
        //public int edition { get; set; }
        //public int revision { get; set; }
        //public DateTime published { get; set; }
        //public DateTime reviewDate { get; set; }
        //public DateTime reviewReminderDate { get; set; }
        //public string contentTypeId { get; set; }
        //public int status { get; set; }
        //public int processingStatus { get; set; }
        //public List<object> appendices { get; set; }
        //public object signedDocuments { get; set; }
        public string sqlId { get; set; }
        //public string templateId { get; set; }
        //public int templateDocumentIdNumber { get; set; }
        //public int templateDocumentEditionNumber { get; set; }
        //public FieldValues fieldValues { get; set; }
        //public DateTime modified { get; set; }
        //public string modifiedBy { get; set; }
        //public string uniqueId { get; set; }
        //public string openFileUrl { get; set; }
        //public bool allowAppendices { get; set; }
        //public string statusName { get; set; }
        //public string processingStatusName { get; set; }
        //public bool isMouseOver { get; set; }
        //public bool isSelected { get; set; }
    }

    public class FieldValues
    {
        public string Title { get; set; }
        public DateTime Modified { get; set; }
        public string Editor { get; set; }
        public string ODMDocumentTypeId { get; set; }
        public bool ODMIsLimitedAccess { get; set; }
        public object ODMLimitedUsers { get; set; }
    }

    public class DeletedDocument
    {
        public string webUrl { get; set; }
        public string odmDocId { get; set; }
        public string documentTypeId { get; set; }
        public string odmDocIdNumber { get; set; }

    } 

    public class Identity
    {
        public string id { get; set; }
        public int type { get; set; }
        public string uid { get; set; }
    } 

    public class SharePointField
    {
        public int type { get; set; }
        public object value { get; set; }
        public string id { get; set; }
        public string title { get; set; }
        public string internalName { get; set; }
        public bool required { get; set; }
        public bool hidden { get; set; }
        public bool? multiple { get; set; }
        public List<Identity> identities { get; set; }
        public string termSetId { get; set; }
        public List<string> termIds { get; set; }
        public string parentInternalName { get; set; }
        public object limitLevel { get; set; }
        public bool? isDateOnly { get; set; }

    }

    public class DocumentFields
    {
        public List<SharePointField> sharePointFields { get; set; }
        public string versionHash { get; set; }
        public bool sameVersion { get; set; }
    }
}
