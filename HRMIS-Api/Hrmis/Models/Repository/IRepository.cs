using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Hrmis.Models.Repository
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll(Func<T, bool> predicate = null);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null);
        T Get(Expression<Func<T, bool>> predicate);

        Task<T> GetAsync(Expression<Func<T, bool>> predicate);

        int GetCount(Expression<Func<T, bool>> predicate);

        Task<int> GetCountAsync(Expression<Func<T, bool>> predicate);

        T GetSingle(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "");
        IQueryable<T> GetMany(Expression<Func<T, bool>> where);
        void Add(T entity);
        void Update(T entity);
        void Update(object id, T entity);
        void Delete(T entity);

        void Delete(IEnumerable<T> entities);

        void Delete(object entity);
        void Save();

        Task<bool> SaveAsync();
    }
}
