using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.Services;

namespace GameStore.BLL.Payments.PaymentStrategies
{
    public class IBoxPaymentStrategy : IPaymentStrategy
    {
        private readonly IOrderService _orderService;

        public IBoxPaymentStrategy(IOrderService orderService)
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
