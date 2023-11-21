using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kassasysteem.Models
{
    public class MyOrderRequest
    {

        public int drawer_id { get; set; }
        public decimal total { get; set; }

        public int customer_number { get; set; }
    }
}
