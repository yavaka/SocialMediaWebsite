using System;

namespace SocialMedia.Models
{
    public class User
    {
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

    }

    public enum Gender
    {
        male = 0,
        female = 1
    }
}
