using System.ComponentModel;

namespace GameStore.DAL.Pipeline.Util
{
    public enum OrderOption
    {
        [Description("New")]
        New,
        [Description("MostPopular")]
        MostPopular,
        [Description("MostCommented")]
        MostCommented,
        [Description("PriceAsc")]
        PriceAsc,
        [Description("PriceDesc")]
        PriceDesc,
    }
}
