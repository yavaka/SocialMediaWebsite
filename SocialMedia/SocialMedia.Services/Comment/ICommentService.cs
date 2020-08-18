namespace SocialMedia.Services.Comment
{
    using SocialMedia.Services.Common;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICommentService : IService
    {
        Task AddComment(CommentServiceModel commentServiceModel);

        Task EditComment(CommentServiceModel serviceModel);

        Task DeleteComment(int id);

        Task<CommentServiceModel> GetComment(int id);

        Task<ICollection<CommentServiceModel>> GetCommentsByPostIdAsync(int postId);
    }
}
