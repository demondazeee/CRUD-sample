using Microsoft.EntityFrameworkCore;
using test_crud.Entities;

namespace test_crud.DBContext
{
    public class DB : DbContext
    {
        public DbSet<Users> User => Set<Users>();
        public DbSet<Tasks> Tasks => Set<Tasks>();

        public IConfiguration config { get; set; }
        public DB(IConfiguration config) 
        {
            this.config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(this.config["ConnectionStrings:DB"]);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
