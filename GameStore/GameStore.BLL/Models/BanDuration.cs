namespace GameStore.BLL.Models
{
    public enum BanDuration
    {
        NoBan = 0,
        Hour = 1,
        Day = 24,
        Week = 168,
        Month = 720,
        Permanent = -1,
    }
}
