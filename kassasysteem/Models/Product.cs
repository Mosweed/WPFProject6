using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kassasysteem.Models
{
    public class Product
    {
        public int id { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public int barcode { get; set; }
        public int stock { get; set; }
        public int price { get; set; }
    }
}
