using Microsoft.EntityFrameworkCore;
using SocialMedia.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Data
{
    public class SocialMediaDbContext : DbContext
    {
        private const string CONNECTION_STRING = @"Server=(localdb)\mssqllocaldb; Database=SocialMedia; Integrated Security=True; Trusted_Connection=True";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(CONNECTION_STRING);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }

    }
}
