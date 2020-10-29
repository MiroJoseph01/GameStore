using System.Collections.Generic;
using System.Linq;
using GameStore.DAL.Entities.Interfaces;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.DAL.Repositories.Sql
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class, ISoftDelete
    {
        private readonly GameStoreContext _dbContext;
        private readonly IEntityStateLogger<TEntity> _entityStateLogger;

        public Repository(GameStoreContext dbContext, IEntityStateLogger<TEntity> entityStateLogger)
        {
            _dbContext = dbContext;
            _entityStateLogger = entityStateLogger;
        }

        public bool IsPresent(string id)
        {
            var entity = GetById(id);

            return !(entity is null) && !entity.IsRemoved;
        }

        public virtual void Create(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);

            _entityStateLogger.LogInsert(entity);
        }

        public virtual void Delete(string id)
        {
            TEntity entity = GetById(id);

            entity.IsRemoved = true;

            _entityStateLogger.LogDelete(entity);
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return _dbContext.Set<TEntity>()
                .Where(x => x.IsRemoved == false)
                .ToList();
        }

        public virtual TEntity GetById(string id)
        {
            TEntity entity = _dbContext.Set<TEntity>().Find(id);

            if (entity is null || entity.IsRemoved)
            {
                return null;
            }

            return entity;
        }

        public virtual void Update(string id, TEntity entity)
        {
            TEntity entityForUpdate = GetById(id);

            if (entityForUpdate != null)
            {
                _dbContext.Entry(entityForUpdate).State = EntityState.Detached;
            }

            _dbContext.Entry(entity).State = EntityState.Modified;

            _entityStateLogger.LogUpdate(entityForUpdate, entity);
        }
    }
}
