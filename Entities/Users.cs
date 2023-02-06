using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test_crud.Entities
{
    public class Users
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }


        public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();

        public Users(string username, string password)
        {
            Username= username;
            Password= password;
        }

    }
}
