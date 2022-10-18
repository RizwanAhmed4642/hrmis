using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hrmis.Models.DbModel;
using Type = System.Type;

namespace Hrmis.Models.Repository
{
    public class GenericUnitOfWork : IDisposable
    {
        private readonly HR_System entities;

        public GenericUnitOfWork()
        {
            entities = new HR_System();
            entities.Configuration.ProxyCreationEnabled = false;
        }

       // public FmsEntities Context { get { return this.entities; } }

        public Dictionary<Type, object> repositories = new Dictionary<Type, object>();

        public IRepository<T> Repository<T>() where T : class
        {
            if (repositories.Keys.Contains(typeof(T)))
            {
                return repositories[typeof(T)] as IRepository<T>;
            }
            IRepository<T> repo = new GenericRepository<T>(entities);
            repositories.Add(typeof(T), repo); 
            return repo;
        }

        public void SaveChanges()
        {
            using (var dbContextTransaction = entities.Database.BeginTransaction())
            {
                try
                {
                    entities.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw ex;
                }
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            using (var dbContextTransaction = entities.Database.BeginTransaction())
            {
                try
                {
                    await entities.SaveChangesAsync();
                    dbContextTransaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw ex;
                }
            }
        }

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    entities.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
