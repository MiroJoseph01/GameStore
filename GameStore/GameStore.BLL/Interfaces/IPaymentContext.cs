using GameStore.BLL.Payments;

namespace GameStore.BLL.Interfaces
{
    public interface IPaymentContext
    {
        void SetStrategy(IPaymentStrategy strategy);

        PaymentInfo ProcessPayment(string id);
    }
}
