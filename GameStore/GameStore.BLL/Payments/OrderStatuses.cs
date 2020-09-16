namespace GameStore.BLL.Payments
{
    public static class OrderStatuses
    {
        public static string Open
        {
            get
            {
                return "Open";
            }
        }

        public static string Paid
        {
            get
            {
                return "Paid";
            }
        }

        public static string NotPaid
        {
            get
            {
                return "NotPaid";
            }
        }
    }
}
