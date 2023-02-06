using test_crud.Entities;

namespace test_crud.Services
{
    public interface ITasksRepository: IRepositoryBase<Tasks>
    {
        Task<(IEnumerable<Tasks>, PageMetadata)> GetAllTasks(string? name, string? search, int pageNumber, int pageSize, Guid userId, TasksStatus? role);
    }
}
