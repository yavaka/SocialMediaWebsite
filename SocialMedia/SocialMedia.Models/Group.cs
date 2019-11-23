using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SocialMedia.Models
{
    public class Group
    {
        public Group()
        {
            this.Members = new List<User>();
            this.Posts = new List<Post>();
            this.Admins = new List<User>();
        }

        public int GroupId{ get; set; }
        public string Title{ get; set; }
        public string Description { get; set; }
        public ICollection<User> Members { get; set; }
        public ICollection<Post> Posts{ get; set; }
        public ICollection<User> Admins{ get; set; }

    }
}
