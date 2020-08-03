
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using OnlineShoping.Models;
using Microsoft.EntityFrameworkCore;
using OnlineShoping.Models.DatabaseModel;

namespace OnlineShoping.Services
{
   public class AppDbContext : IdentityDbContext<ApplicationUser>
    {

        public AppDbContext(DbContextOptions<AppDbContext> options)
          : base(options)
        { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<ProductImage> ProductImage { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<Comment> Comments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            

            modelBuilder.Entity<ProductColor>()
          .HasKey(bc => new { bc.ProductId, bc.ColorId });

            modelBuilder.Entity<ProductColor>()
                .HasOne(bc => bc.Product)
                .WithMany(b => b.ProductColor)
                .HasForeignKey(bc => bc.ProductId);

            modelBuilder.Entity<ProductColor>()
                .HasOne(bc => bc.Color)
                .WithMany(c => c.ProductColor)
                .HasForeignKey(bc => bc.ColorId);


           
        }
    }
}
