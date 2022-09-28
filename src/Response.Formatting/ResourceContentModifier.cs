using Data.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Response.Formatting
{
    public class ResourceContentModifier<T> : IResourceContentModifier<T> where T : IResource
    {
        public Task<T> CollapseContent(T source, IEnumerable<string> ownerKeys)
        {

            // optionally remove the current content
            if (ownerKeys.Any())
            {
                // Transform the stored content to  key value pairs
                Dictionary<string, object> storedContent = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(source.Content));
                
                // extract those key value pairs that are declared as key properties
                var keyItems = storedContent
                    .Where(k => ownerKeys.Contains(k.Key))
                    .ToDictionary(k => k.Key, k => k.Value);
                
                if (ownerKeys.Count() == keyItems.Count())
                {
                    // if all were found, reduce the content to only those key properties
                    var returnedText = JsonConvert.SerializeObject(keyItems);
                    var returnedContent = JsonConvert.DeserializeObject<dynamic>(returnedText);

                    source.Content = returnedContent;
                }                
                return Task.FromResult(source);
            }
            return Task.FromResult(source);
        }
    }
}
