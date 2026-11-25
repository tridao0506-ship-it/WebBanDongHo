using Microsoft.EntityFrameworkCore;
using WatchStore.Models;

namespace WatchStore.Data
{
    /// <summary>
    /// EF Core database context for the watch store.
    /// </summary>
    public class WatchContext : DbContext
    {
        public WatchContext(DbContextOptions<WatchContext> options) : base(options)
        {
        }

        public DbSet<Watch> Watches => Set<Watch>();
    }
}
