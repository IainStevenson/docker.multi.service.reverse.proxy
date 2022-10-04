using FluentValidation;
using MediatR;
using Storage;

namespace Api.Domain.Storage.Delete
{
    public class ResourceStorageDeleteRequestHandler : IRequestHandler<ResourceStorageDeleteRequest, ResourceStorageDeleteResponse>
    {
        private readonly IRepository<Data.Model.Storage.Resource> _storage;
        private readonly AbstractValidator<ResourceStorageDeleteRequest> _validator;
        private const int BADREQUEST = 400;
        private const int NOTFOUND = 404;
        private const int ALREADYGONE = 410;
        private const int PRECONDITIONFAILED = 412;
        private const int NOCONTENT = 204;


        public ResourceStorageDeleteRequestHandler(IRepository<Data.Model.Storage.Resource> storage, ResourceStorageDeleteRequestValidator requestValidator)
        {
            _storage = storage;
            _validator = requestValidator;
        }

        public async Task<ResourceStorageDeleteResponse> Handle(ResourceStorageDeleteRequest request, CancellationToken cancellationToken)
        {

            var response = new ResourceStorageDeleteResponse() { };

            var validationResult = _validator.Validate(request);

            if (!validationResult.IsValid)
            {
                response.RequestValidationErrors.AddRange(validationResult.Errors.Select(x => x.ErrorMessage));
                response.StatusCode = BADREQUEST;
            }


            Data.Model.Storage.Resource? resource = (await _storage.GetAsync(r => r.Id == request.Id
                                                                                   && r.OwnerId == request.OwnerId
                                                                                   && r.Namespace == request.Namespace
                                                                                   )).FirstOrDefault();


            if (resource == null)
            {
                response.StatusCode = NOTFOUND;
                return response;
            }


            // only proceed if resource is unmodified since or is one of the etags
            if (
                    (resource.Modified.HasValue ? resource.Modified.Value <= request.IsUnchangedSince : resource.Created <= request.IsUnchangedSince) 
                    ||
                    request.IsETags.Contains(resource.Etag)
                    )
            {

                var count = await _storage.DeleteAsync(request.Id);
                if (count == 1)
                {
                    response.StatusCode = NOCONTENT;
                    return response;
                }
                response.RequestValidationErrors.Add($"The resource deletion was attempted but did not happen. This indicates that it has gone already.");

                response.StatusCode = ALREADYGONE;
                return response;
            }
            else
            {

                if (request.IsETags.Any())
                {
                    response.RequestValidationErrors.Add($"The resource has None of the specified ETags {string.Join(',', request.IsETags)}/r/n");
                }
                if (request.IsUnchangedSince != DateTimeOffset.MinValue)
                {
                    response.RequestValidationErrors.Add($"The resource has been modified since {request.IsUnchangedSince}");
                }
                response.StatusCode = PRECONDITIONFAILED;
                return response;
            }
        }
    }
}
