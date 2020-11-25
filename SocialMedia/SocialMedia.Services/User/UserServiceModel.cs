namespace SocialMedia.Services.User
{
    using System;

    public class UserServiceModel
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Country { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}
