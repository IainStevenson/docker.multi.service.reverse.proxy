using Data.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Response.Formatting
{
    /// <summary>
    /// Modifies a <see cref="IResource"/> to collapse the  <see cref="IResource.Content"/> down to only those properties that are specified by the <see cref="propertyNames"/> collection.
    /// </summary>
    /// <typeparam name="T">The concrete Type of the Resource</typeparam>
    /// <typeparam propertyNames="T">The prperty names to be retained/typeparam>
    public interface IResourceContentModifier<T> where T : IResource
    {
        Task<T> CollapseContent(T source, IEnumerable<string> propertyNames);
    }
}
