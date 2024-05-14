using Microsoft.EntityFrameworkCore;
using _NET_Realtime_Chat.Models;

namespace _NET_Realtime_Chat.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
