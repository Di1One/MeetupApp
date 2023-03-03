using MeetupApp.DataBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetupApp.DataBase
{
    public class MeetupAppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public MeetupAppDbContext(DbContextOptions<MeetupAppDbContext> options)
            : base(options)
        {
        }
    }
}
