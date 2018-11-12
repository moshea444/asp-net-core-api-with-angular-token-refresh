using Domain;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AspNetUser> AspNetUsers { get; set; }

        public DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.ToTable("AspNetUsers");
                entity.HasKey(p => p.Id);
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.ToTable("AspNetUserTokens");
                entity.HasKey(p => new { p.UserId, p.LoginProvider, p.Name });
            });
        }
    }
}
