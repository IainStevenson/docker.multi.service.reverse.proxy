﻿using Data.Model.Storage;
using FluentValidation;
using MediatR;
using Storage;

namespace Api.Domain.Storage.Put
{
    public class ResourceStoragePutRequestHandler : IRequestHandler<ResourceStoragePutRequest, ResourceStoragePutResponse>
    {
        private readonly IRepository<Resource> _storage;
        private readonly AbstractValidator<ResourceStoragePutRequest> _requestValidator;
        private const int BADREQUEST = 400;
        private const int NOTFOUND = 404;
        private const int UPDATED = 200;
        private const int PRECONDITIONFAILED = 412;

        public ResourceStoragePutRequestHandler(IRepository<Resource> storage, ResourceStoragePutRequestValidator requestValidator)
        {
            _storage = storage;
            _requestValidator = requestValidator;
        }

        public async Task<ResourceStoragePutResponse> Handle(ResourceStoragePutRequest request, CancellationToken cancellationToken)
        {

            var response = new ResourceStoragePutResponse() { };
            var validationResult = _requestValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                response.StatusCode = BADREQUEST;
                response.RequestValidationErrors = validationResult.Errors.Select(x => $"{x.PropertyName}\t{x.ErrorCode}\t{x.ErrorMessage}").ToList();
                return response;
            }

            Resource? existingResource = (await _storage.GetAsync(r => r.Id == request.Id
                                                                                && r.OwnerId == request.OwnerId
                                                                                && r.Namespace == request.Namespace
                                                                                )).FirstOrDefault();
            if (existingResource == null)
            {
                response.StatusCode = NOTFOUND;
                return response;
            }


            if (
                    !
                    (existingResource.Modified.HasValue ?
                    existingResource.Modified.Value <= request.UnmodifiedSince :
                    existingResource.Created <= request.UnmodifiedSince) || (request.ETags.Contains(existingResource.Etag))
                    )
            {
                if (request.ETags.Any())
                {
                    response.RequestValidationErrors.Add($"The resource has None of the specified ETags {string.Join(',', request.ETags)} and therefore has not been updated./r/n");
                }
                if (request.UnmodifiedSince != DateTimeOffset.MaxValue)
                {
                    response.RequestValidationErrors.Add($"The resource has been modified since {request.UnmodifiedSince} and therefore has not been updated.");
                }
                response.StatusCode = PRECONDITIONFAILED;
                return response;
            }

            existingResource.Content = request.Content;
            existingResource.Modified = DateTimeOffset.UtcNow;
            existingResource.Metadata.Tags.Add(new Tag() { Name = MetadataPropertyNames.ChangeRequestIdentifier, Value = request.RequestId });
            existingResource.Metadata.Tags.Add(new Tag() { Name = MetadataPropertyNames.Updated, Value = existingResource.Modified });

            if (!string.IsNullOrWhiteSpace(request.MoveTo))
            {
                existingResource.Namespace = request.MoveTo;
                existingResource.Metadata.Tags.Add(new Tag() { Name = MetadataPropertyNames.NamespaceRename, Value = existingResource.Namespace });
            }
            
            response.Model = await _storage.UpdateAsync(existingResource); ;
            response.StatusCode = UPDATED;
            return response;
        }



    }
}
