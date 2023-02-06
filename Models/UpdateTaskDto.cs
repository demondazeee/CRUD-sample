using test_crud.Entities;

namespace test_crud.Models
{
    public class UpdateTaskDto 
    {
        public string? Name { get; set; } = string.Empty;

        public TasksStatus? TaskStatus { get; set; }
    }
}
