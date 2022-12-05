using Data.Model;

namespace Api.Domain.Handling.Resource
{
    /// <summary>
    /// Handles property removal from <see cref="IResource"/> objects to reduce metadata transfers back to the client.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IResourceContentModifier<T> where T : IResource
    {
        /// <summary>
        /// Modifies a <see cref="IResource"/> to collapse the  <see cref="IResource.Content"/> down to only those properties that are specified by the <see cref="propertyNames"/> collection. 
        /// The property names are usually the key property names which the client nominated for the namespace content type.
        /// </summary>
        /// <param name="source">The Resource instance.</param>
        /// <param name="propertyNames">The property names to be retained</param>
        /// <returns>The modified resource.</returns>
        Task<T> CollapseContent(T source, string contentKeys);
    }
}
