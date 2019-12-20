using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMedia.Models
{
    [Table("Users")]
    public class User : IdentityUser
    {
        //public User()
        //{
        //    this.Posts = new HashSet<Post>();
        //    this.Friends = new HashSet<User>();
        //    this.Groups = new HashSet<UserInGroup>();
        //    this.Comments = new HashSet<Comment>();
        //}
     
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{this.FirstName} {this.LastName}";
        public string Country { get; set; }
        public DateTime? DOB { get; set; }
        public Gender Gender { get; set; }
        public string Bio { get; set; }
        public string Locale { get; set; } = "en-GB";
        
        [NotMapped]
        public string Message{ get; set; }
        //public virtual ICollection<Post> Posts { get; set; }
        //public virtual ICollection<User> Friends { get; set; }
        //public virtual ICollection<UserInGroup> Groups { get; set; }
        //public virtual ICollection<Comment> Comments { get; set; }

    }

    public enum Gender
    {
        male = 0,
        female = 1
    }
}
