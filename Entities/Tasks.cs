using System.ComponentModel.DataAnnotations.Schema;

namespace test_crud.Entities
{
    public enum TasksStatus
    {
        ToDo,
        InProgress,
        Done
    }
    public class Tasks
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public TasksStatus TaskStatus { get; set; }


        [ForeignKey("OwnerId")]
        public Guid OwnerId { get; set; }

        public Users? Owner { get; set; }

        public Tasks(string name)
        {
            Name= name;
        }


    }
}
