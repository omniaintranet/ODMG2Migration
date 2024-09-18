using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.UnProcessedDocument
{ 
    public class DraftDocument
    {
        public string odmDocumentId { get; set; }
        public string webUrl { get; set; }
        public int editionToCreateDraft { get; set; }
        public int latestEdition { get; set; }
    }
}
