using FluentValidation;
using MediatR;
using Storage;

namespace Api.Domain.Storage.Get
{
    public class ResourceStorageGetOneRequestHandler : IRequestHandler<ResourceStorageGetOneRequest, ResourceStorageGetOneResponse>
    {
        private readonly IRepository<Data.Model.Storage.Resource> _storage;
        private readonly AbstractValidator<ResourceStorageGetOneRequest> _requestValidator;
        private const int BADREQUEST = 400;
        private const int NOTFOUND = 404;
        private const int NOTMODIFIED = 304;
        private const int OK = 200;


        public ResourceStorageGetOneRequestHandler(IRepository<Data.Model.Storage.Resource> storage, ResourceStorageGetOneRequestValidator requestValidator)
        {
            _storage = storage;
            _requestValidator = requestValidator;
        }

        public async Task<ResourceStorageGetOneResponse> Handle(ResourceStorageGetOneRequest request, CancellationToken cancellationToken)
        {
            var response = new ResourceStorageGetOneResponse();

            var validationResult = _requestValidator.Validate(request);

            if (!validationResult.IsValid)
            {
                response.RequestValidationErrors = validationResult.Errors.Select(x => $"{x.PropertyName}\t{x.ErrorCode}\t{x.ErrorMessage}").ToList();
                response.StatusCode = BADREQUEST;
            }

            Data.Model.Storage.Resource? resource = (await _storage.GetAsync(r => r.Id == request.Id
                                                                                && r.OwnerId == request.OwnerId
                                                                                && r.Namespace == request.Namespace
                                                                                )).SingleOrDefault();


            if (resource == null)
            {
                response.StatusCode = NOTFOUND; 
                return response;
            }

            if (request.IfNotETags.Any() && request.IfNotETags.Contains(resource.Etag))
            {
                response.StatusCode = NOTMODIFIED; 
                return response;
            }


            var resourceHasNotBeenModifiedSince = !(resource.Modified.HasValue ?
                                                        resource.Modified > request.IfModifiedSince :
                                                        resource.Created > request.IfModifiedSince);
            if (resourceHasNotBeenModifiedSince)
            {
                response.StatusCode = NOTMODIFIED;
                return response;
            }

            response.Model = resource;
            response.StatusCode = OK;
            return response;
        }
    }

}
