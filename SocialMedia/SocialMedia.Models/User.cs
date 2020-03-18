using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMedia.Models
{
    [Table("Users")]
    public class User : IdentityUser
    {
        public User()
        {
            this.Posts = new HashSet<Post>();
            this.Comments = new HashSet<Comment>();
            this.FriendshipAddressee = new HashSet<Friendship>();
            this.FriendshipRequester = new HashSet<Friendship>();
            this.Groups = new HashSet<UserInGroup>();
        }
      
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{this.FirstName} {this.LastName}";
        public string Country { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }

        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; }
        [DataType(DataType.MultilineText)]
        public string Bio { get; set; }
        public string Locale { get; set; } = "en-GB";

        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Friendship> FriendshipAddressee { get; set; }
        public virtual ICollection<Friendship> FriendshipRequester { get; set; }
        public virtual ICollection<UserInGroup> Groups { get; set; }

        [NotMapped]
        public string Message{ get; set; }

    }

    public enum Gender
    {
        male = 0,
        female = 1
    }
}
