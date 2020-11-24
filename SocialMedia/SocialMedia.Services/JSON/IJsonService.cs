namespace SocialMedia.Services.JSON
{
    using System.Collections.Generic;

    public interface IJsonService<T>
    {
        IEnumerable<T> GetObjects(string json);
    }
}
