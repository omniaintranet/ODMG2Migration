using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DraftObjectResult
{
    public class DraftObjectResult
    {
        public string NextLinkUrl { get; set; }

        public List<ListItem> ListItems { get; set; }
    }

    public partial class ListItem
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string WebUrl { get; set; }
        public string FileUrl { get; set; }
        public string FileExtension { get; set; }
        public string DocumentId { get; set; }
        public string ContentLanguageName { get; set; }
        public long DocumentIdNumber { get; set; }
        public object DocumentTypeId { get; set; }
        public bool IsConvertedPdf { get; set; }
        public long Edition { get; set; }
        public long Revision { get; set; }
        //public DateTimeOffset Published { get; set; }
        //public DateTimeOffset ReviewDate { get; set; }
        //public DateTimeOffset ReviewReminderDate { get; set; }
        public string ContentTypeId { get; set; }
        public long Status { get; set; }
        public long ProcessingStatus { get; set; }
        //public object[] Appendices { get; set; }
        public Guid SqlId { get; set; }
        public string TemplateId { get; set; }
        public long TemplateDocumentIdNumber { get; set; }
        public long TemplateDocumentEditionNumber { get; set; }
        //public FieldValues FieldValues { get; set; }
        public string Modified { get; set; }
        public string ModifiedBy { get; set; }
    }

    public partial class FieldValues
    {
        [JsonProperty("@odata.etag")]
        public string OdataEtag { get; set; }

        [JsonProperty("ContentTypeId")]
        public ContentTypeId ContentTypeId { get; set; }

        [JsonProperty("FileLeafRef")]
        public string FileLeafRef { get; set; }

        [JsonProperty("id")] 
        public string Id { get; set; }

        [JsonProperty("ContentType")]
        public string ContentType { get; set; }

        [JsonProperty("Modified")]
        public DateTimeOffset Modified { get; set; }

        [JsonProperty("Editor")]
        public string Editor { get; set; }
    }

    public partial class ContentTypeId
    {
        [JsonProperty("StringValue")]
        public string StringValue { get; set; }
    }
}
