using System;
using System.Collections.Generic;

namespace SocialMedia.Models
{
    public class User
    {
        public User()
        {
            this.Posts = new List<Post>();
            this.Friends = new List<User>();
            this.Groups = new List<Group>();
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

        public List<Post> Posts { get; set; }
        public List<User> Friends { get; set; }
        public List<Group> Groups { get; set; }

    }

    public enum Gender
    {
        male = 0,
        female = 1
    }
}
