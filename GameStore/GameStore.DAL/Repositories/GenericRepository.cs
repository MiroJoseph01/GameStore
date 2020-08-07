using System;
using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.Interfaces;

namespace GameStore.DAL.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : class, ISoftDelete
    {
        private readonly GameStoreContext _dbContext;

        public GenericRepository(GameStoreContext dbContext)
        {
            _dbContext = dbContext;
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
            return _dbContext.Set<TEntity>().Where(x => x.IsRemoved == false).ToList();
        }

        public virtual TEntity GetById(Guid id)
        {
            TEntity entity = _dbContext.Set<TEntity>().Find(id);

            TEntity result = entity.IsRemoved == false ? entity : null;

            return result;
        }

        public virtual void Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
        }
    }
}
