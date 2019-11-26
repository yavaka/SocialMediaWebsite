using System;
using System.Collections.Generic;

namespace SocialMedia.Models
{
    public class User
    {
        public User()
        {
            this.Posts = new HashSet<Post>();
            this.Friends = new HashSet<User>();
            this.Groups = new HashSet<UserInGroup>();
            this.Comments = new HashSet<Comment>();
        }

        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Country { get; set; }
        public DateTime DOB { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{this.FirstName} {this.LastName}";
        public Gender Gender { get; set; }
        public string Bio { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<User> Friends { get; set; }
        public virtual ICollection<UserInGroup> Groups { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

    }

    public enum Gender
    {
        male = 0,
        female = 1
    }
}
