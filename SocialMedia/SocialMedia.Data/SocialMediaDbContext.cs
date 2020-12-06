namespace SocialMedia.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using SocialMedia.Data.Models;

    public class SocialMediaDbContext : IdentityDbContext<User>
    {
        public SocialMediaDbContext(DbContextOptions<SocialMediaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Group> Groups { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<UserInGroup> UsersInGroups { get; set; }
        public DbSet<TagFriendInPost> TagFriendsInPosts { get; set; }
        public DbSet<TagFriendInComment> TagFriendsInComments { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ImageData> ImagesData { get; set; }
        public DbSet<Avatar> ProfilePictures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Tag friend in comment 
            modelBuilder.Entity<TagFriendInComment>(entity =>
            {
                entity.ToTable("TagFriendsInComments");

                entity.HasKey(pk => pk.Id);

                //Tagger
                entity.HasOne(t => t.Tagger)
                    .WithMany(u => u.TaggerInComments)
                    .HasForeignKey(t => t.TaggerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TagFriendToTaggerInComment_FK");

                //Tagged
                entity.HasOne(t => t.Tagged)
                   .WithMany(u => u.TaggedInComments)
                   .HasForeignKey(t => t.TaggedId)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("TagFriendToTaggedInComment_FK");

                //Comment
                entity.HasOne(t => t.Comment)
                .WithMany(c => c.TaggedUsers)
                .HasForeignKey(t => t.CommentId)
                .HasConstraintName("TagFreindToComment_FK");
            });

            //Tag friend in post
            modelBuilder.Entity<TagFriendInPost>(entity =>
            {
                entity.ToTable("TagFriendsInPosts");

                entity.HasKey(pk => pk.Id)
                .HasName("TagFriendsInPosts_PK");

                //Tagger
                entity.HasOne(t => t.Tagger)
                    .WithMany(u => u.TaggerInPosts)
                    .HasForeignKey(t => t.TaggerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TagFriendsToTaggerInPost_FK");

                //Tagged
                entity.HasOne(t => t.Tagged)
                    .WithMany(u => u.TaggedInPosts)
                    .HasForeignKey(t => t.TaggedId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TagFriendsToTaggedInPost_FK");

                //Post
                entity.HasOne(t => t.Post)
                .WithMany(p => p.TaggedUsers)
                .HasForeignKey(t => t.PostId)
                .HasConstraintName("TagFriendsToPost_FK");
            });

            //Friendships
            modelBuilder.Entity<Friendship>(entity =>
            {
                entity.ToTable("Friendships");

                entity.HasKey(pk => new { pk.RequesterId, pk.AddresseeId })
                    .HasName("Friendship_PK");

                //Addressee
                entity.HasOne(f => f.Addressee)
                    .WithMany(u => u.FriendshipAddressee)
                    .HasForeignKey(f => f.AddresseeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FriendshipToAddressee_FK");

                //Requester
                entity.HasOne(f => f.Requester)
                    .WithMany(u => u.FriendshipRequester)
                    .HasForeignKey(f => f.RequesterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FriendshipToRequester_FK");

            });

            //Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email)
                    .IsUnique(true);

                entity.HasIndex(x => x.Locale)
                    .IsUnique(false);

                entity.HasIndex(e => e.UserName)
                    .HasName("User_AK2")
                    .IsUnique();

                entity.HasIndex(e => new { e.FirstName, e.LastName })
                    .HasName("User_AK1")
                    .IsUnique();

                entity.Property(e => e.FirstName)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.LastName)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasMany(i => i.Images)
                    .WithOne(u => u.Uploader)
                    .HasForeignKey(uId => uId.UploaderId);
            });

            modelBuilder.Entity<Avatar>(entity =>
            {
                entity.HasOne(i => i.Uploader)
                    .WithOne(a => a.Avatar)
                    .HasForeignKey<Avatar>(i => i.UploaderId);
            });

            //Comments
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasOne(a => a.Author)
                .WithMany(c => c.Comments)
                .HasForeignKey(aId => aId.AuthorId);


                entity.HasOne(p => p.CommentedPost)
                .WithMany(c => c.Comments)
                .HasForeignKey(pId => pId.CommentedPostId);
            });

            //Posts
            modelBuilder.Entity<Post>(entity =>
            {
                //Post has a author
                entity.HasOne<User>(a => a.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(id => id.AuthorId);

                //Post has many comments
                entity.HasMany<Comment>(c => c.Comments)
                .WithOne(p => p.CommentedPost)
                .HasForeignKey(i => i.CommentedPostId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            //UsersInGroups (Mapping table)
            modelBuilder.Entity<UserInGroup>(entity =>
            {
                entity.ToTable("UsersInGroups");

                //PKs
                entity.HasKey(ug => new { ug.UserId, ug.GroupId });

                //User has many groups
                entity.HasOne(u => u.User)
                    .WithMany(g => g.Groups)
                    .HasForeignKey(uId => uId.UserId);

                //Group has many members(Users)
                entity.HasOne(g => g.Group)
                    .WithMany(u => u.Members)
                    .HasForeignKey(gId => gId.GroupId);

            });

            //Group
            modelBuilder.Entity<Group>(entity =>
            {
                //Group has many posts
                entity.HasMany(p => p.Posts)
                .WithOne(g => g.Group)
                .HasForeignKey(gId => gId.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(t => t.Title)
                .IsUnique();

            });

            base.OnModelCreating(modelBuilder);
        }

    }
}
