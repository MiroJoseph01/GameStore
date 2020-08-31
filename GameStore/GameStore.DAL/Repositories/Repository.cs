using System;
using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.Entities.Interfaces;
using GameStore.DAL.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.DAL.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class, ISoftDelete
    {
        private readonly GameStoreContext _dbContext;

        public Repository(GameStoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool IsPresent(Guid id)
        {
            var entity = GetById(id);

            return !(entity is null) && !entity.IsRemoved;
        }

        public virtual void Create(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
        }

        public virtual void Delete(Guid id)
        {
            TEntity entity = GetById(id);

            entity.IsRemoved = true;
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return _dbContext.Set<TEntity>()
                .Where(x => x.IsRemoved == false)
                .ToList();
        }

        public virtual TEntity GetById(Guid id)
        {
            TEntity entity = _dbContext.Set<TEntity>().Find(id);

            TEntity result = entity.IsRemoved == false ? entity : null;

            return result;
        }

        public virtual void Update(Guid id, TEntity entity)
        {
            TEntity entityForUpdate = GetById(id);

            if (entityForUpdate != null)
            {
                _dbContext.Entry(entityForUpdate).State = EntityState.Detached;
            }

            _dbContext.Entry(entity).State = EntityState.Modified;
        }
    }
}
