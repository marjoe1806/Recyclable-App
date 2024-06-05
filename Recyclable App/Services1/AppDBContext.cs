using Microsoft.EntityFrameworkCore;
using Recyclable_App.Models;
using Recyclable_App.Models.Service1;

namespace Recyclable_App.Services1
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<RecyclableTypes> RecyclableTypes { get; set; }
        public DbSet<RecyclableItems> RecyclableItems { get; set; }
    }
   
}
