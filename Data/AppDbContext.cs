using Microsoft.EntityFrameworkCore;
using ExGradoBack.Models;

namespace ExGradoBack.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Auth> Auth { get; set; }
        public DbSet<InfoUser> InfoUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auth>()
                .HasOne(a => a.InfoUser)
                .WithOne(i => i.Auth)
                .HasForeignKey<InfoUser>(i => i.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}