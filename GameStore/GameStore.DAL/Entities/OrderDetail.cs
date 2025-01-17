﻿using GameStore.DAL.Entities.Interfaces;

namespace GameStore.DAL.Entities
{
    public class OrderDetail : ISoftDelete
    {
        public string OrderDetailId { get; set; }

        public string ProductId { get; set; }

        public decimal Price { get; set; }

        public short Quantity { get; set; }

        public float Discount { get; set; }

        public string OrderId { get; set; }

        public Order Order { get; set; }

        public bool IsRemoved { get; set; }
    }
}
