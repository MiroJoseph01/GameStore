namespace GameStore.DAL.Entities.Interfaces
{
    public interface ISoftDelete
    {
        bool IsRemoved { get; set; }
    }
}
