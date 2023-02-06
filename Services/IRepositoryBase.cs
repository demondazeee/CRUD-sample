using System.Linq.Expressions;

namespace test_crud.Services
{
    public interface IRepositoryBase<T>
    {
        Task Create(T entity);

        Task Delete(T entity);

        Task<IEnumerable<T>> GetAll();

        Task<IEnumerable<T>> GetAllByExpresion(Expression<Func<T, bool>> expression);

        Task<T?> GetValueByExpression(Expression<Func<T, bool>> expression);

        Task SaveChanges();
    }
}
