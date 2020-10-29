using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.Services;

namespace GameStore.BLL.Payments.PaymentStrategies
{
    public class CardPaymentStrategy : IPaymentStrategy
    {
        private readonly IOrderService _orderService;

        public CardPaymentStrategy(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public PaymentInfo Pay(string paymentId)
        {
            var paymentInfo = new PaymentInfo()
            {
                Id = paymentId,
            };

            _orderService.UpdateStatusOfOrder(paymentId, OrderStatuses.Paid);

            return paymentInfo;
        }
    }
}
