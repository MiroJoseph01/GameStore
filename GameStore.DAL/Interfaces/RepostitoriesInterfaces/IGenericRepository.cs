using System;
using System.Collections.Generic;

namespace GameStore.DAL.Interfaces
{
    public interface IGenericRepository<TEntity>
        where TEntity : ISoftDelete
    {
        IEnumerable<TEntity> GetAll();

        TEntity GetById(Guid id);

        void Create(TEntity entity);

        void Update(TEntity entity);

        void Delete(Guid id);
    }
}
