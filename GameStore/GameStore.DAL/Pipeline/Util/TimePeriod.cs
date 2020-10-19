using System.ComponentModel;

namespace GameStore.DAL.Pipeline.Util
{
    public enum TimePeriod
    {
        [Description("LastWeek")]
        LastWeek,
        [Description("LastMonths")]
        LastMonths,
        [Description("LastYear")]
        LastYear,
        [Description("LastTwoYears")]
        LastTwoYears,
        [Description("LastThreeYears")]
        LastThreeYears,
        [Description("AllTime")]
        AllTime,
    }
}
