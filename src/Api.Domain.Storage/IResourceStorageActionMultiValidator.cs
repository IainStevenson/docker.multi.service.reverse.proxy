using Data.Model.Storage;

namespace Api.Domain.Storage
{
    public interface IResourceStorageActionMultiValidator<TRequest, TResponse>
    {
        /// <summary>
        /// Validate the action may proceeed based on the specified request constraints.
        /// Will decide the return code and the actual contents of the resources collection.
        /// It may strip unchanged resosurces from the collection if the request specifies a since DateTime
        /// </summary>
        /// <param name="resources">The resouces to validate.</param>
        /// <param name="request">The resource constraints to evaluate.</param>
        /// <param name="response">The response carrying the results of validation</param>
        /// <returns>A <see cref="Tuple"/> object pair of the provided and maybe modified <see cref="IEnumerable{Resource}<"/>, and the results of the action validation in the modified <see cref="TResponse"/>.</returns>
        (IEnumerable<Resource>, TResponse) Validate(IEnumerable<Resource> resources, TRequest request, TResponse response);
    }
}
