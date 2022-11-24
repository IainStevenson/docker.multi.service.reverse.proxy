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
            response.StatusCode = PRECONDITIONFAILED;

            if ((resource.Modified.HasValue ?
                        resource.Modified > request.IsUnchangedSince :
                        resource.Created > request.IsUnchangedSince)
                    )
            {
                response.RequestValidationErrors.Add($"Deletion failed, as the resource has been modified since {request.IsUnchangedSince}");
                return response;
            }

            if (request.IsNotETags.Any() && !request.IsNotETags.Contains(resource.Etag))
            {
                response.RequestValidationErrors.Add($"Deletion failed, as the resource has None of the specified ETags {string.Join(',', request.IsNotETags)}/r/n");
                return response;

            }

            var count = await _storage.DeleteAsync(request.Id);
            response.StatusCode = NOCONTENT;
            return response;

        }
    }
}

