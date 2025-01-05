using ConsoleApp1.Models;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Samples
{
    internal class EFCoreSample(string connectionString)
    {
        public async Task<UserModel?> GetDataAsync()
        {
            using (var context = new AppDbContext(connectionString))
            {
                context.Database.EnsureCreated();

                long cnt = await context.User.CountAsync();

                UserModel? user = await context.User.FirstOrDefaultAsync();

                Console.WriteLine($"RowCount - {cnt}");
                Console.WriteLine($"RowData - {user?.id} {user?.name}");

                return user;
            }
        }
    }

    public class AppDbContext(string connectionString) : DbContext
    {
        public DbSet<UserModel> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(connectionString, MySqlServerVersion.AutoDetect(connectionString));
        }
    }
}
