using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Moving
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class ContentTypeId
    {
        public string StringValue { get; set; }
    }

    public class FieldValues
    {
        [JsonProperty("@odata.etag")]
        public string OdataEtag { get; set; }
        public ContentTypeId ContentTypeId { get; set; }
        public string FileLeafRef { get; set; }
        public string Title { get; set; }
        public string ODMDocId { get; set; }
        public string ODMDocIdNumber { get; set; }
        public string ODMEdition { get; set; }
        public string ODMRevision { get; set; }
        public string ODMContentLanguage { get; set; }
        public DateTime ODMPublished { get; set; }
        public string ODMDocumentTypeId { get; set; }
        public DateTime ODMApproved { get; set; }
        public string ODMApprovedBy { get; set; }
        public string ODMDocumentHistory { get; set; }
        public string ODMRelatedDocuments { get; set; }
        public bool ODMIsConvertedPDF { get; set; }
        public string id { get; set; }
        public string ContentType { get; set; }
        public DateTime Modified { get; set; }
        public string Editor { get; set; }
        public bool? ODMIsPreview { get; set; }
        public string ODMTemplateId { get; set; }
    }

    public class ODMPublishedDocument
    {
        public string title { get; set; }
        public string fileUrl { get; set; }
        public string webUrl { get; set; }
        public string documentId { get; set; }
        public int edition { get; set; }
        public string sqlId { get; set; }
        public int id { get; set; }
        public dynamic fieldValues { get; set; }
        public string documentTypeId { get; set; }
        public string documentIdNumber { get; set; }
        /*public string templateId { get; set; }
        public string templateDocumentIdNumber { get; set; }
        public string templateDocumentEditionNumber { get; set; }
        public FieldValues fieldValues { get; set; }
        public DateTime modified { get; set; }
        public string modifiedBy { get; set; }
        public string revision { get; set; }
        public DateTime published { get; set; }
        public DateTime reviewDate { get; set; }
        public DateTime reviewReminderDate { get; set; }
        public string contentTypeId { get; set; }
        public int status { get; set; }
        public int processingStatus { get; set; }
        public List<object> appendices { get; set; }
        public string contentLanguageName { get; set; }
        public int documentIdNumber { get; set; }
        public string documentTypeId { get; set; }
        public bool isConvertedPDF { get; set; }
        public string fileUrl { get; set; }
        public string fileExtension { get; set; }
        public List<object> relatedDocuments { get; set; }
        public int id { get; set; }
        */

        public bool? IsSelected { get; set; }
    }

    public class PublishedDocuments
    {
        public object nextLinkUrl { get; set; }
        public List<ODMPublishedDocument> listItems { get; set; }
    } 
}
