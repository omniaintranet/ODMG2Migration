using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public enum SiteObject
    {
        Source = 0,
        Destination = 1
    }
    public class SenderInfo
    {
        public SiteObject Sender { get; set; }
        public Object Value { get; set; }
    }
}
