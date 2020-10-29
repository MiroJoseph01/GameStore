using System;
using System.Collections.Generic;
using GameStore.DAL.Entities.Interfaces;

namespace GameStore.DAL.Entities
{
    public class Order : ISoftDelete
    {
        public string OrderId { get; set; }

        public string CustomerId { get; set; }

        public string Status { get; set; }

        public bool IsRemoved { get; set; }

        public virtual IList<OrderDetail> OrderDetails { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public int EmployeeID { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime RequiredDate { get; set; }

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
