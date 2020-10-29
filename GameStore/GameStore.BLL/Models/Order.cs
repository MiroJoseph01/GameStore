using System;
using System.Collections.Generic;

namespace GameStore.BLL.Models
{
    public class Order
    {
        public string OrderId { get; set; }

        public string CustomerId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public string Status { get; set; }

        public decimal Total { get; set; }

        public IList<OrderDetail> OrderDetails { get; set; }

        public int EmployeeID { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime RequierdDate { get; set; }

        public DateTime ShippedDate { get; set; }

        public string ShipVia { get; set; }

        public double Freight { get; set; }

        public string ShipName { get; set; }

        public string ShipAddress { get; set; }

        public string ShipCity { get; set; }

        public string ShipRegion { get; set; }

        public string ShipPostalCode { get; set; }

        public string ShipCountry { get; set; }
    }
}
