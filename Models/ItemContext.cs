﻿using Microsoft.EntityFrameworkCore;

namespace my_eshop_api.Models
{
    public class ItemContext : DbContext
    {
        public ItemContext(DbContextOptions<ItemContext> options)
            : base(options)
        {
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>().Property(p => p.Price).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Order>().Property(p => p.TotalPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<OrderDetail>().Property(p => p.ItemUnitPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<OrderDetail>().Property(p => p.Quantity).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<OrderDetail>().Property(p => p.TotalPrice).HasColumnType("decimal(18,2)");
        }
    }
}