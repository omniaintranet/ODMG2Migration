using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Root
    {
        public object data { get; set; }
        public bool success { get; set; }
        public object errorMessage { get; set; }
        public int responseCode { get; set; }
    }
}
