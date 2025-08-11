using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Entities;

namespace minimal_api.Infrastructure.Db
{
    public class MyDbContext : DbContext
    {
        private readonly IConfiguration _config;

        public MyDbContext(IConfiguration config)
        {
            _config = config;
        }
        public DbSet<Admin> Admin { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    Id = 1,
                    Email = "admin@admin.com",
                    Password = "1234",
                    Profile = "adm"
                }
            );
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _config.GetConnectionString("mysql") ?? throw new Exception("Connection String Is Missing.");

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString)
                );
            }
        }
    }
}