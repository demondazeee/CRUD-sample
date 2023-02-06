namespace test_crud.Models
{
    public class TaskDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public TaskStatus TaskStatus { get; set; }
    }
}
