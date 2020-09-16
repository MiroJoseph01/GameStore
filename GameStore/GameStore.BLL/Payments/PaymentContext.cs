using System;
using GameStore.BLL.Interfaces;

namespace GameStore.BLL.Payments
{
    public class PaymentContext : IPaymentContext
    {
        private IPaymentStrategy _strategy;

        public void SetStrategy(IPaymentStrategy strategy)
        {
            _strategy = strategy;
        }

        public PaymentInfo ProcessPayment(Guid id)
        {
            return _strategy.Pay(id);
        }
    }
}
