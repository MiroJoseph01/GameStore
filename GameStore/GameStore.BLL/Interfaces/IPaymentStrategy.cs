using GameStore.BLL.Payments;

namespace GameStore.BLL.Interfaces
{
    public interface IPaymentStrategy
    {
        PaymentInfo Pay(string paymentId);
    }
}
