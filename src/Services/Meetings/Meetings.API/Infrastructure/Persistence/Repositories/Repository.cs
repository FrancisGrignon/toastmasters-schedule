namespace Meetings.API.Infrastructure.Persistence.Repositories
{
    using Core.Repositories;
    using Meetings.Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public class Repository<TEntity, TContext> : IRepository<TEntity> where TEntity : class, IEntity where TContext : DbContext
    {
        protected readonly TContext Context;

        public Repository(TContext context)
        {
            Context = context;
        }

        public TEntity Get(int id)
        {
            return Context.Set<TEntity>().Find(id);
        }

        public Task<TEntity> GetAsync(int id)
        {
            return Context.Set<TEntity>().FindAsync(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Context.Set<TEntity>().ToArray();
        }

        public async Task<TEntity[]> GetAllAsync()
        {
            return await Context.Set<TEntity>().ToArrayAsync();
        }

        public IEnumerable<TEntity> GetAll<TOrderKey>(Expression<Func<TEntity, TOrderKey>> orderBy)
        {
            return Context.Set<TEntity>().OrderBy(orderBy).ToArray();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate);
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, string includeProperties = null)
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (false == string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (null == orderBy)
            {
                return query.ToArray();
            }

            return orderBy(query).ToArray();
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().SingleOrDefault(predicate);
        }

        public virtual void Add(TEntity entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.Active = true;

            Context.Set<TEntity>().Add(entity);
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            var buffer = entities.ToList();

            buffer.ForEach(entity =>
            {
                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.Active = true;
            });

            Context.Set<TEntity>().AddRange(buffer);
        }

        public virtual void Remove(TEntity entity)
        {
            entity.Active = false;
            entity.UpdatedAt = DateTime.UtcNow;

           // Context.Set<TEntity>().Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Remove(entity);
            }

            //Context.Set<TEntity>().RemoveRange(buffer);
        }

        public void Update(TEntity entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;

            Context.Entry(entity).State = EntityState.Modified;
        }

        public int Complete()
        {
            return Context.SaveChanges();
        }

        public Task<int> CompleteAsync()
        {
            return Context.SaveChangesAsync();
        }

        public bool Exists(int id)
        {
            var entity = Get(id);
            
            if (null == entity)
            {
                return false;
            }

            return true;
        }
    }
}