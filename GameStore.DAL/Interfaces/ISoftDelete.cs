namespace GameStore.DAL.Interfaces
{
    public interface ISoftDelete
    {
        bool IsRemoved { get; set; }
    }
}
