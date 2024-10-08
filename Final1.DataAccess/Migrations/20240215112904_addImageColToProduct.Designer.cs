﻿// <auto-generated />
using FinalWeb1.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FinalWeb1.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240215112904_addImageColToProduct")]
    partial class addImageColToProduct
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FinalWeb1.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DisplayOrder = 1,
                            Name = "Sweater"
                        },
                        new
                        {
                            Id = 2,
                            DisplayOrder = 2,
                            Name = "Hoodie"
                        },
                        new
                        {
                            Id = 3,
                            DisplayOrder = 3,
                            Name = "Shoe"
                        });
                });

            modelBuilder.Entity("FinalWeb1.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Material")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<string>("Size")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CategoryId = 2,
                            Description = "Praesent vitae sodales libero.. ",
                            Gender = "Male",
                            ImageUrl = "",
                            Material = "Cotton",
                            Name = "Basic Hoodie",
                            Price = 5.9900000000000002,
                            Size = "M"
                        },
                        new
                        {
                            Id = 2,
                            CategoryId = 1,
                            Description = "Praesent vitae sodales libero.. ",
                            Gender = "Unisex",
                            ImageUrl = "",
                            Material = "Draper",
                            Name = "Basic Sweater",
                            Price = 6.9900000000000002,
                            Size = "M"
                        },
                        new
                        {
                            Id = 3,
                            CategoryId = 4,
                            Description = "Praesent vitae sodales libero..Praesent vitae sodales libero..Praesent vitae sodales libero..Praesent vitae sodales libero..Praesent vitae sodales libero..",
                            Gender = "Unisex",
                            ImageUrl = "",
                            Material = "Cotton",
                            Name = "Oversized T-Shirt",
                            Price = 3.9900000000000002,
                            Size = "XL"
                        });
                });

            modelBuilder.Entity("FinalWeb1.Models.Product", b =>
                {
                    b.HasOne("FinalWeb1.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });
#pragma warning restore 612, 618
        }
    }
}
