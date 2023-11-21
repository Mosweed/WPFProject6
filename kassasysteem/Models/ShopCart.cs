using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kassasysteem.Models
{
    public class ShopCart
    {

        public int Id { get; set; }

        public string name { get; set; }

        public string image { get; set; }

        public int quantity { get; set; }

        public int barcode { get; set; }

        public decimal price { get; set; }

        public decimal shopcartitem_price { get; set; }

        public int discount_percentage { get; set; }


    }
}
