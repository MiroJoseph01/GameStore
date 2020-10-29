using Microsoft.AspNetCore.Mvc;

namespace GameStore.BLL.Payments
{
    public class PaymentInfo
    {
        public string Id { get; set; }

        public FileStreamResult FileStreamResult { get; set; }
    }
}
