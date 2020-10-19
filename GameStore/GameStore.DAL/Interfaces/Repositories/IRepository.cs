using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        IEnumerable<TEntity> Filter(
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int skip = 0,
            int take = 0);
    }
}
