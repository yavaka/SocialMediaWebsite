namespace SocialMedia.Services.JSON
{
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;
    using System.Linq;

    public class JsonService<T> : IJsonService<T>
    {
        public IEnumerable<T> GetObjects(string json)
        {
            JArray jsonArray = JArray.Parse(json);

            var result = new List<T>();

            foreach (var jToken in jsonArray)
            {
                result.Add(jToken.ToObject<T>());
            }

            return result.Where(i =>i != null).ToList();
        }
    }
}
