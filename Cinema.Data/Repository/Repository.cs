using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Data.Repository
{
    public abstract class Repository<T> : IDisposable, IRepository<T> where T : class
    {
        protected DbContext db;
        protected IDbSet<T> dbSet;

        public Repository(DbContext context)
        {
            db = context;
            dbSet = context.Set<T>();
        }

        public void AddOrUpdate(T entity)
        {
            dbSet.AddOrUpdate(entity);
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }
        
        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return dbSet.Where(predicate);
        }

        public T Get(int id)
        {
            return dbSet.Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return dbSet;
        }

        public void Save()
        {
            db.SaveChanges();
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
