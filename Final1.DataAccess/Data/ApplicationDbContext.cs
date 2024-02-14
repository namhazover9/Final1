using FinalWeb1.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalWeb1.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Sweater", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Hoodie", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Shoe", DisplayOrder = 3 }
                );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Basic Hoodie",
                    Description = "Praesent vitae sodales libero.. ",
                    Price = 5.99,
                    Size = "M",
                    Gender = "Male",
                    Material = "Cotton",
                    CategoryId = 2
                },
                new Product
                {
                    Id = 2,
                    Name = "Basic Sweater",
                    Description = "Praesent vitae sodales libero.. ",
                    Price = 6.99,
                    Size = "M",
                    Gender = "Unisex",
                    Material = "Draper",
                    CategoryId = 1
                },
                new Product
                {
                    Id = 3,
                    Name = "Oversized T-Shirt",
                    Description = "Praesent vitae sodales libero..Praesent vitae sodales libero..Praesent vitae sodales libero..Praesent vitae sodales libero..Praesent vitae sodales libero..",
                    Price = 3.99,
                    Size = "XL",
                    Gender = "Unisex",
                    Material = "Cotton",
                    CategoryId = 1
                });

        }
    }
    
    
}
