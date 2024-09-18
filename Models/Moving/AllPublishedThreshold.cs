using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Moving
{ 

    public class ODMBerordaorganisationer
    {
        public string Label { get; set; }
        public string TermGuid { get; set; }
    }

    public class ODMDocumentType
    {
        public string Label { get; set; }
        public string TermGuid { get; set; }
    }

    public class OmnUtfardandeOrganisation
    {
        public string Label { get; set; }
        public string TermGuid { get; set; }
    }

    /*public class Document
    {
        public List<object> relatedDocuments { get; set; }
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
        public string sqlId { get; set; }
        public string templateId { get; set; }
        public int templateDocumentIdNumber { get; set; }
        public int templateDocumentEditionNumber { get; set; }
        public FieldValues fieldValues { get; set; }
        public DateTime modified { get; set; }
        public string modifiedBy { get; set; }
    }*/

    public class AllPublishedThreshold
    {
        public string nextPageString { get; set; }
        public object previousPageString { get; set; }
        public List<ODMPublishedDocument> documents { get; set; }
        public bool useThresholdQuery { get; set; }
    }
}
