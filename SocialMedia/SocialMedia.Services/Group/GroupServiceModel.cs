namespace SocialMedia.Services.Group
{
    using SocialMedia.Services.User;
    using System.Collections.Generic;

    public class GroupServiceModel
    {
        public GroupServiceModel()
        {
            this.Members = new List<UserServiceModel>();
        }

        public int GroupId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string AdminId { get; set; }

        public ICollection<UserServiceModel> Members { get; set; }
    }
}
