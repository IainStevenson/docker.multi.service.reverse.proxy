using Data.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Response.Formater
{
    public class ResourceContentModifier<T> : IResourceContentModifier<T> where T : IResource
    {
        public Task<T> CollapseContent(T source, IEnumerable<string> ownerKeys)
        {

            // optionally remove the current content
            if (ownerKeys.Any())
            {
                Dictionary<string, object> storedContent = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(source.Content));

                var keyItems = storedContent
                    .Where(k => ownerKeys.Contains(k.Key))
                    .ToDictionary(k => k.Key, k => k.Value);

                var returnedText = JsonConvert.SerializeObject(keyItems);
                var returnedContent = JsonConvert.DeserializeObject<dynamic>(returnedText);

                source.Content = returnedContent;
                return Task.FromResult(source);
            }
            return Task.FromResult(source);
        }
    }
}
