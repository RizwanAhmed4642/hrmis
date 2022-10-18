using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hrmis.Models.DbModel;

namespace Hrmis.Models.Repository
{
    class GenericRepository<T> : IRepository<T> where T : class
    {
        internal HR_System entities;
        internal DbSet<T> dbSet;

        public GenericRepository(HR_System fmsEntities)
        {
            entities = fmsEntities;
            dbSet = entities.Set<T>();
        }

        public virtual T GetSingle(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = includeProperties.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            if (orderBy != null)
            {
                return orderBy(query).FirstOrDefault();
            }
            return query.FirstOrDefault();
        }

        public IQueryable<T> GetAll(Func<T, bool> predicate = null)
        {
            if (predicate != null)
            {
                return dbSet.Where(predicate).AsQueryable();
            }

            return dbSet.AsQueryable();
        }


        public async Task<IEnumerable<T>>  GetAllAsync(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate != null)
            {
                return await dbSet.Where(predicate).ToListAsync();
            }

            return await dbSet.ToListAsync();
        }
        public T Get(Expression<Func<T, bool>> predicate)
        {
            return dbSet.FirstOrDefault(predicate);
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await dbSet.FirstOrDefaultAsync(predicate);
        }

        public virtual int GetCount(Expression<Func<T, bool>> predicate)
        {
            return entities.Set<T>().Count(predicate);
        }

        public virtual async Task<int> GetCountAsync(Expression<Func<T, bool>> predicate)
        {
            return await entities.Set<T>().CountAsync(predicate);
        }

        public virtual IQueryable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where).AsQueryable();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Update(object id, T entity)
        {
            //T entityToUpdate = dbSet.Find(id);

           // Update(entity);
        }

        public virtual void Update(T entity)
        {
            //dbSet.Attach(entity);
            entities.Entry(entity).State = EntityState.Modified;
            
        }

        public virtual void Delete(object id)
        {
            T entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(T entityToDelete)
        {
            if (entities.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
            
        }

        public virtual void Delete(IEnumerable<T> entitiesToDelete)
        {
            dbSet.RemoveRange(entitiesToDelete);
        }


        public virtual void Save()
        {
            entities.SaveChanges();
        }

        public virtual async Task<bool> SaveAsync()
        {
            try
            {
                await entities.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
