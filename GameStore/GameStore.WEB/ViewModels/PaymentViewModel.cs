namespace GameStore.Web.ViewModels
{
    public class PaymentViewModel
    {
        public string PaymentName { get; set; }

        public string PaymentMethod { get; set; }

        public string OrderId { get; set; }

        public decimal Total { get; set; }

        public string LinkToImg { get; set; }
    }
}
