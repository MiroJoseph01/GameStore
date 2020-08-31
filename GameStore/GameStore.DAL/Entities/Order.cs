using System;
using System.Collections.Generic;
using GameStore.DAL.Entities.Interfaces;

namespace GameStore.DAL.Entities
{
    public class Order : ISoftDelete
    {
        public Guid OrderId { get; set; }

        public Guid CustomerId { get; set; }

        public string Status { get; set; }

        public bool IsRemoved { get; set; }

        public virtual IList<OrderDetail> OrderDetails { get; set; }

        public OrderStatus OrderStatus { get; set; }
    }
}
