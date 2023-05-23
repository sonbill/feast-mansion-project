using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using feast_mansion_project.Models;

namespace feast_mansion_project.Models.Domain
{
	public class ApplicationDbContext : DbContext
    {
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}
        
        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartDetail> CartDetails { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<Feedback> Feedbacks { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CartDetail>()
                .HasOne(cd => cd.Product)
                .WithMany()
                .HasForeignKey(cd => cd.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<User>()
            //    .HasOne(u => u.Customer)
            //    .WithOne(c => c.User)
            //    .HasForeignKey<Customer>(c => c.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Customer)
                .WithOne(c => c.User)
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Feedback>()
            //    .HasOne(f => f.User)
            //    .WithMany()
            //    .HasForeignKey(f => f.UserId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Feedback>()
            //    .HasOne(f => f.Customer)
            //    .WithMany()
            //    .HasForeignKey(f => f.CustomerId)
            //    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Customer)
                .WithMany(c => c.Feedbacks)
                .HasForeignKey(f => f.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();


            //modelBuilder.Entity<CartDetail>()
            //    .HasOne(cd => cd.Product)
            //    .WithMany()
            //    .HasForeignKey(cd => cd.ProductId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<Order>()
            //.HasMany(o => o.OrderDetails)
            //.WithOne(oi => oi.Order)
            //.HasForeignKey(oi => oi.OrderId);

            //modelBuilder.Entity<Cart>()
            //    .HasMany(c => c.CartDetails)
            //    .WithOne(ci => ci.Cart)
            //    .HasForeignKey(ci => ci.CartId);
        }
    }
}

