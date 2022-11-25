using Data.Model.Storage;

namespace Api.Domain.Storage
{
    /// <summary>
    /// Validates the requested action on the resource according to the request constraints and sets the validation outcome into the response.
    /// </summary>
    /// <typeparam name="TRequest">The request specifying the constraints.</typeparam>
    /// <typeparam name="TResponse">The response which will carry the validation result.</typeparam>
    public interface IResourceStorageActionValidator<TRequest, TResponse>
    {
        /// <summary>
        /// Validate the action may proceeed based on the specified request constraints.
        /// </summary>
        /// <param name="resource">The resouce to validate.</param>
        /// <param name="request">The resource constraints to evaluate.</param>
        /// <param name="response">The response carrying the results of validation</param>
        /// <returns>A <see cref="Tuple"/> object pair of the provided and maybe modified <see cref="Resource"/>, and the results of the action validation in the modified <see cref="TResponse"/>.</returns>
        (Resource?, TResponse) Validate(Resource? resource, TRequest request, TResponse response);
        
    }
}
