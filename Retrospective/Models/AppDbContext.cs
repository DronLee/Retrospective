using Microsoft.EntityFrameworkCore;

namespace Retrospective.Models
{
    /// <summary>
    /// Класс описывает БД.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options)
            : base(options)
        {
        }

        /// <summary>
        /// Темы.
        /// </summary>
        public DbSet<Subject> Subjects { get; set; }

        /// <summary>
        /// Записи.
        /// </summary>
        public DbSet<Record> Records { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Создание индекса для ускорения поиска.
            modelBuilder.Entity<Record>().HasIndex(r => new { r.SubjectId, r.CreatedOn });
        }
    }
}