using System;
using System.Collections.Generic;
using System.Text;

namespace GameStroe.Mobile.Models
{
    public class Order
    {
        public string OrderId { get; set; }

        public string CustomerId { get; set; }

        public IList<OrderDetail> OrderDetails { get; set; }

        public string OrderStatus { get; set; }

        public decimal Total { get; set; }

        public string OrderDate { get; set; }

        public List<Shipper> ShipOptions { get; set; }

        public string ShipVia { get; set; }

        public string ShipName { get; set; }

        public string ShipAddress { get; set; }

        public string ShipCity { get; set; }

        public string ShipRegion { get; set; }

        public string ShipPostalCode { get; set; }

        public string ShipCountry { get; set; }
    }
}
