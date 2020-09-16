using System;
using GameStore.BLL.Interfaces;

namespace GameStore.BLL.Payments.PaymentStrategies
{
    public class IBoxPaymentStrategy : IPaymentStrategy
    {
        private readonly IOrderService _orderService;

        public IBoxPaymentStrategy(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public PaymentInfo Pay(Guid paymentId)
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
