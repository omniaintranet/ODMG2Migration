using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Moving
{
    public class PublishedQuery
    {
        public List<string> viewFields { get; set; }
        public string rowPerPage { get; set; }
        public int currentPage { get; set; }
        public int lastItemIdInThresholdQuery { get; set; }
        public string pagingInfo { get; set; }
        public string sortBy { get; set; }
        public bool sortAsc { get; set; }
        public bool loadPreviousPage { get; set; }
        public bool hasDocumentIdParam { get; set; }
        public string webUrl { get; set; }
        public List<object> sqlIds { get; set; }
        public List<object> filters { get; set; }
        public List<object> spItemIds { get; set; }
    }
}
