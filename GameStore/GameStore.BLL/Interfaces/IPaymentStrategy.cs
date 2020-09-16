using System;
using GameStore.BLL.Payments;

namespace GameStore.BLL.Interfaces
{
    public interface IPaymentStrategy
    {
        PaymentInfo Pay(Guid paymentId);
    }
}
