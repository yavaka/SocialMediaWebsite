namespace SocialMedia.Services.Post
{
    using SocialMedia.Services.Common;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPostService : IService
    {
        Task AddPost(PostServiceModel serviceModel);

        Task EditPost(PostServiceModel serviceModel);
        
        Task DeletePost(int id);

        Task<PostServiceModel> GetPost(int id);

        Task<ICollection<PostServiceModel>> GetPostsByUserIdAsync(string userId);

        Task<ICollection<PostServiceModel>> GetPostsByGroupIdAsync(int groupId);
        
        Task<int?> GetGroupIdOfPost(int id);
    }
}
