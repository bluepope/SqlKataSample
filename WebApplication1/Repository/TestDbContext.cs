using Microsoft.EntityFrameworkCore;

using System.Reflection;

using WebApplication1.Repository.Entities;

namespace WebApplication1.Repository
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public virtual DbSet<UserModel> User { get; set; }
    }
}
