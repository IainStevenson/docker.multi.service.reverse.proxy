using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Data.Dynamic
{
    public static class DynamicExtensions
    {
        public static List<string> GetPropertyKeysForDynamic(dynamic dynamicToGetPropertiesFor)
        {
            JObject attributesAsJObject = dynamicToGetPropertiesFor;
            var values = attributesAsJObject.ToObject<Dictionary<string, object>>();
            return values.Keys.Select(x => x.ToLower().Trim()).ToList();
        }
    }

}