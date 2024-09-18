using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DocumentStatus
    {
        public string id { get; set; }
        public int spDocumentId { get; set; }
        public string spWebUrl { get; set; }
        public int status { get; set; }
        public int processingStatus { get; set; }
        public string documentId { get; set; }
        public int documentVersionType { get; set; }
        public string documentTypeId { get; set; }
        public object createdBy { get; set; }
        public object modifiedBy { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime modifiedAt { get; set; }
        public object deletedAt { get; set; }
    }
}
