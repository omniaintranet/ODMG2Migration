using Models.DocumentTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
   public class BatchUpdateItem
    {
        public VersionData CurrentVersion { get; set; }
        public DocumentType CurrentDT { get; set; }
        public string documentId { get; set; }
    }
}
