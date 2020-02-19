using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Models;
using SocialMedia.Models.IdentityModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Data
{
    public class SocialMediaDbContext : IdentityDbContext<User>
    {
        public SocialMediaDbContext(DbContextOptions<SocialMediaDbContext> options) 
            : base(options)
        {
        }

        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }

        //public virtual DbSet<UserInGroup> UsersInGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(user => user.HasIndex(x => x.Locale)
                                             .IsUnique(false));

            //User has many friends
            modelBuilder.Entity<User>()
                .HasMany<User>(f => f.Friends)
                .WithOne()
                .HasForeignKey(u => u.Id);

            //User has many friend requests
            modelBuilder.Entity<User>()
                .HasMany<User>(fr => fr.FriendRequests)
                .WithOne()
                .HasForeignKey(u => u.Id);


            //    //User has many posts
            //    modelBuilder.Entity<Post>()
            //        .HasOne<User>(a => a.Author)
            //        .WithMany(u => u.Posts)
            //        .HasForeignKey(id => id.AuthorId);

            //    //Mapping table PKs
            //    modelBuilder.Entity<UserInGroup>()
            //        .HasKey(ug => new { ug.UserId, ug.GroupId });

            //    //User has many groups (Mapping table)
            //    modelBuilder.Entity<UserInGroup>()
            //        .HasOne(u => u.User)
            //        .WithMany(g => g.Groups)
            //        .HasForeignKey(uId => uId.UserId);

            //    //User has many groups (Mapping table)
            //    modelBuilder.Entity<UserInGroup>()
            //        .HasOne(g => g.Group)
            //        .WithMany(u => u.Members)
            //        .HasForeignKey(gId => gId.GroupId);

            //    //Post has tagged users
            //    modelBuilder.Entity<Post>()
            //        .HasMany(t => t.TaggedFriend);

            //    //Group has many posts
            //    modelBuilder.Entity<Group>()
            //        .HasMany(p => p.Posts);

            //    //Post has many comments
            //    modelBuilder.Entity<Post>()
            //        .HasMany(c => c.Comments);

            //    //Comment has a author
            //    modelBuilder.Entity<Comment>()
            //        .HasOne(a => a.Author)
            //        .WithMany(c => c.Comments)
            //        .HasForeignKey(aId => aId.AuthorId);

            //    //Comment has tagged friends
            //    modelBuilder.Entity<Comment>()
            //        .HasMany(t =>t.TaggedFriends);
        }

        }
}
