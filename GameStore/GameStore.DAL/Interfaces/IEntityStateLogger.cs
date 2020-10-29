namespace GameStore.DAL.Interfaces
{
    public interface IEntityStateLogger<TEntity>
        where TEntity : class
    {
        void LogDelete(TEntity entity);

        void LogInsert(TEntity entity);

        void LogUpdate(TEntity oldEntity, TEntity newEntity);
    }
}
