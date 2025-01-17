﻿namespace GameStore.BLL.Models
{
    public class OrderDetail
    {
        public string OrderDetailId { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductKey { get; set; }

        public decimal Price { get; set; }

        public short Quantity { get; set; }

        public float Discount { get; set; }

        public string OrderId { get; set; }
    }
}
