namespace SocialMedia.Services.JSON
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;
    using System.Linq;

    public class JsonService<T> : IJsonService<T>
    {
        public IEnumerable<T> GetObjects(string json)
        {
            var result = JsonConvert
                .DeserializeObject<List<T>>(json);

            return result.Where(i =>i != null).ToList();
        }

        public string SerializeObjects(List<T> objects)
        {
            string result = JsonConvert.SerializeObject(objects, Formatting.Indented);

            return result;
        }
    }
}
