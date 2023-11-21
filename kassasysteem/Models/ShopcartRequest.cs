using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kassasysteem.Models
{
    public class ShopcartRequest
    {
        public int barcode { get; set; }
        public int quantity { get; set; }
        public int drawer_id { get; set; }
    }
}
