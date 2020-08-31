using System;
using System.Collections.Generic;

namespace GameStore.BLL.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }

        public Guid CustomerId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public string Status { get; set; }

        public IList<OrderDetail> OrderDetails { get; set; }
    }
}
