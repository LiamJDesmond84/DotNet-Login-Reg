using Microsoft.EntityFrameworkCore;

namespace DotNet_Login_Reg.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }  

    }
}
