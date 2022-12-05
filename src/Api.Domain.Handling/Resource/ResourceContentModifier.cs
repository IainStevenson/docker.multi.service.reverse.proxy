using Data.Model;
using Newtonsoft.Json;

namespace Api.Domain.Handling.Resource
{
    public class ResourceContentModifier<T> : IResourceContentModifier<T> where T : IResource
    {
        public Task<T> CollapseContent(T source, string contentKeys)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (string.IsNullOrWhiteSpace(contentKeys))
            {
                return Task.FromResult(source);
            }

            var ownerKeys  = contentKeys.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

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
