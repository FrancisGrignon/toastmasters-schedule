using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Meetings.API.Infrastructure.Core.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity Get(int id);

        Task<TEntity> GetAsync(int id);

        IEnumerable<TEntity> GetAll();

        Task<TEntity[]> GetAllAsync();

        IEnumerable<TEntity> GetAll<TOrderKey>(Expression<Func<TEntity, TOrderKey>> orderBy);

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, string includeProperties = null);

        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        void Add(TEntity entity);

        void AddRange(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);

        void RemoveRange(IEnumerable<TEntity> entities);

        void Update(TEntity entity);

        int Complete();

        Task<int> CompleteAsync();

        bool Exists(int id);
    }
}
