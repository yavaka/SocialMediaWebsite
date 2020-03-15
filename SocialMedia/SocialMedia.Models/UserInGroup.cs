using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Models
{
    public class UserInGroup
    {
        public string UserId { get; set; }
        public User User{ get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }

        public bool Admin { get; set; } = false;
    }
}
