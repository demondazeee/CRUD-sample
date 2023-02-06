using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using test_crud.DBContext;
using test_crud.Entities;

namespace test_crud.Services
{
    public class TasksRepository : RepositoryBase<Tasks>, ITasksRepository
    {
        public TasksRepository(DB context) : base(context)
        {
        }
        public async Task<(IEnumerable<Tasks>, PageMetadata)> GetAllTasks(string? name, string? search, int pageNumber, int pageSize, Guid userId, TasksStatus? role)
        {
            var collection = context.Tasks as IQueryable<Tasks>;

            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection = collection.Where(v => v.Name== name);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                collection = collection.Where(v => v.Name.Contains(search));
            }

            var totalItemCount = await collection.Where(v => v.OwnerId == userId).CountAsync();

            var pageMetadata = new PageMetadata(totalItemCount, pageSize, pageNumber);

            var results = await collection
                .Where(v => v.TaskStatus == role)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (results, pageMetadata);
        }
    }
}
