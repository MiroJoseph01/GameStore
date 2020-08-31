using System;
using System.Collections.Generic;
using GameStore.DAL.Entities.Interfaces;

namespace GameStore.DAL.Entities
{
    public class OrderStatus : ISoftDelete
    {
        public Guid OrderStatusId { get; set; }

        public string Status { get; set; }

        public bool IsRemoved { get; set; }

        public IList<Order> Orders { get; set; }
    }
}
