using System;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.BLL.Payments
{
    public class PaymentInfo
    {
        public Guid Id { get; set; }

        public FileStreamResult FileStreamResult { get; set; }
    }
}
