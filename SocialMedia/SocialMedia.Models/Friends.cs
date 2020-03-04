using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SocialMedia.Models
{
    public class Friends
    {
        public Friends()
        {
            this.FriendRequests = new List<User>();
        }

        [Key]
        public int Id { get; set; }
        //FK Account
        public string AccountId { get; set; }
        public User Account { get; set; }
        public IList<User> FriendRequests { get; set; }
    }
}
