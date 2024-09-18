using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class MigrationReport
    {
        public string SPListItemId { get; set; }
        public string OriginalUrl { get; set; }
        public string ODMDocumentId { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSucceed { get; set; }
    }
}
