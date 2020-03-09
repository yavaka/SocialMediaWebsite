using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SocialMedia.Models
{
    public class Friendship
    {
        public string RequesterId { get; set; }
        public string AddresseeId { get; set; }
        
        //0 = Pending
        //1 = Accepted
        public int Status { get; set; }

        public virtual User Addressee { get; set; }
        public virtual User Requester { get; set; }
    }
}
