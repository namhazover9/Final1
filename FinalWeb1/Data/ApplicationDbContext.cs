using FinalWeb1.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalWeb1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Sweater", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Hoodie", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Shoe", DisplayOrder = 3 }
                );
        }
    }
    
    
}
