using Microsoft.EntityFrameworkCore;
using UserProximity.Models;

namespace UserProximity.API.Data
{
    public class UserDbContext: DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
