using GetWorkItems.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace GetWorkItems {
    public class GetWorkItemContext : DbContext {
        public DbSet<WorkItem> WorkItems { get; set; }
        public DbSet<Configuration> Configurations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ManagerWorkItems;Trusted_Connection=true;");
        }
    }
}