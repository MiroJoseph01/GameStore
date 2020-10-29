using System.Collections.Generic;
using GameStore.DAL.Entities.Interfaces;

namespace GameStore.DAL.Interfaces.Repositories
{
    public interface IRepository<TEntity>
        where TEntity : ISoftDelete
    {
        IEnumerable<TEntity> GetAll();

        bool IsPresent(string id);

        TEntity GetById(string id);

        void Create(TEntity entity);

        void Update(string id, TEntity entity);

        void Delete(string id);
    }
}
