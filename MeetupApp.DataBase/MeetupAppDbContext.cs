using MeetupApp.DataBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetupApp.DataBase
{
    public class MeetupAppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RefreshToken> RerfreshTokens { get; set; }

        public MeetupAppDbContext(DbContextOptions<MeetupAppDbContext> options)
            : base(options)
        {
        }
    }
}
