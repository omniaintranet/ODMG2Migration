using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class MigratedItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Data { get; set; }
        //public DocumentProperties Properties { get; set; }
    }
}
