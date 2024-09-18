using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DraftObject
    {
        public List<string> viewFields { get; set; }
        public int rowPerPage { get; set; }
        public string pagingInfo { get; set; }
        public string sortBy { get; set; }
        public bool sortAsc { get; set; }
        public string webUrl { get; set; }
        public List<int> spItemIds { get; set; }
    }
}
