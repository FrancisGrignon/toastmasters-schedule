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

        public virtual TEntity Get(int id)
        {
            return Context.Set<TEntity>().Where(p => p.Active && id == p.Id).SingleOrDefault();
        }

        public virtual Task<TEntity> GetAsync(int id)
        {
            return Context.Set<TEntity>().Where(p => p.Active && id == p.Id).SingleOrDefaultAsync();
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return Context.Set<TEntity>().Where(p => p.Active);
        }

        public virtual async Task<TEntity[]> GetAllAsync()
        {
            return await Context.Set<TEntity>().Where(p => p.Active).ToArrayAsync();
        }

        public virtual IEnumerable<TEntity> GetAll<TOrderKey>(Expression<Func<TEntity, TOrderKey>> orderBy)
        {
            return Context.Set<TEntity>().Where(p => p.Active).OrderBy(orderBy).ToArray();
        }

        public virtual Task<TEntity[]> GetAllAsync<TOrderKey>(Expression<Func<TEntity, TOrderKey>> orderBy)
        {
            return Context.Set<TEntity>().Where(p => p.Active).OrderBy(orderBy).ToArrayAsync();
        }

        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate);
        }

        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, string includeProperties = null)
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

        public virtual TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
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
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Remove(entity);
            }
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
            return Context.Set<TEntity>().Where(p => p.Active && id == p.Id).Any();
        }

        public Task<bool> ExistsAsync(int id)
        {
            return Context.Set<TEntity>().Where(p => p.Active && id == p.Id).AnyAsync();
        }
    }
}