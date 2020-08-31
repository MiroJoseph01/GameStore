using System;
using System.Collections.Generic;
using GameStore.DAL.Entities.Interfaces;

namespace GameStore.DAL.Interfaces.Repositories
{
    public interface IRepository<TEntity>
        where TEntity : ISoftDelete
    {
        IEnumerable<TEntity> GetAll();

        bool IsPresent(Guid id);

        TEntity GetById(Guid id);

        void Create(TEntity entity);

        void Update(Guid id, TEntity entity);

        void Delete(Guid id);
    }
}
