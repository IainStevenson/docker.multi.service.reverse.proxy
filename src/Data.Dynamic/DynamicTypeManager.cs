using System;
using System.Collections.Generic;
using System.Linq;

namespace Data.Dynamic
{
    public class DynamicTypeManager : IDynamicTypeManager
    {
        /// <summary>
        ///     Eliminates non-key properties from a dynamic (JObject) object
        /// </summary>
        /// <param name="item">The item to modify</param>
        /// <param name="keys">The allowed properties that will remain in the returned object</param>
        /// <returns>The item modified to remove non-key properties</returns>
        public dynamic ResourceKeys(dynamic item, string[] keys)
        {
            var dict = item.ToObject<Dictionary<string, object>>();

            List<string> actualPropertyNames = DynamicExtensions.GetPropertyKeysForDynamic(item);

            var propertyNamesToRemove = actualPropertyNames.Except(keys, StringComparer.OrdinalIgnoreCase).ToList();

            foreach (var name in propertyNamesToRemove)
                dict.Remove(name);
            var resourceKeys = dict;
            return resourceKeys;
        }
    }
}