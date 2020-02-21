using Microsoft.EntityFrameworkCore;

namespace BeltExam.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users {get;set;}
        public DbSet<Going> Goings {get;set;}
        public DbSet<Plan> Plans {get;set;}
    }
}