using Microsoft.EntityFrameworkCore;

namespace Retrospective.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Record> Records { get; set; }
    }
}