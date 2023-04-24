using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace UserRepositoryEF
{
    public class IdentityContext : IdentityDbContext<User, Role, int>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Role>().HasData(
                new Role { Name = "NormalUser" },
                new Role { Name = "Librarian" },
                new Role { Name = "Administrator" });

            builder.Entity<User>().HasData(
                new User { UserName = "admin", Passwrod = "123", Email = "123@test.pl" });
        }
    }
}
