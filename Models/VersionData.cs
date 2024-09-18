using Models.DocumentTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class VersionData
    {
        public string CheckinComment { get; set; }
        public string VersionLabel { get; set; }
        public byte[] Binary { get; set; }
        public Stream FileStream { get; set; }
        public List<FieldInfoValue> FieldsValues { get; set; }
        public string FileRef { get; set; } 

        public string Boundary { get; set; }

        public int DraftDocumentId { get; set; }

        public DocumentType CurrentDocumentType { get; set; }

        public string CurrentDocumentId { get; set; }

        public bool IsLatestVersion { get; set; }
        public string OriginalListItemId { get; set; }

        public string DocumentTypeName { get; set; }
        public bool IsFixedDocumentType { get; set; }
    }
}
