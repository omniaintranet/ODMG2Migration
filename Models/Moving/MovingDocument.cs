using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Moving
{
   public class MovingDocument
    {
        public string documentId { get; set; }
        public string sqlId { get; set; }
        public string targetSiteUrl { get; set; }
        public int edition { get; set; }
        public string sourceSiteUrl { get; set; }
        public ReplaceUser replaceUser { get; set; }
    }

    public class ReplaceUser
    {
        public string uid { get; set; }
    }
}
