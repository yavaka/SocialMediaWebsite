using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Models
{
    public class Group
    {
        public Group()
        {
            this.Members = new List<User>();
            this.Posts = new List<Post>();
        }

        public int GroupId{ get; set; }
        public string Title{ get; set; }
        public string Description { get; set; }
        public List<User> Members { get; set; }
        public List<Post> Posts{ get; set; }
        public List<User> Admins{ get; set; }

    }
}
