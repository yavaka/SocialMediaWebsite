using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Models.ViewModels
{
    public class GroupViewModel : IViewModel
    {
        public GroupViewModel()
        {
            this.Groups = new List<Group>();
            this.MemberGroups = new List<Group>();
            this.NonMemberGroups = new List<Group>();
            this.Posts = new List<Post>();
        }
        public Group Group{ get; set; }
        public User CurrentUser { get; set; }
        public ICollection<Group> Groups{ get; set; }
        public ICollection<Post> Posts{ get; set; }
        public ICollection<Group> NonMemberGroups{ get; set; }
        public ICollection<Group> MemberGroups{ get; set; }
    }
}
