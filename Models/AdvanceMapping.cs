using Models.DocumentTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class AdvanceMapping
    {
        public List<DocumentType> DocumentTypes { get; set; }
        public ODMMigration MigrationObject { get; set; }
        //public List<FieldInfo> SourceFieldsCollection { get; set; }
        public List<FieldInfo> DraftFieldsCollection { get; set; }
        public string MappingColumnsFilter { get; set; }

        public string DynamicDocumentTypeFieldName { get; set; }
        public string ODMContentLanguage { get; set; }
        public string FixedDocumentTypeName { get; set; }
        public bool UseFixedDocumentType { get; set; }
        public bool KeepingVersionHistory { get; set; }

        public string DocumentTypeTermsetId { get; set; }
    }
}
