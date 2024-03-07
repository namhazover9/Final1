using FinalWeb1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinalWeb1.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCondition> ProductConditions { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Men's Clothing & Shoes", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Women's Clothing & Shoes", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Unisex Clothing", DisplayOrder = 3 }
                );

            modelBuilder.Entity<ProductCondition>().HasData(
                new ProductCondition { ConditionId = 1, Name = "New" },
                new ProductCondition { ConditionId = 2, Name = "Used - Like New" },
                new ProductCondition { ConditionId = 3, Name = "Used - Good" },
                new ProductCondition { ConditionId = 4, Name = "Used - Fair" }
                );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Basic Hoodie",
                    Description = "I would like to pass my used hoodie, 90% new. Size XL. ",
                    Price = 3.49,                   
                    CategoryId = 2,

                },
                new Product
                {
                    Id = 2,
                    Name = "Ananas Shoes",
                    Description = "Size 38. 90% New.",
                    Price = 6.99,
                    CategoryId = 2,

                },
                new Product
                {
                    Id = 3,
                    Name = "Green Dress",
                    Description = "Only worn once, need to pass at a good price.",
                    Price = 5.99,
                    CategoryId = 2,

                    
                },
                new Product
                {
                    Id = 4,
                    Name = "Baggy Jeans",
                    Description = "From popular brand 3tStreetWear. Like new, size L.",
                    Price = 5.99,
                    CategoryId = 2,

                });
        }
    }
    
    
}
