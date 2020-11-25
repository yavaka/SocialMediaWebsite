namespace SocialMedia.Services.JSON
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    public class JsonService<T> : IJsonService<T>
    {
        public IEnumerable<T> GetObjects(string json)
        => JsonConvert
                .DeserializeObject<List<T>>(json)
                .Where(i => i != null)
                .ToList();

        public string SerializeObjects(List<T> objects)
        => JsonConvert.SerializeObject(objects, Formatting.Indented);
    }
}
