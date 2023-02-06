using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using test_crud.DBContext;

namespace test_crud.Services
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected DB context;

        protected RepositoryBase(DB context)
        {
           this.context = context;
        }
        public async Task Create(T entity)
        {
            await this.context.Set<T>().AddAsync(entity);

            await this.SaveChanges();
        }

        public async Task Delete(T entity)
        {
            this.context.Set<T>().Remove(entity);

            await this.SaveChanges();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await this.context.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllByExpresion(Expression<Func<T, bool>> expression)
        {
            return await this.context.Set<T>().Where(expression).ToListAsync();
        }

        public async Task<T?> GetValueByExpression(Expression<Func<T, bool>> expression)
        {
            return await this.context.Set<T>().Where(expression).FirstOrDefaultAsync();
        }

        public async Task SaveChanges()
        {
            await this.context.SaveChangesAsync();
        }
    }
}
